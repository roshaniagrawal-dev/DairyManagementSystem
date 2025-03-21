using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class Client
    {
        public Client()
        {
            MapClientProducts = new HashSet<MapClientProduct>();
            Products = new HashSet<Product>();
        }

        public int ClientId { get; set; }
        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = null!;
        [Display(Name = "Client Code")]
        public string ClientCode { get; set; } = null!;
        [Display(Name = "Client Address")]
        [MaxLength(40, ErrorMessage = "Maximum length is 40 characters")]
        public string ClientAddress { get; set; } = null!;
        [Display(Name = "Client Mobile No.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string MobileNo { get; set; } = null!;
        [NotMapped]
        public String? IsActiveStr { get; set; }
        public bool IsActive { get; set; }
        public DateTime ClientPostingDate { get; set; }

        public virtual ICollection<MapClientProduct> MapClientProducts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
