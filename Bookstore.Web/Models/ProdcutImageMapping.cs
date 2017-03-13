using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyStore.Web.Models
{
    public class ProductImageMapping
    {
        public int Id { get; set; }
        public int ImageNumber { get; set; }
        public int ProductId { get; set; }
        public int ProductImageId { get; set; }

        public virtual Product Product { get; set; }
        public virtual ProductImage ProductImage { get; set; }

    }
}
