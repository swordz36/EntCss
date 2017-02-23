using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Web.Models
{
    [MetadataType(typeof(ProductMetaData))]
    public partial class Product
    {
    }

    public class ProductMetaData
    {
        [Display(Name = "Product Name")]
        public string Name;
    }
}
