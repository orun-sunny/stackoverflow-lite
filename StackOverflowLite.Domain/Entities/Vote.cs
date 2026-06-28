namespace StackOverflowLite.Domain.Entities;

public enum VoteType
{
    UpVote = 1,
    DownVote = 2
}

public class Vote
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? QuestionId { get; set; }
    public Guid? AnswerId { get; set; }
    public VoteType VoteType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
