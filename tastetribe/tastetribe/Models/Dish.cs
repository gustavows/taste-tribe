using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace tastetribe.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}