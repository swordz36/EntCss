using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyStore.Web.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Display(Name="File")]
        public string FileName { get; set; }
    }
}
