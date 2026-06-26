namespace StackOverflowLite.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Question? Question { get; set; }
}
