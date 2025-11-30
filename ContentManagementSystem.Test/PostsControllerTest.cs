using ContentManagementSystem.Controllers;
using ContentManagementSystem.Data;
using ContentManagementSystem.Models;
using ContentManagementSystem.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ContentManagementSystem.Tests
{
    public class PostsControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        private void MockUser(Controller controller, string email)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, email),
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsView_WithPosts()
        {
            var context = GetDatabaseContext();
            var user = new User { UserId = 1, Name = "Test", Email = "test@mail.com", Password = "123" };
            context.Users.Add(user);

            context.Posts.Add(new Post { Title = "Test Post", Text = "Content", UserId = 1 });
            await context.SaveChangesAsync();

            var controller = new PostsController(context);

            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_AddsPost_AndRedirects_WhenUserExists()
        {
            var context = GetDatabaseContext();
            var user = new User { UserId = 1, Email = "author@mail.com", Name = "Author", Password = "123" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new PostsController(context);
            MockUser(controller, "author@mail.com");

            var newPost = new Post { Title = "New Post", Text = "Some text" };
            var result = await controller.Create(newPost);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var savedPost = context.Posts.FirstOrDefault();
            Assert.NotNull(savedPost);
            Assert.Equal("New Post", savedPost.Title);
            Assert.Equal(1, savedPost.UserId);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesPost_IfUserIsOwner()
        {
            var context = GetDatabaseContext();
            var user = new User { UserId = 10, Email = "owner@mail.com", Name = "O", Password = "123" };
            var post = new Post { PostId = 5, Title = "To Delete", Text = "...", UserId = 10 };

            context.Users.Add(user);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new PostsController(context);
            MockUser(controller, "owner@mail.com");

            var result = await controller.DeleteConfirmed(5);

            Assert.Equal(0, context.Posts.Count());
        }

        [Fact]
        public async Task DeleteConfirmed_DoesNotRemove_IfUserIsNotOwner()
        {
            var context = GetDatabaseContext();
            var owner = new User { UserId = 1, Email = "owner@mail.com", Name = "A", Password = "1" };
            var hacker = new User { UserId = 2, Email = "hacker@mail.com", Name = "B", Password = "1" };

            var post = new Post { PostId = 1, Title = "Safe Post", Text = "...", UserId = 1 };

            context.Users.AddRange(owner, hacker);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new PostsController(context);
            MockUser(controller, "hacker@mail.com");
            await controller.DeleteConfirmed(1);

            Assert.Equal(1, context.Posts.Count());
        }

        [Fact]
        public async Task AddComment_AddsComment_WhenDataIsValid()
        {
            var context = GetDatabaseContext();
            var user = new User { UserId = 1, Email = "commenter@mail.com", Name = "C", Password = "1" };
            var post = new Post { PostId = 100, Title = "Post", Text = "...", UserId = 1 };

            context.Users.Add(user);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            var controller = new PostsController(context);
            MockUser(controller, "commenter@mail.com");

            
            var result = await controller.AddComment(100, "Nice post!");

            var comment = context.Comments.FirstOrDefault();
            Assert.NotNull(comment);
            Assert.Equal("Nice post!", comment.Text);
            Assert.Equal(100, comment.PostId);
            Assert.Equal(1, comment.UserId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(100, redirectResult.RouteValues["id"]);
        }
    }
}