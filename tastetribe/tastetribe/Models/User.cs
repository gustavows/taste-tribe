using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace tastetribe.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}