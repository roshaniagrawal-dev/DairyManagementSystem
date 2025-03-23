using System;
using System.Collections.Generic;

namespace DairyManagementSystem.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Products = new HashSet<Product>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
