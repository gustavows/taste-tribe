using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace tastetribe.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
