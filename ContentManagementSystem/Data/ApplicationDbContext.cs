using Microsoft.EntityFrameworkCore;
using ContentManagementSystem.Models.User;
using ContentManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ContentManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
    }
}