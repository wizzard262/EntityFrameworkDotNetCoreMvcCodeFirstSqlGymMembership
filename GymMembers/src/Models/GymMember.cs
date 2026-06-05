namespace GymMembers.Models;

public class GymMember
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime JoinDate { get; set; }

    // Navigation: many-to-many with ClassSession
    public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
}
