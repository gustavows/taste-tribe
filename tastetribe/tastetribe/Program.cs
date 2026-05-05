using Microsoft.EntityFrameworkCore;
using tastetribe.Data;
using tastetribe.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsDevelopment())
{
    // Use in-memory DB for local development so seeded data is available even if Azure is unreachable
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("dev"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed development data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var env = services.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            var context = services.GetRequiredService<AppDbContext>();

            if (!context.Users.Any())
            {
                var u1 = new User { FirstName = "Alex", LastName = "Smith" };
                var u2 = new User { FirstName = "Maria", LastName = "Garcia" };
                context.Users.AddRange(u1, u2);

                var r1 = new Restaurant { Name = "Chipotle", CuisineType = "Fast Casual" };
                var r2 = new Restaurant { Name = "Bento Cafe", CuisineType = "Japanese" };
                context.Restaurants.AddRange(r1, r2);
                context.SaveChanges();

                var d1 = new Dish { Name = "Burrito", RestaurantId = r1.RestaurantId };
                var d2 = new Dish { Name = "Sushi Roll", RestaurantId = r2.RestaurantId };
                context.Dishes.AddRange(d1, d2);
                context.SaveChanges();

                var rev1 = new Review { UserId = u1.UserId, RestaurantId = r1.RestaurantId, DishId = d1.DishId, Rating = 4, ReviewText = "Great burrito." };
                var rev2 = new Review { UserId = u2.UserId, RestaurantId = r2.RestaurantId, DishId = d2.DishId, Rating = 5, ReviewText = "Excellent sushi." };
                context.Reviews.AddRange(rev1, rev2);
                context.SaveChanges();
                logger.LogInformation("Seeded development data.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();