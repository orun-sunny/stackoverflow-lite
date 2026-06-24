namespace StackOverflowLite.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Reputation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
