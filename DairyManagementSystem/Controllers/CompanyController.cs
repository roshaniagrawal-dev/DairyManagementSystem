using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DairyManagementSystem.Models;
using System.Data;
using ClosedXML.Excel;
using System.Linq;

namespace DairyManagementSystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly DMSContext _db;

        public CompanyController(DMSContext db)
        {
            _db = db;
        }


        // GET: CategoryController

        public ActionResult Index()
        {
            //var compList = _db.Companies.Where(x => x.IsActive == true).ToList();
            return View();
        }
        
        [HttpGet]
        public ActionResult GetList()
        {
            List<Company> compList = _db.Companies.Where(x => x.IsActive == true).ToList();
            return Json(new { data = compList });
        }

        // GET: CategoryController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var lastCompany = _db.Companies.Where(x=> x.IsActive==true).Where(x=> x.IsActive==true).OrderByDescending(c => c.CompanyId).FirstOrDefault();
            int newNumber = (lastCompany != null) ? int.Parse(lastCompany.Code.Split('_')[1]) + 1 : 1;
            string newCode = "Cmp_" + newNumber.ToString("D3");
            ViewBag.Code = newCode;


            string str = "false";
            ViewBag.ValidEndDate = str;
            var highId = _db.Companies.Where(x => x.IsActive == true).ToList();
            
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company comp)
        {
            try
            {
                if (comp == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var lastCompany = _db.Companies.Where(x=> x.IsActive==true).OrderByDescending(c => c.CompanyId).FirstOrDefault();
                    int newNumber = (lastCompany != null) ? int.Parse(lastCompany.Code.Split('_')[1]) + 1 : 1;
                    string newCode = "Cmp_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                    return View(comp);
                }
                else
                {
                    ModelState.Remove("CompanyPostingDate");
                    ModelState.Remove("CompanyId");
                    ModelState.Remove("Code");
                    if(comp.isOperational == "false" && comp.EndDate == null)
                    {
                        var str = "true";
                        ViewBag.ValidEndDate = str;
                        ModelState.AddModelError("EndDate","End Date or Check Box Required.");
                    }
                    if (ModelState.IsValid)
                    {
                        comp.CompanyPostingDate = DateTime.Now;
                        _db.Add(comp);
                        _db.SaveChanges();


                        TempData["Create"] = "Data for Company : " + comp.CompanyName + " is successfully submitted.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Company : " + comp.CompanyName + " is invalid.";
                        var lastCompany = _db.Companies.Where(x=> x.IsActive==true).OrderByDescending(c => c.CompanyId).FirstOrDefault();
                        int newNumber = (lastCompany != null) ? int.Parse(lastCompany.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Cmp_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                        return View(comp);

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
            Company prod = _db.Companies.Find(id);

            return View(prod);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company collection)
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

                        TempData["Edit"] = "Data for Company : " + collection.CompanyName + " is updated successfully.";

                    }
                    else
                    {
                        var lastCompany = _db.Companies.Where(x=> x.IsActive==true).OrderByDescending(c => c.CompanyId).FirstOrDefault();
                        int newNumber = (lastCompany != null) ? int.Parse(lastCompany.Code.Split('_')[1]) + 1 : 1;
                        string newCode = "Cmp_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                        TempData["Error"] = "Data for Company : " + collection.CompanyName + " is invalid.";
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
                    Company comp = _db.Companies.Find(id);
                    comp.IsActive = false;
                    _db.SaveChanges();

                    TempData["Delete"] = "Data for Company : " + comp.CompanyName + " is deleted sucessfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SearchDetails(Company comp)
        {
            var compList = _db.Companies.ToList();
           
            if (comp == null)
            {
                return NotFound();
            }
            else 
            {
                
                if (comp.CompanyName != null)
                {
                    compList = compList.Where(x =>  x.CompanyName.ToUpper().Contains(comp.CompanyName.ToUpper())).ToList();
                }if (comp.StartDate.Year != 0001)
                {
                    compList = compList.Where(x => comp.StartDate == x.StartDate).ToList();
                }if (comp.EndDate != null)
                {
                    compList = compList.Where(x => comp.EndDate == x.EndDate).ToList();
                }
                if (comp.CompanyAddress != null)
                {
                    compList = compList.Where(x =>  x.CompanyAddress.ToUpper().Contains(comp.CompanyAddress.ToUpper())).ToList();
                }if (comp.StateCode != 0)
                {
                    compList = compList.Where(x => comp.StateCode == x.StateCode).ToList();
                }
                if (comp.BankDetail != null)
                {
                    compList = compList.Where(x =>  x.BankDetail.ToUpper().Contains(comp.BankDetail.ToUpper())).ToList();
                }if (comp.Code != null)
                {
                    compList = compList.Where(x =>  x.Code.ToUpper().Contains(comp.Code.ToUpper())).ToList();
                } if (comp.IsActiveStr != null)
                {
                    bool isative = Convert.ToBoolean(comp.IsActiveStr);
                    compList = compList.Where(x => x.IsActive == isative).ToList();
                }
              
            }


            return Json( new { data = compList });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var CompList = _db.Companies.ToList();

                var filename = "Company.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Company Name"),
                    new DataColumn("Company Posting Date"),
                     new DataColumn("Start Date"),
                    new DataColumn("End Date"),
                    new DataColumn("Address"),
                    new DataColumn("State Code"),
                    new DataColumn("Bank Detail")
                });

                foreach (var a in CompList)
                {
                    dt.Rows.Add(a.CompanyName, a.CompanyPostingDate, a.StartDate, a.EndDate == null ? "---" : a.EndDate, a.CompanyAddress, a.StateCode, a.BankDetail);
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
                    sheet1.Rows(2, CompList.Count() + 1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.SkyBlue);

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
