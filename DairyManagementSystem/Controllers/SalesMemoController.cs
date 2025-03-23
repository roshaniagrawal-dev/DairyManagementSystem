using Microsoft.AspNetCore.Http;
using DairyManagementSystem.Models;
using DairyManagementSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;

namespace DairyManagementSystem.Controllers
{
    public class SalesMemoController : Controller
    {
        private readonly DMSContext _db;

        public SalesMemoController(DMSContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult GetList()
        {
            try
            {
                var catList = _db.SalesMemos.Where(x => x.IsActive == true).ToList();
                return Json(new { data = catList });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }

        // GET: SalesMemoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SalesMemoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SalesMemoController/Create
        public ActionResult Create()
        {
            try
            {
                var LastClient = _db.SalesMemos.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.SalesMemoId).FirstOrDefault();
                int newNumber = (LastClient != null) ? int.Parse(LastClient.Code.Split('_')[1]) + 1 : 1;
                string newCode = "SM_" + newNumber.ToString("D3");
                ViewBag.Code = newCode;
                SalesMemoDetails sales = new SalesMemoDetails();
                sales.ListProduct = _db.Products.Where(x => x.IsActive == true).ToList();

                return View(sales);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }

        [HttpGet]
        public ActionResult GetListCreate()
        {
            try
            {
                SalesMemoDetails sales = new SalesMemoDetails();
                sales.ListProduct = _db.Products.Where(x => x.IsActive == true).ToList();
                sales.SalesTableData = (from a in _db.Clients
                                        select new SalesTableData
                                        {
                                            ClientName = a.ClientName,
                                            ClientId = a.ClientId,
                                            ListQR = _db.MapClientProducts.Where(x=>x.ClientId==a.ClientId).ToList()
                                        }).ToList();
                return Json(new { data = sales.SalesTableData });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }

        [HttpGet]
        public ActionResult GetListEdit()
        {
            try
            {
                SalesMemoDetails sales = new SalesMemoDetails();
                var SalesMemoId = Convert.ToInt32(TempData["SalesMemoIdEdit"] ?? 0);
                sales.SalesTableData = (from a in _db.Clients
                                        select new SalesTableData
                                        {
                                            ClientName = a.ClientName,
                                            ClientId = a.ClientId,
                                            ListQR = _db.MapClientProducts.Where(x => x.ClientId == a.ClientId && x.SalesMemoId== SalesMemoId).ToList()
                                        }).ToList();
                return Json(new { data = sales.SalesTableData });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }

        // POST: SalesMemoController/Create
        [HttpPost]
        public ActionResult CreateTable([FromBody] List<List<string>> data)
        {
            try
            {
                if (data == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                }
                else if (data.Count >= 4)
                {
                    var createformdata = data;
                    var SalesMemoId = TempData["SalesMemoId"] ?? 0.ToString();
                    var ListC = _db.Clients.Where(x => x.IsActive == true).ToList();
                    var RateList = createformdata[createformdata.Count - 2];
                    for (var i = 0; i < createformdata.Count-3; i++)
                    {
                        var itemListClient = createformdata[i];
                        if (itemListClient != null)
                        {
                            int ClientId = 0;
                            var ListP = _db.Products.Where(x => x.IsActive == true).ToList();
                            if (ListC.Count > i)
                            {
                                ClientId = ListC[i].ClientId;
                            }
                            for (var j = 0; j < itemListClient.Count; j++)
                            {
                                MapClientProduct mapClientProduct = new MapClientProduct();
                                mapClientProduct.Quantity = Convert.ToDecimal(itemListClient[j]);
                                mapClientProduct.ProductId = ListP[j].ProductId;
                                mapClientProduct.Rate = Convert.ToDecimal(RateList[j]);
                                mapClientProduct.ClientId = ClientId;
                                mapClientProduct.SalesMemoId = Convert.ToInt32(SalesMemoId);
                                mapClientProduct.IsActive = true;
                                _db.Add(mapClientProduct);
                                _db.SaveChanges();

                            }
                        }
                    }
                    TempData["Create"] = "Data for Sales is successfully submitted.";
                }
                else
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                }
                return Json(new { data = "true" });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }
        
        [HttpPost]
        public ActionResult CreateTableUpdate([FromBody] List<List<string>> data)
        {
            try
            {
                if (data == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                }
                else if (data.Count >= 4)
                {
                    var createformdata = data;
                    var SalesMemoId = TempData["SalesMemoId"] ?? 0.ToString();
                    var ListC = _db.Clients.Where(x => x.IsActive == true).ToList();
                    var RateList = createformdata[createformdata.Count - 2];
                    for (var i = 0; i < createformdata.Count-3; i++)
                    {
                        var itemListClient = createformdata[i];
                        if (itemListClient != null)
                        {
                            int ClientId = 0;
                            var ListP = _db.Products.Where(x => x.IsActive == true).ToList();
                            if (ListC.Count > i)
                            {
                                ClientId = ListC[i].ClientId;
                            }
                            for (var j = 0; j < itemListClient.Count; j++)
                            {
                                MapClientProduct mapClientProduct = _db.MapClientProducts.Where(x => x.ProductId == ListP[j].ProductId && x.ClientId == ClientId && x.SalesMemoId == Convert.ToInt32(SalesMemoId)).FirstOrDefault();
                                if (mapClientProduct != null)
                                {
                                    mapClientProduct.Quantity = Convert.ToDecimal(itemListClient[j]);
                                    mapClientProduct.Rate = Convert.ToDecimal(RateList[j]);
                                    mapClientProduct.IsActive = true;
                                    _db.Update(mapClientProduct);
                                }
                                else
                                {
                                    MapClientProduct mapClientProductAdd = new MapClientProduct();
                                    mapClientProductAdd.Quantity = Convert.ToDecimal(itemListClient[j]);
                                    mapClientProductAdd.ProductId = ListP[j].ProductId;
                                    mapClientProductAdd.Rate = Convert.ToDecimal(RateList[j]);
                                    mapClientProductAdd.ClientId = ClientId;
                                    mapClientProductAdd.SalesMemoId = Convert.ToInt32(SalesMemoId);
                                    mapClientProductAdd.IsActive = true;
                                    _db.Add(mapClientProductAdd);
                                }
                                _db.SaveChanges();

                            }
                        }
                    }
                    TempData["Edit"] = "Data for Sales is successfully updated.";
                }
                else
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                }
                return Json(new { data = "true" });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }
        
        [HttpPost]
        public ActionResult Create([FromBody] string s)
        {
            try
            {
                SalesMemo sales = new SalesMemo();
                var queryParams = System.Web.HttpUtility.ParseQueryString(s);
                bool isActive;
                if (bool.TryParse(queryParams["IsActive"], out isActive))
                {
                    sales.IsActive = isActive;
                }
                sales.Code = queryParams["Code"];
                sales.SalesDate = DateTime.Parse(queryParams["SalesDate"]);

                if (sales == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    return Json(new { data = "false" });

                }
                else
                {
                    ModelState.Remove("PostingDate");
                    ModelState.Remove("SalesMemoId");
                    ModelState.Remove("CreatedBy");
                    if (ModelState.IsValid)
                    {
                        var date = DateTime.Now;
                        sales.PostingDate = date;
                        sales.CreatedBy = 1;
                        _db.Add(sales);
                        _db.SaveChanges();
                       SalesMemo id  = _db.SalesMemos.Where(x => x.PostingDate == date).FirstOrDefault();
                      TempData["SalesMemoId"] = id.SalesMemoId;
                        
                        
                    }
                    else
                    {
                        TempData["Error"] = "Data for Sale : " + sales.Code + " is invalid.";
                        return Json(new { data = "false" });


                    }
                }
                return Json(new { data = "true" });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }

        // GET: SalesMemoController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                SalesMemoDetails sales = new SalesMemoDetails();
                sales.salesMemo = _db.SalesMemos.Where(x=>x.SalesMemoId==id).First();
                sales.ListProduct = _db.Products.Where(x => x.IsActive == true).ToList();
                sales.mapClientProducts = _db.MapClientProducts.Where(x => x.IsActive == true && x.SalesMemoId == id).ToList();
                TempData["SalesMemoIdEdit"] = id;
                return View(sales);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // POST: SalesMemoController/Edit/5
        [HttpPost]
        public ActionResult Edit([FromBody] string s)
        {
            try
            {
                SalesMemo sales = new SalesMemo();
                var queryParams = System.Web.HttpUtility.ParseQueryString(s);
                bool isActive;
                if (bool.TryParse(queryParams["IsActive"], out isActive))
                {
                    sales.IsActive = isActive;
                }
                sales.Code = queryParams["Code"];
                sales.SalesMemoId = Convert.ToInt32(queryParams["SalesMemoId"]);
                sales.PostingDate = DateTime.Parse(queryParams["PostingDate"]);
                sales.SalesDate = DateTime.Parse(queryParams["SalesDate"]);
                sales.CreatedBy = Convert.ToInt32(queryParams["CreatedBy"]);

                if (sales == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    return Json(new { data = "false" });

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _db.Update(sales);
                        _db.SaveChanges();
                        SalesMemo id = _db.SalesMemos.Where(x => x.SalesMemoId == sales.SalesMemoId).FirstOrDefault();
                        TempData["SalesMemoId"] = id.SalesMemoId;
                    }
                    else
                    {
                        TempData["Error"] = "Data for Sale : " + sales.Code + " is invalid.";
                        return Json(new { data = "false" });


                    }
                }
                return Json(new { data = "true" });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { data = "false" });
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                var mapClientProductList = _db.MapClientProducts.Where(x =>  x.SalesMemoId == id).ToList();
                if (id == 0 || id == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";

                }
                else
                {
                    foreach(var item in mapClientProductList)
                    {
                        MapClientProduct mp = _db.MapClientProducts.Find(item.MapClientProductId);
                        mp.IsActive = false;
                        _db.SaveChanges();
                    }
                    SalesMemo ven = _db.SalesMemos.Find(id);
                    ven.IsActive = false;
                    _db.SaveChanges();
                    TempData["Delete"] = "Data for Vendor : " + ven.Code + " is deleted sucessfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult SearchDetails(SalesMemo ven)
        {
            var sales = _db.SalesMemos.ToList();
            if (ven == null)
            {
                return NotFound();
            }
            else
            {
                DateTime defualtdate = Convert.ToDateTime("01-01-0001 00:00:00");
                if (ven.SalesDate != defualtdate)
                {
                    sales = sales.Where(x => x.SalesDate==ven.SalesDate).ToList();
                }
                if (ven.Code != null)
                {
                    sales = sales.Where(x => x.Code.ToUpper().Contains(ven.Code.ToUpper())).ToList();
                }
                if (ven.IsActiveStr != null)
                {
                    bool isative = Convert.ToBoolean(ven.IsActiveStr);
                    sales = sales.Where(x => x.IsActive == isative).ToList();
                }

            }


            return Json(new { data = sales });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var VenList = _db.SalesMemos.ToList();

                var filename = "SalesMemo.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Code"),
                    new DataColumn("Sales Date"),
                    new DataColumn("Sales Memo Posting Date")
                });

                foreach (var a in VenList)
                {
                    dt.Rows.Add(a.Code, a.PostingDate, a.Code);
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var sheet1 = wb.AddWorksheet(dt, "Sheet1");
                    sheet1.Row(1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.Blue);
                    sheet1.Row(1).CellsUsed().Style.Font.Bold = true;
                    sheet1.Row(1).CellsUsed().Style.Font.FontColor = XLColor.White;

                    sheet1.Columns().AdjustToContents();
                    sheet1.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    sheet1.Rows(2, VenList.Count() + 1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.SkyBlue);

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
                Console.WriteLine(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ExportExcelTable([FromBody] List<List<string>> data)
        {
            try
            {
                //columns
                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.Add(new DataColumn("Clients"));
                foreach (var item in _db.Products)
                {
                    dt.Columns.Add(new DataColumn(item.ProductName));
                }
                dt.Columns.Add(new DataColumn("Total"));
                
                //rows
                var createformdata = data;
                var ListC = _db.Clients.Where(x => x.IsActive == true).ToList();
                var ListP = _db.Products.Where(x => x.IsActive == true).ToList();
                var rates = createformdata[createformdata.Count - 2];
                for (var i = 0; i < createformdata.Count -3; i++)
                {
                    var itemListClient = createformdata[i].ToList(); 
                    var total = 0;
                    foreach(var item in itemListClient)
                    {
                        total += Convert.ToInt32(item);
                    }
                    DataRow newRow = dt.NewRow();
                    newRow["Clients"] = ListC[i].ClientName;
                    for (var p = 0; p < ListP.Count; p++)
                    {
                        newRow[ListP[p].ProductName] = itemListClient[p];      
                    }
                    newRow["Total"] = total; 
                    dt.Rows.Add(newRow); 
                }

                //rate row
                var totalR = 0;
                foreach (var item in rates)
                {
                    totalR += Convert.ToInt32(item);
                }
                DataRow newRowR = dt.NewRow();
                newRowR["Clients"] = "Rate";
                for (var item=0; item <rates.Count;item++)
                {
                    newRowR[ListP[item].ProductName] = rates[item];
                }
                newRowR["Total"] = totalR;
                dt.Rows.Add(newRowR);
                
                //total and amount row
                
                DataRow newRowT = dt.NewRow();
                newRowT["Clients"] = "Total";
                DataRow newRowQ = dt.NewRow();
                newRowQ["Clients"] = "Amount";
                var ttlCl = 0;
                var multi = 1;
                for (var p = 0; p < ListP.Count; p++)
                {
                    for (var i = 0; i < ListC.Count; i++)
                    {
                        var item = createformdata[i][p];

                        ttlCl += Convert.ToInt32(item); 
                    }
                    multi = Convert.ToInt32(rates[p]) * ttlCl;
                    newRowT[ListP[p].ProductName] = ttlCl;
                    newRowQ[ListP[p].ProductName] = multi;
                    ttlCl = 0;
                    multi = 1;
                }
                newRowT["Total"] = "0";
                dt.Rows.Add(newRowT); 
                newRowQ["Total"] = "0";
                dt.Rows.Add(newRowQ);

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var sheet1 = wb.AddWorksheet(dt, "Sheet1");
                    sheet1.Row(1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.Blue);
                    sheet1.Row(1).CellsUsed().Style.Font.Bold = true;
                    sheet1.Row(1).CellsUsed().Style.Font.FontColor = XLColor.White;

                    sheet1.Columns().AdjustToContents();
                    sheet1.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    sheet1.Rows(2, ListC.Count() + 4).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.SkyBlue);

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Json(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}
