using DairyManagementSystem.Models;
using System.Data;

namespace DairyManagementSystem.ViewModel
{
    public class SalesMemoDetails
    {
        public SalesMemo salesMemo { get; set; }
        public List<Product> ListProduct { get; set; }

        public List<SalesTableData> SalesTableData { get; set; }
        public List<MapClientProduct> mapClientProducts { get; set; }
    }
    public class ProudctRate
    {
        public List<string> ProductRate { get; set; }
    }
    public class CombinedDataModel
    {
        public SalesMemo FormData { get; set; }
        public List<List<string>> ConvertedData { get; set; }
    }
    public class SalesTableData
    {
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public List<MapClientProduct> ListQR { get; set; }
        public decimal Total { get; set; }
    }
}
