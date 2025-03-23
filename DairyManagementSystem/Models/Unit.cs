using System;
using System.Collections.Generic;

namespace DairyManagementSystem.Models
{
    public partial class Unit
    {
        public Unit()
        {
            Products = new HashSet<Product>();
        }

        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
