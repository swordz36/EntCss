using System.ComponentModel.DataAnnotations;

namespace BabyStore.Web.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        [Display(Name = "Product Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}
