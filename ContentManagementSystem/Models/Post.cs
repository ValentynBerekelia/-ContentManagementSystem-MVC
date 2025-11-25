using ContentManagementSystem.Models.User;
namespace ContentManagementSystem.Models.User

{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Comment> Coments { get; set; } = new List<Comment>();
    }
}
