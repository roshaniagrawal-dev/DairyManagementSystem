using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DairyManagementSystem.Models;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Data;

namespace DairyManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly DMSContext _db;
       
        public ProductController(DMSContext db)
        {
            _db = db;
        }

        public void DDLload()
        {
            ViewBag.CategoryList = _db.Categories.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName

                }).Distinct();
            ViewBag.CompanyList = _db.Companies.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.CompanyId.ToString(),
                    Text = x.CompanyName

                }); 
            ViewBag.VenList = _db.Vendors.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.VendorId.ToString(),
                    Text = x.VendorName

                });
            ViewBag.TypeList = _db.ProductTypes.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.TypeId.ToString(),
                    Text = x.TypeName

                }); 
            ViewBag.UnitList = _db.Units.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.UnitId.ToString(),
                    Text = x.UnitName

                }); 
            ViewBag.ClientList = _db.Clients.Where(x=>x.IsActive==true).Select(
                x => new SelectListItem
                {
                    Value = x.ClientId.ToString(),
                    Text = x.ClientName

                }); 
        }

        public ActionResult GetDropdownData()
        {
            List<SelectListItem> Cat = _db.Categories.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName

                }).ToList();
            List<SelectListItem> Comp = _db.Companies.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.CompanyId.ToString(),
                    Text = x.CompanyName

                }).ToList();
            List<SelectListItem> Ven = _db.Vendors.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.VendorId.ToString(),
                    Text = x.VendorName

                }).ToList();
           List<SelectListItem> Typ = _db.ProductTypes.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.TypeId.ToString(),
                    Text = x.TypeName

                }).ToList();
            List<SelectListItem> Uni = _db.Units.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.UnitId.ToString(),
                    Text = x.UnitName

                }).ToList();
            List<SelectListItem> Cli = _db.Clients.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.ClientId.ToString(),
                    Text = x.ClientName

                }).ToList();
            return Json(new {Category = Cat, Company = Comp, Vendor = Ven, Type = Typ, Unit = Uni, Client = Cli});
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            DDLload();
            return View();
        }

        [HttpGet]
        public ActionResult GetList()
        {
            DDLload();

            var prodList = (from a in _db.Products
                            where a.IsActive == true
                            select new ProductView
                            {
                                ProductId = a.ProductId,
                                ProductName = a.ProductName,
                                Price = a.Price,
                                ProductPostingDate = a.ProductPostingDate,
                                CategoryName = _db.Categories.Where(x => x.CategoryId == a.CategoryId).Select(x => x.CategoryName).First(),
                                CompanyName = _db.Companies.Where(x => x.CompanyId == a.CompanyId).Select(x => x.CompanyName).First(),
                                VendorName = _db.Vendors.Where(x => x.VendorId == a.VendorId).Select(x => x.VendorName).First(),
                                Hsnsaccode = a.Hsnsaccode,
                                Igstrate = a.Igstrate,
                                Cgstrate = a.Cgstrate,
                                Sgstrate = a.Sgstrate,
                                Code = a.Code,
                                PurchaseRate = a.PurchaseRate,
                                TypeName = _db.ProductTypes.Where(x => x.TypeId == a.TypeId).Select(x => x.TypeName).First(),
                                UnitName = _db.Units.Where(x => x.UnitId == a.UnitId).Select(x => x.UnitName).First(),
                                ClientName = _db.Clients.Where(x => x.ClientId == a.ClientId).Select(x => x.ClientName).First()

                            }).ToList();
            return Json(new { data = prodList });
        }



        // GET: CategoryController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
            int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
            string newCode = "Prod_" + newNumber.ToString("D3");
            ViewBag.Code = newCode;
            DDLload();
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product prod)
        {
            try
            {
                if (prod == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
                    int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
                    string newCode = "Prod_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                }
                else
                {
                    ModelState.Remove("ProductPostingDate");
                    ModelState.Remove("ProductId");
                    ModelState.Remove("Category");
                    ModelState.Remove("Company");
                    ModelState.Remove("Vendor");
                    ModelState.Remove("Type");
                    ModelState.Remove("Unit");
                    ModelState.Remove("Client");
                    if (ModelState.IsValid)
                    {
                        prod.ProductPostingDate = DateTime.Now;
                        _db.Add(prod);
                        _db.SaveChanges();


                        TempData["Create"] = "Data for Product : " + prod.ProductName + " is successfully submitted.";

                    }
                    else
                    {
                        var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
                        int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Prod_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                        TempData["Error"] = "Data for Product : " + prod.ProductName + " is invalid.";
                    }
                }

                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
            int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
            string newCode = "Prod_" + newNumber.ToString("D3");
            ViewBag.Code = newCode;
            var prod = _db.Products.Find(id);
            var catList = _db.Categories.Where(x => x.IsActive == true).Select(
                 x => new SelectListItem
                 {
                     Value = x.CategoryId.ToString(),
                     Text = x.CategoryName

                 });
            var compList = _db.Companies.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.CompanyId.ToString(),
                    Text = x.CompanyName

                });
            var venList = _db.Vendors.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.VendorId.ToString(),
                    Text = x.VendorName

                });
           var TypeList = _db.ProductTypes.Where(x => x.IsActive == true).Select(
               x => new SelectListItem
               {
                   Value = x.TypeId.ToString(),
                   Text = x.TypeName

               });
            var UnitList = _db.Units.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.UnitId.ToString(),
                    Text = x.UnitName

                });
            var ClientList = _db.Clients.Where(x => x.IsActive == true).Select(
                x => new SelectListItem
                {
                    Value = x.ClientId.ToString(),
                    Text = x.ClientName

                });
            ViewBag.CategoryListEdit = new SelectList(catList,"Value","Text",prod.CategoryId);
            ViewBag.CompanyListEdit = new SelectList(compList, "Value", "Text", prod.CompanyId);
            ViewBag.VenListEdit = new SelectList(venList, "Value", "Text", prod.VendorId);
            ViewBag.TypeListEdit = new SelectList(TypeList, "Value", "Text", prod.TypeId);
            ViewBag.UnitListEdit = new SelectList(UnitList, "Value", "Text", prod.UnitId);
            ViewBag.UnitListEdit = new SelectList(ClientList, "Value", "Text", prod.ClientId);

            return View(prod);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product collection)
        {
            try
            {
                if (collection == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
                    int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
                    string newCode = "Prod_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                }
                else
                {
                    ModelState.Remove("Category");
                    ModelState.Remove("Company");
                    ModelState.Remove("Vendor");
                    ModelState.Remove("Type");
                    ModelState.Remove("Unit");
                    ModelState.Remove("Client");
                    if (ModelState.IsValid)
                    {
                        _db.Update(collection);
                        _db.SaveChanges();

                        TempData["Edit"] = "Data for Product : " + collection.ProductName + " is updated successfully.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Product : " + collection.ProductName + " is invalid.";
                        var LastProduct = _db.Products.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ProductId).FirstOrDefault();
                        int newNumber = (LastProduct != null) ? int.Parse(LastProduct.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Prod_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";

                }
                else
                {
                    Product prod = _db.Products.Find(id);
                    prod.IsActive = false;
                    _db.SaveChanges();

                    TempData["Delete"] = "Data for Product : " + prod.ProductName + " is deleted sucessfully";

                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

       [HttpPost]
        public ActionResult SearchDetails(Product product)
        {
            DDLload();
            var ProductList = _db.Products.ToList();
            if(product == null)
            {
                return NotFound();
            }
            else
            {
                if(product.ProductName != null )
                {
                    ProductList = ProductList.Where(x=>  x.ProductName.ToUpper().Contains(product.ProductName.ToUpper()) ).ToList();
                }if(product.Price != 0 && product.Price != null)
                {
                    ProductList = ProductList.Where(x=>x.Price == product.Price).ToList();
                }if(product.CategoryId != 0 )
                {
                    ProductList=  ProductList.Where(x=>x.CategoryId == product.CategoryId).ToList();
                }if(product.CompanyId != 0 )
                {
                    ProductList = ProductList.Where(x=>x.CompanyId == product.CompanyId).ToList();
                }if(product.VendorId != 0 )
                {
                    ProductList = ProductList.Where(x=>x.VendorId == product.VendorId).ToList();
                }if(product.Hsnsaccode != null)
                {
                    ProductList = ProductList.Where(x=>x.Hsnsaccode == product.Hsnsaccode).ToList();
                }if(product.Igstrate != 0 )
                {
                    ProductList=  ProductList.Where(x=>x.Igstrate == product.Igstrate).ToList();
                }if(product.Cgstrate != 0 )
                {
                    ProductList = ProductList.Where(x=>x.Cgstrate == product.Cgstrate).ToList();
                }if(product.Sgstrate != 0 )
                {
                    ProductList = ProductList.Where(x=>x.Sgstrate == product.Sgstrate).ToList();
                }if(product.Code != null )
                {
                    ProductList = ProductList.Where(x=>x.Code == product.Code).ToList();
                }if(product.TypeId != 0 )
                {
                    ProductList = ProductList.Where(x=>x.TypeId == product.TypeId).ToList();
                }if(product.UnitId != 0 )
                {
                    ProductList = ProductList.Where(x=>x.UnitId == product.UnitId).ToList();
                }if(product.ClientId != 0 )
                {
                    ProductList = ProductList.Where(x=>x.ClientId == product.ClientId).ToList();
                }
                if (product.Code != null)
                {
                    ProductList = ProductList.Where(x => x.Code.ToUpper().Contains(product.Code.ToUpper())).ToList();
                }
                if (product.IsActiveStr != null)
                {
                    bool isative = Convert.ToBoolean(product.IsActiveStr);
                    ProductList = ProductList.Where(x => x.IsActive == isative).ToList();
                }

            }

            var prodList = (from a in ProductList
                            select new ProductView
                            {
                                ProductId =  a.ProductId,
                                ProductName = a.ProductName,
                                Price = a.Price,
                                CategoryName = _db.Categories.Where(x => x.CategoryId == a.CategoryId).Select(x => x.CategoryName).First(),
                                CompanyName = _db.Companies.Where(x => x.CompanyId == a.CompanyId).Select(x => x.CompanyName).First(),
                                VendorName = _db.Vendors.Where(x => x.VendorId == a.VendorId).Select(x => x.VendorName).First(),
                                ProductPostingDate = a.ProductPostingDate,
                                Action = "",
                                Hsnsaccode = a.Hsnsaccode,
                                Igstrate = a.Igstrate,
                                Cgstrate = a.Cgstrate,
                                Sgstrate = a.Sgstrate,
                                PurchaseRate = a.PurchaseRate,
                                Code = a.Code,
                                TypeName = _db.ProductTypes.Where(x => x.TypeId == a.TypeId).Select(x => x.TypeName).First(),
                                UnitName = _db.Units.Where(x => x.UnitId == a.UnitId).Select(x => x.UnitName).First(),
                                ClientName = _db.Clients.Where(x => x.ClientId == a.ClientId).Select(x => x.ClientName).First()

                            }).ToList();


            return Json(new { data = prodList });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var ProdList = (from a in _db.Products
                                where a.IsActive == true
                                select new ProductView
                                {
                                    ProductId = a.ProductId,
                                    ProductName = a.ProductName,
                                    Price = a.Price,
                                    ProductPostingDate = a.ProductPostingDate,
                                    CategoryName = _db.Categories.Where(x => x.CategoryId == a.CategoryId).Select(x => x.CategoryName).First(),
                                    CompanyName = _db.Companies.Where(x => x.CompanyId == a.CompanyId).Select(x => x.CompanyName).First(),
                                    VendorName = _db.Vendors.Where(x => x.VendorId == a.VendorId).Select(x => x.VendorName).First(),
                                    Hsnsaccode = a.Hsnsaccode,
                                    Igstrate = a.Igstrate,
                                    Cgstrate = a.Cgstrate,
                                    Sgstrate = a.Sgstrate,
                                    PurchaseRate = a.PurchaseRate,
                                    Code = a.Code,
                                    TypeName = _db.ProductTypes.Where(x => x.TypeId == a.TypeId).Select(x => x.TypeName).First(),
                                    UnitName = _db.Units.Where(x => x.UnitId == a.UnitId).Select(x => x.UnitName).First(),
                                    ClientName = _db.Clients.Where(x => x.ClientId == a.ClientId).Select(x => x.ClientName).First()

                                }).ToList();

                var filename = "Product.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Code"),
                    new DataColumn("Product Name"),
                    new DataColumn("Selling Price"),
                    new DataColumn("Purchase Rate"),
                    new DataColumn("Category Name"),
                    new DataColumn("Company Name"),
                    new DataColumn("Vendor Name"),
                    new DataColumn("Type Name"),
                    new DataColumn("Unit Name"),
                    new DataColumn("Client Name"),
                    new DataColumn("HSN/SAC Code"),
                    new DataColumn("IGST Rate"),
                    new DataColumn("CGST Rate"),
                    new DataColumn("SGST Rate"),
                    new DataColumn("Product Posting Date")

                });

                foreach (var a in ProdList)
                {
                    dt.Rows.Add(a.Code,a.ProductName, a.Price,a.PurchaseRate, a.CategoryName, a.CompanyName, a.VendorName,a.TypeName,a.UnitName, a.ClientName, a.Hsnsaccode,a.Igstrate,a.Cgstrate,a.Sgstrate, a.ProductPostingDate);
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var sheet1 = wb.AddWorksheet(dt,"Sheet1");
                    sheet1.Row(1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.Blue);
                    sheet1.Row(1).CellsUsed().Style.Font.Bold = true;
                    sheet1.Row(1).CellsUsed().Style.Font.FontColor = XLColor.White;

                    sheet1.Columns().AdjustToContents();
                    sheet1.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    sheet1.Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    sheet1.Rows(2,ProdList.Count()+1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.SkyBlue);
                  
                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
            }catch (Exception ex)
            {
                return NotFound();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
