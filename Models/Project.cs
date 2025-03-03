using System.Collections.Generic;

namespace ProjectTrackingSystem.Models
{
    public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public DateTime Deadline { get; set; }
    public ICollection<EmployeeProject> EmployeeProjects { get; set; }
}

}