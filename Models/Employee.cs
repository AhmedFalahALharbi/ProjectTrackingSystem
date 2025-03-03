using System.Collections.Generic;

namespace ProjectTrackingSystem.Models
{
    // Employee class (many-to-one with Department, many-to-many with Project)
   public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public decimal PerformanceRating { get; set; }
        
        // Many-to-one relationship with Department
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        
        // Many-to-many relationship with Project
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}