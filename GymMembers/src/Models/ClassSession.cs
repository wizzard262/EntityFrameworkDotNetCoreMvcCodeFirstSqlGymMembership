namespace GymMembers.Models;

public class ClassSession
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public int Capacity { get; set; }

    // Navigation: many-to-many with GymMember
    public ICollection<GymMember> GymMembers { get; set; } = new List<GymMember>();
}
