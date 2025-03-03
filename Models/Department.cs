using System.Collections.Generic;

namespace ProjectTrackingSystem.Models
{
   public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        
        public ICollection<Employee> Employees { get; set; }
    }
}