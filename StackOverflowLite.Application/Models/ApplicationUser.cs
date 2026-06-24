using Microsoft.AspNetCore.Identity;

namespace StackOverflowLite.Application.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
