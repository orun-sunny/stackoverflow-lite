namespace StackOverflowLite.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
