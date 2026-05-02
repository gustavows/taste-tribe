using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using tastetribe.Models;
using tastetribe.Data;

namespace tastetribe.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Home Page
        public IActionResult Index()
        {
            return View();
        }

        // Data Visualization Page
        public IActionResult Data()
        {
            return View();
        }

        // About Page
        public IActionResult About()
        {
            return View();
        }

        // Find Restaurants Page
        public IActionResult FindRestaurants()
        {
            return View();
        }

        // CRUD Pages for Review entity
        // GET: /Home/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName");
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name");
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name");
            return View();
        }

        // POST: /Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Read));
            }

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName", review.UserId);
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name", review.RestaurantId);
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name", review.DishId);
            return View(review);
        }

        // GET: /Home/Read
        public async Task<IActionResult> Read()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .ToListAsync();

            return View(reviews);
        }

        // GET: /Home/Update/5
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews.FindAsync(id.Value);
            if (review == null) return NotFound();

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName", review.UserId);
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name", review.RestaurantId);
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name", review.DishId);

            return View(review);
        }

        // POST: /Home/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Review review)
        {
            if (id != review.ReviewId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Reviews.AnyAsync(r => r.ReviewId == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Read));
            }

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName", review.UserId);
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name", review.RestaurantId);
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name", review.DishId);
            return View(review);
        }

        // GET: /Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .FirstOrDefaultAsync(r => r.ReviewId == id.Value);

            if (review == null) return NotFound();

            return View(review);
        }

        // POST: /Home/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Read));
        }

        // Default
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}