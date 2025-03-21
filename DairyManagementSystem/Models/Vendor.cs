using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Products = new HashSet<Product>();
        }

        public int VendorId { get; set; }
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; } = null!;
        [Display(Name = "Vendor PostingDate")]
        public DateTime VendorPostingDate { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [NotMapped]
        public String? IsActiveStr { get; set; }

        [NotMapped]
        public string? isOperational { get; set; }
        public DateTime? EndDate { get; set; }
        [MaxLength(40, ErrorMessage = "Maximum length is 40 characters")]
        public string Address { get; set; } = null!;
        [Display(Name = "State Code")]
        public int StateCode { get; set; }
        [Display(Name = "Bank Details")]
        public string BankDetail { get; set; } = null!;
        public bool IsActive { get; set; }
        public string Code { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
