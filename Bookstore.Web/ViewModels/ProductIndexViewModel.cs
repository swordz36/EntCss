using BabyStore.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace BabyStore.Web.ViewModels
{
    public class ProductIndexViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public string Search { get; set; }
        public IEnumerable<CategoryWithCount> CatWithCount { get; set; }
        public string Category { get; set; }
        public string SortBy { get; set; }
        public Dictionary<string, string> Sorts { get; set; }

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
