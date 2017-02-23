using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BabyStore.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}