using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using tastetribe.Models;
using tastetribe.Data;

namespace tastetribe.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Data()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult FindRestaurants()
        {
            return View();
        }

        // GET: /Home/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName");
            ViewBag.UsersList = await _context.Users.ToListAsync();
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name");
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name");
            return View();
        }

        // POST: /Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review? review, string? submit, string? userFirstName, string? userLastName, string? restaurantName, string? dishName)
        {
            _logger.LogDebug("Create POST called. userFirstName={userFirstName}, userLastName={userLastName}, review.UserId={UserId}", userFirstName, userLastName, review?.UserId);
            review ??= new Review();

            if (!string.IsNullOrWhiteSpace(userFirstName))
            {
                var u = new User { FirstName = userFirstName.Trim(), LastName = (userLastName ?? string.Empty).Trim() };
                _context.Users.Add(u);
                await _context.SaveChangesAsync();
                review.UserId = u.UserId;
                _logger.LogInformation("Created new user {UserId} from free-text input.", u.UserId);
            }

            if (!string.IsNullOrWhiteSpace(restaurantName))
            {
                var r = new Restaurant { Name = restaurantName.Trim(), CuisineType = string.Empty };
                _context.Restaurants.Add(r);
                await _context.SaveChangesAsync();
                review.RestaurantId = r.RestaurantId;
                _logger.LogInformation("Created new restaurant {RestaurantId} from free-text input.", r.RestaurantId);
            }

            if (!string.IsNullOrWhiteSpace(dishName))
            {
                if (review.RestaurantId <= 0)
                {
                    var r2 = new Restaurant { Name = "Unknown", CuisineType = string.Empty };
                    _context.Restaurants.Add(r2);
                    await _context.SaveChangesAsync();
                    review.RestaurantId = r2.RestaurantId;
                    _logger.LogInformation("Created placeholder restaurant {RestaurantId} for dish.", r2.RestaurantId);
                }

                var d = new Dish { Name = dishName.Trim(), RestaurantId = review.RestaurantId };
                _context.Dishes.Add(d);
                await _context.SaveChangesAsync();
                review.DishId = d.DishId;
                _logger.LogInformation("Created new dish {DishId} from free-text input.", d.DishId);
            }

            if (review.UserId <= 0)
                ModelState.AddModelError("UserId", "Please provide a user name or select an existing user.");
            if (review.RestaurantId <= 0)
                ModelState.AddModelError("RestaurantId", "Please select a restaurant.");
            if (review.DishId <= 0)
                ModelState.AddModelError("DishId", "Please select a dish.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync();
                    if (string.Equals(submit, "saveAndNew", System.StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction(nameof(Create));
                    return RedirectToAction(nameof(Read));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred saving the review: " + ex.Message);
                }
            }

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName", review.UserId);
            ViewBag.UsersList = await _context.Users.ToListAsync();
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

        // GET: /Home/DebugReviews
        [HttpGet]
        public async Task<IActionResult> DebugReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .ToListAsync();

            return Json(reviews.Select(r => new { r.ReviewId, User = r.User?.FirstName, Restaurant = r.Restaurant?.Name, Dish = r.Dish?.Name, r.Rating }));
        }

        // GET: /Home/Update/5
        public async Task<IActionResult> Update(int? id)
        {
            _logger.LogDebug("Update GET called with id={id}", id);
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .FirstOrDefaultAsync(r => r.ReviewId == id.Value);

            if (review == null)
            {
                _logger.LogWarning("Update: review not found for id={id}", id);
                return NotFound();
            }

            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FirstName", review.UserId);
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "RestaurantId", "Name", review.RestaurantId);
            ViewBag.Dishes = new SelectList(await _context.Dishes.ToListAsync(), "DishId", "Name", review.DishId);
            return View(review);
        }

        // POST: /Home/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Review review, string? userFirstName, string? userLastName, string? restaurantName, string? dishName)
        {
            if (id != review.ReviewId) return BadRequest();

            if (!string.IsNullOrWhiteSpace(userFirstName))
            {
                var u = new User { FirstName = userFirstName.Trim(), LastName = (userLastName ?? string.Empty).Trim() };
                _context.Users.Add(u);
                await _context.SaveChangesAsync();
                review.UserId = u.UserId;
                _logger.LogInformation("Created new user {UserId} during update.", u.UserId);
            }

            if (!string.IsNullOrWhiteSpace(restaurantName))
            {
                var existingR = await _context.Restaurants.FirstOrDefaultAsync(r => r.Name.ToLower() == restaurantName.Trim().ToLower());
                if (existingR != null)
                {
                    review.RestaurantId = existingR.RestaurantId;
                }
                else
                {
                    var r = new Restaurant { Name = restaurantName.Trim(), CuisineType = string.Empty };
                    _context.Restaurants.Add(r);
                    await _context.SaveChangesAsync();
                    review.RestaurantId = r.RestaurantId;
                    _logger.LogInformation("Created new restaurant {RestaurantId} during update.", r.RestaurantId);
                }
            }

            if (!string.IsNullOrWhiteSpace(dishName))
            {
                var existingD = await _context.Dishes.FirstOrDefaultAsync(d => d.Name.ToLower() == dishName.Trim().ToLower() && d.RestaurantId == review.RestaurantId);
                if (existingD != null)
                {
                    review.DishId = existingD.DishId;
                }
                else if (review.RestaurantId > 0)
                {
                    var d = new Dish { Name = dishName.Trim(), RestaurantId = review.RestaurantId };
                    _context.Dishes.Add(d);
                    await _context.SaveChangesAsync();
                    review.DishId = d.DishId;
                    _logger.LogInformation("Created new dish {DishId} during update.", d.DishId);
                }
            }

            if (review.UserId <= 0)
                ModelState.AddModelError("UserId", "Please provide a user name or select an existing user.");
            if (review.RestaurantId <= 0)
                ModelState.AddModelError("RestaurantId", "Please provide a restaurant name.");
            if (review.DishId <= 0)
                ModelState.AddModelError("DishId", "Please provide a dish name.");

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
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred updating the review: " + ex.Message);
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
            _logger.LogDebug("Delete GET called with id={id}", id);
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .FirstOrDefaultAsync(r => r.ReviewId == id.Value);

            if (review == null)
            {
                _logger.LogWarning("Delete: review not found for id={id}", id);
                return NotFound();
            }

            return View(review);
        }

        // POST: /Home/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                try
                {
                    _context.Reviews.Remove(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    TempData["Error"] = "Unable to delete review: " + ex.Message;
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }

            return RedirectToAction(nameof(Read));
        }
        // GET: /Home/UpdateList
        public async Task<IActionResult> UpdateList()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .ToListAsync();
            return View(reviews);
        }

        // GET: /Home/DeleteList
        public async Task<IActionResult> DeleteList()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Include(r => r.Dish)
                .ToListAsync();
            return View(reviews);
        }

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