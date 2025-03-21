using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DairyManagementSystem.Models
{
    public partial class SalesMemo
    {
        public SalesMemo()
        {
            MapClientProducts = new HashSet<MapClientProduct>();
        }

        public int SalesMemoId { get; set; }
        public string Code { get; set; } = null!;
        public DateTime SalesDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime PostingDate { get; set; }
        [NotMapped]
        public String? IsActiveStr { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<MapClientProduct> MapClientProducts { get; set; }
    }
}
