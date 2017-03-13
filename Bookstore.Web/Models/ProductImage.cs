using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyStore.Web.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Display(Name = "File")]
        [StringLength(100, ErrorMessage = "File name is too long")]
        [Index(IsUnique = true)]
        public string FileName { get; set; }

        public virtual ICollection<ProductImageMapping> ProdcutImageMappings { get; set; }
    }
}
