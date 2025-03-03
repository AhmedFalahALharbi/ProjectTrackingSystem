using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectTrackingSystem.Models;
using Dapper;
using System.Data.SqlClient;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Initialize the database context
        using (var context = new ApplicationDbContext())
        {
            // 1. Ensure the database is created (if not already)
            context.Database.EnsureCreated();

            // 2. Add some data for testing (optional)
            AddSampleData(context);

            // 3. Task 1 - Query to find employees who have worked on more than 3 projects in the last 6 months
            FindEmployeesWithMoreThan3ProjectsIn6Months(context);

            // 4. Task 2 - Use Dapper to optimize a query that retrieves all employees with their projects
            GetEmployeeProjectsWithDapper();

            // 5. Task 3 - Implement stored procedure execution using Dapper to calculate bonuses
            CalculateEmployeeBonusesWithDapper();

            // 6. Task 4 - Compare Entity Framework Core vs Dapper for fetching financial reports
            CompareEFCoreVsDapperForFinancialReports(context);
        }
    }

    // 1. Task - Find employees who have worked on more than 3 projects in the last 6 months
    static void FindEmployeesWithMoreThan3ProjectsIn6Months(ApplicationDbContext context)
    {
        var sixMonthsAgo = DateTime.Now.AddMonths(-6);
        var employees = context.Employees
            .Where(e => e.EmployeeProjects
                .Any(ep => ep.Project.Deadline >= sixMonthsAgo))
            .GroupBy(e => e.EmployeeId)
            .Where(g => g.Count() > 3)
            .Select(e => e.FirstOrDefault())
            .ToList();

        Console.WriteLine("Employees who have worked on more than 3 projects in the last 6 months:");
        foreach (var employee in employees)
        {
            Console.WriteLine($"Employee: {employee.Name}");
        }
    }

    // 2. Task - Use Dapper to fetch all employees with their projects (optimizing the query)
    static void GetEmployeeProjectsWithDapper()
    {
        string connectionString = "Server=localhost;Database=TrackingDB;Trusted_Connection=True;TrustServerCertificate=True;";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = @"
                SELECT e.Name AS EmployeeName, p.Name AS ProjectName, p.Deadline AS ProjectDeadline
                FROM Employees e
                JOIN EmployeeProjects ep ON e.EmployeeId = ep.EmployeeId
                JOIN Projects p ON ep.ProjectId = p.ProjectId";
            
            var employeeProjects = connection.Query<EmployeeProjectDto>(query).ToList();

            Console.WriteLine("\nEmployees and their assigned projects (using Dapper):");
            foreach (var item in employeeProjects)
            {
                Console.WriteLine($"Employee: {item.EmployeeName}, Project: {item.ProjectName}, Deadline: {item.ProjectDeadline}");
            }
        }
    }

    // 3. Task - Use Dapper to execute a stored procedure for calculating employee bonuses
    static void CalculateEmployeeBonusesWithDapper()
    {
        string connectionString = "Server=localhost;Database=TrackingDB;Trusted_Connection=True;TrustServerCertificate=True;";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Assume the stored procedure 'CalculateBonus' exists in the database
            var bonuses = connection.Query<EmployeeBonusDto>("CalculateBonus", commandType: System.Data.CommandType.StoredProcedure).ToList();

            Console.WriteLine("\nEmployee Bonuses (calculated using Dapper):");
            foreach (var bonus in bonuses)
            {
                Console.WriteLine($"Employee: {bonus.EmployeeName}, Bonus: {bonus.BonusAmount}");
            }
        }
    }

    // 4. Task - Compare EF Core and Dapper for financial reports
    static void CompareEFCoreVsDapperForFinancialReports(ApplicationDbContext context)
    {
        // Using Entity Framework Core
        var departmentSalariesEFCore = context.Departments
            .Select(d => new
            {
                Department = d.Name,
                TotalSalary = d.Employees.Sum(e => e.Salary)
            })
            .ToList();

        Console.WriteLine("\nDepartment Salaries (using Entity Framework Core):");
        foreach (var item in departmentSalariesEFCore)
        {
            Console.WriteLine($"Department: {item.Department}, Total Salary: {item.TotalSalary}");
        }

        // Using Dapper
        string connectionString = "Server=localhost;Database=TrackingDB;Trusted_Connection=True;TrustServerCertificate=True;";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = @"
                SELECT d.Name AS DepartmentName, SUM(e.Salary) AS TotalSalary
                FROM Departments d
                JOIN Employees e ON d.DepartmentId = e.DepartmentId
                GROUP BY d.Name";
            
            var departmentSalariesDapper = connection.Query<DepartmentSalaryDto>(query).ToList();

            Console.WriteLine("\nDepartment Salaries (using Dapper):");
            foreach (var item in departmentSalariesDapper)
            {
                Console.WriteLine($"Department: {item.DepartmentName}, Total Salary: {item.TotalSalary}");
            }
        }
    }

    // Add sample data for testing (optional)
    static void AddSampleData(ApplicationDbContext context)
    {
        if (!context.Departments.Any())
        {
            var department = new Department { Name = "IT" };
            context.Departments.Add(department);
            context.SaveChanges();

            var employee = new Employee { Name = "John Doe", DepartmentId = department.DepartmentId, Salary = 5000 };
            context.Employees.Add(employee);
            context.SaveChanges();

            var project1 = new Project { Name = "Project A", Deadline = DateTime.Now.AddMonths(3) };
            var project2 = new Project { Name = "Project B", Deadline = DateTime.Now.AddMonths(6) };

            context.Projects.AddRange(project1, project2);
            context.SaveChanges();

            context.EmployeeProjects.AddRange(
                new EmployeeProject { EmployeeId = employee.EmployeeId, ProjectId = project1.ProjectId },
                new EmployeeProject { EmployeeId = employee.EmployeeId, ProjectId = project2.ProjectId }
            );
            context.SaveChanges();
        }
    }
}

// DTOs for Dapper queries
public class EmployeeProjectDto
{
    public string EmployeeName { get; set; }
    public string ProjectName { get; set; }
    public DateTime ProjectDeadline { get; set; }
}

public class EmployeeBonusDto
{
    public string EmployeeName { get; set; }
    public decimal BonusAmount { get; set; }
}

public class DepartmentSalaryDto
{
    public string DepartmentName { get; set; }
    public decimal TotalSalary { get; set; }
}
