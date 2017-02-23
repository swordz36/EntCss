namespace Bookstore.Web.Models
{
    public class CategoryWithCount
    {
        public int ProductCount { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameWithCount => $"{CategoryName} ({ProductCount})";
    }
}