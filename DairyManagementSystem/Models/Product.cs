using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class Product
    {
        public Product()
        {
            MapClientProducts = new HashSet<MapClientProduct>();
        }

        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;
        [Display(Name = "Selling Price")]
        public int Price { get; set; }
        [Display(Name = "Product PostingDate")]
        public DateTime ProductPostingDate { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [NotMapped]
        public String? IsActiveStr { get; set; }

        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Purchase Rate")]
        public int PurchaseRate { get; set; }
        [Display(Name = "HSN/SAC Code")]
        public string Hsnsaccode { get; set; } = null!;
        [Display(Name = "IGST Rate")]
        public int Igstrate { get; set; }
        [Display(Name = "CGST Rate")]
        public int Cgstrate { get; set; }
        [Display(Name = "SGST Rate")]
        public int Sgstrate { get; set; }
        [Display(Name = "Type")]
        public int TypeId { get; set; }
        [Display(Name = "Unit")]
        public int UnitId { get; set; }
        public string Code { get; set; } = null!;
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Client Client { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual ProductType Type { get; set; } = null!;
        public virtual Unit Unit { get; set; } = null!;
        public virtual Vendor Vendor { get; set; } = null!;
        public virtual ICollection<MapClientProduct> MapClientProducts { get; set; }
    }
}
