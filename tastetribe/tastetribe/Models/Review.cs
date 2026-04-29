namespace tastetribe.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public int DishId { get; set; }

        public User? User { get; set; }
        public Restaurant? Restaurant { get; set; }
        public Dish? Dish { get; set; }
    }
}
