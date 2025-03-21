using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class Company
    {
        public Company()
        {
            Products = new HashSet<Product>();
        }

        public int CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = null!;

        [Display(Name = "Company PostingDate")]
        public DateTime CompanyPostingDate { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public String? IsActiveStr { get; set; }

        [NotMapped]
        public string? isOperational { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Display(Name = "Company Address")]
        [MaxLength(40, ErrorMessage = "Maximum length is 40 characters")]
        public string CompanyAddress { get; set; } = null!;

        [Display(Name = "State Code")]
        public int StateCode { get; set; }

        [Display(Name = "Bank Details")]
        public string BankDetail { get; set; } = null!;
        public string Code { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
