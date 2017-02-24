using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BabyStore.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$",ErrorMessage ="Invalid Characters Entered")]
    
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}