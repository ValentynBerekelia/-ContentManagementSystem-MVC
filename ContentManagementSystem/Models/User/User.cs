using Microsoft.AspNetCore.Identity;

namespace ContentManagementSystem.Models.User
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Post> Post { get; set; } = new List<Post>();
    }
}
