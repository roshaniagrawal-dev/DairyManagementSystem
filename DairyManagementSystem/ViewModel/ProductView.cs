using System.ComponentModel.DataAnnotations;

namespace DairyManagementSystem
{
    public class ProductView
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        [Required]
        public int Price { get; set; }

        public DateTime ProductPostingDate { get; set; }

        [Required]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string CategoryName { get; set; } = null!; 
        [Required]
        public string VendorName { get; set; } = null!;
        public string Action { get; set; } = null!;
        [Display(Name = "Purchase Rate")]
        [Required]
        public int PurchaseRate { get; set; }
        [Display(Name = "HSN/SAC Code")]
        [Required]
        public string Hsnsaccode { get; set; } = null!;
        [Display(Name = "IGST Rate")]
        [Required]
        public int Igstrate { get; set; }
        [Display(Name = "CGST Rate")]
        [Required]
        public int Cgstrate { get; set; }
        [Display(Name = "SGST Rate")]
        [Required]
        public int Sgstrate { get; set; }
        [Display(Name = "Type")]
        [Required]
        public string TypeName { get; set; }
        [Display(Name = "Unit")]
        [Required]
        public string UnitName { get; set; }
        [Display(Name = "Client")]
        [Required]
        public string ClientName { get; set; }
        public string Code { get; set; } = null!;
    }
}
