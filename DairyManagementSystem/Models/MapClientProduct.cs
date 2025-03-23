using System;
using System.Collections.Generic;

namespace DairyManagementSystem.Models
{
    public partial class MapClientProduct
    {
        public int MapClientProductId { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int SalesMemoId { get; set; }
        public bool IsActive { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual SalesMemo SalesMemo { get; set; } = null!;
    }
}
