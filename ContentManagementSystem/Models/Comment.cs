using Microsoft.Extensions.Primitives;
using ContentManagementSystem.Models;
namespace ContentManagementSystem.Models.User
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
    }
}
