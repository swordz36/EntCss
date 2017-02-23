using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bookstore.Web.Models;
using Product = BabyStore.Web.Models.Product;

namespace Bookstore.Web.ViewModels
{
    public class ProductIndexViewModel
    {
        public IQueryable<Product> Products { get; set; }
        public string Search { get; set; }
        public IEnumerable<CategoryWithCount> CatWithCount { get; set; }
        public string Category { get; set; }

        public IEnumerable<SelectListItem> CatFilterItems
        {
            get
            {
                var allCategories = CatWithCount.Select(x => new SelectListItem
                {
                    Value = x.CategoryName,
                    Text = x.CategoryNameWithCount
                });

                return allCategories;
            } 
        }
    }
}
