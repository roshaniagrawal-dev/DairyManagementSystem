using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = null!;
        public string Code { get; set; } = null!;
        [Display(Name = "Category PostingDate")]
        public DateTime CategoryPostingDate { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public String? IsActiveStr { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
