using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DairyManagementSystem.Models;
using System.Data;
using ClosedXML.Excel;

namespace DairyManagementSystem.Controllers
{
    public class VendorController : Controller
    {
        private readonly DMSContext _db;

        public VendorController(DMSContext db)
        {
            _db = db;
        }


        // GET: VendorController

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetList()
        {
            List<Vendor> venList = _db.Vendors.Where(x => x.IsActive == true).ToList();
            return Json(new { data = venList });
        }

        // GET: VendorController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var lastVendor = _db.Vendors.Where(x=> x.IsActive==true).OrderByDescending(c => c.VendorId).FirstOrDefault();
            int newNumber = (lastVendor != null) ? int.Parse(lastVendor.Code.Split('_')[1]) + 1 : 1;
            string newCode = "Ven_" + newNumber.ToString("D3");
            ViewBag.Code= newCode;

            string str = "false";
            ViewBag.ValidEndDate = str;
            var highId = _db.Companies.Where(x => x.IsActive == true).ToList();
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vendor ven)
        {
            try
            {
                if (ven == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var LastVen = _db.Vendors.Where(x=> x.IsActive==true).OrderByDescending(c => c.VendorId).FirstOrDefault();
                    int newNumber = (LastVen != null) ? int.Parse(LastVen.Code.Split('_')[1]) + 1 : 1;
                    string newCode = "Ven_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                    return View(ven);
                }
                else
                {
                    ModelState.Remove("VendorPostingDate");
                    ModelState.Remove("VendorId");
                    ModelState.Remove("Code");
                    if (ven.isOperational == "false" && ven.EndDate == null)
                    {
                        var str = "true";
                        ViewBag.ValidEndDate = str;
                        ModelState.AddModelError("EndDate", "End Date or Check Box Required.");
                        return View(ven);
                    }
                    if (ModelState.IsValid)
                    {
                        ven.VendorPostingDate = DateTime.Now;
                        _db.Add(ven);
                        _db.SaveChanges();


                        TempData["Create"] = "Data for Vendor : " + ven.VendorName + " is successfully submitted.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Vendor : " + ven.VendorName + " is invalid.";
                        var LastVen = _db.Vendors.Where(x=> x.IsActive==true).OrderByDescending(c => c.VendorId).FirstOrDefault();
                        int newNumber = (LastVen != null) ? int.Parse(LastVen.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Ven_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                        return View(ven);

                    }
                }


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            string str = "false";
            ViewBag.ValidEndDate = str;
            Vendor ven = _db.Vendors.Find(id);

            return View(ven);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendor collection)
        {
            try
            {
                if (collection == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";

                }
                else
                {
                    if (collection.isOperational == "false" && collection.EndDate == null)
                    {
                        var str = "true";
                        ViewBag.ValidEndDate = str;
                        ModelState.AddModelError("EndDate", "End Date or Check Box Required.");
                    }

                    if (ModelState.IsValid)
                    {
                        _db.Update(collection);
                        _db.SaveChanges();

                        TempData["Edit"] = "Data for Vendor : " + collection.VendorName + " is updated successfully.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Vendor : " + collection.VendorName + " is invalid.";
                        var LastVen = _db.Vendors.Where(x=> x.IsActive==true).OrderByDescending(c => c.VendorId).FirstOrDefault();
                        int newNumber = (LastVen != null) ? int.Parse(LastVen.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Ven_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                        return View(collection);
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
                    Vendor ven = _db.Vendors.Find(id);
                    ven.IsActive = false;
                    _db.SaveChanges();

                    TempData["Delete"] = "Data for Vendor : " + ven.VendorName + " is deleted sucessfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SearchDetails(Vendor ven)
        {
            var venList = _db.Vendors.ToList();
            if (ven == null)
            {
                return NotFound();
            }
            else
            {
                if (ven.VendorName != null)
                {
                    venList = venList.Where(x => x.VendorName.ToUpper().Contains(ven.VendorName.ToUpper())).ToList();
                }
                if (ven.StartDate.Year != 0001)
                {
                    venList = venList.Where(x => ven.StartDate == x.StartDate).ToList();
                }
                if (ven.EndDate != null)
                {
                    venList = venList.Where(x => ven.EndDate == x.EndDate).ToList();
                }
                if (ven.Address != null)
                {
                    venList = venList.Where(x => x.Address.ToUpper().Contains(ven.Address.ToUpper())).ToList();
                }
                if (ven.StateCode != 0)
                {
                    venList = venList.Where(x => ven.StateCode == x.StateCode).ToList();
                }
                if (ven.BankDetail != null)
                {
                    venList = venList.Where(x => x.BankDetail.ToUpper().Contains(ven.BankDetail.ToUpper())).ToList();
                }
                if (ven.Code != null)
                {
                    venList = venList.Where(x => x.Code.ToUpper().Contains(ven.Code.ToUpper())).ToList();
                }
                if (ven.IsActiveStr != null)
                {
                    bool isative = Convert.ToBoolean(ven.IsActiveStr);
                    venList = venList.Where(x => x.IsActive == isative).ToList();
                }

            }


            return Json(new { data = venList });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var VenList = _db.Vendors.ToList();

                var filename = "Vendor.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Vendor Name"),
                    new DataColumn("Vendor Posting Date"),
                    new DataColumn("Start Date"),
                    new DataColumn("End Date"),
                    new DataColumn("Address"),
                    new DataColumn("State Code"),
                    new DataColumn("Bank Detail")
                });

                foreach (var a in VenList)
                {
                    dt.Rows.Add(a.VendorName, a.VendorPostingDate, a.StartDate, a.EndDate == null ? "---" : a.EndDate, a.Address, a.StateCode, a.BankDetail);
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
    }
}
