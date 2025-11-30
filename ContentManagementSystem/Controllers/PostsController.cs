using ContentManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ContentManagementSystem.Models.User;
namespace ContentManagementSystem.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _context.Posts.Include(u => u.User).ToListAsync();
            return View(result);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var result = await _context.Posts
                .Include(u => u.User)
                .Include(p => p.Coments)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (result == null)
                return NotFound();
            return View(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            var userEmail = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser != null)
            {
                post.UserId = currentUser.UserId;
                ModelState.Remove("User");
                ModelState.Remove("UserId");

                if (ModelState.IsValid)
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(post);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var post = await _context.Posts
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
                return NotFound();
            var userEmail = User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            return View(post);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            var userEmail = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (post != null && currentUser != null && post.UserId == currentUser.UserId)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction(nameof(Details), new { id = postId });
            }
            var userEmail = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser != null)
            {
                var comment = new Comment
                {
                    PostId = postId,
                    UserId = currentUser.UserId,
                    Text = text,
                };
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}
