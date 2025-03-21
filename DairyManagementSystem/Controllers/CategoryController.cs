using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DairyManagementSystem.Models;
using System.Data;
using ClosedXML.Excel;

namespace DairyManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DMSContext _db;
       
        public CategoryController(DMSContext db)
        {
            _db = db;
        } 


        // GET: CategoryController
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult GetList()
        {
            var catList = _db.Categories.Where(x=>x.IsActive==true).ToList();
            return Json(new { data = catList });
        }

        // GET: CategoryController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var Last = _db.Categories.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.CategoryId).FirstOrDefault();
            int newNumber = (Last != null) ? int.Parse(Last.Code.Split('_')[1]) + 1 : 1;
            string newCode = "Cat_" + newNumber.ToString("D3");
            ViewBag.Code = newCode;
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category cat)
        {
            try
            {
                if (cat == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                }
                else
                {
                    ModelState.Remove("CategoryPostingDate");
                    ModelState.Remove("CategoryId");
                    if (ModelState.IsValid)
                    {
                        cat.CategoryPostingDate = DateTime.Now;
                        _db.Add(cat);
                        _db.SaveChanges();
                       

                        TempData["Create"] = "Data for Category : " + cat.CategoryName + " is successfully submitted.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Category : " + cat.CategoryName + " is invalid.";

                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            Category cat = _db.Categories.Where(z=>z.CategoryId==id).First();
           
           return View(cat);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category collection)
        {
            try
            {
                if (collection == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _db.Update(collection);
                        _db.SaveChanges();

                        TempData["Edit"] = "Data for Category : " + collection.CategoryName + " is updated successfully.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Category : " + collection.CategoryName + " is invalid.";

                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
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
                    Category cat = _db.Categories.Find(id);
                    cat.IsActive = false;
                    _db.SaveChanges();

                   TempData["Delete"] = "Data for category : "+cat.CategoryName + " is deleted sucessfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SearchDetails(Category cat)
        {
            var catList = _db.Categories.ToList();
            if (cat == null)
            {
                return NotFound();
            }
            else
            {
                if (cat.CategoryName != null)
                {
                    catList = catList.Where(x =>  x.CategoryName.ToUpper().Contains(cat.CategoryName.ToUpper())).ToList();
                }if (cat.Code != null)
                {
                    catList = catList.Where(x => x.Code.ToUpper().Contains(cat.Code.ToUpper())).ToList();
                }
                if (cat.IsActiveStr != null)
                {
                    bool isative = Convert.ToBoolean(cat.IsActiveStr);
                    catList = catList.Where(x => x.IsActive == isative).ToList();
                }

            }

            return Json(new { data = catList });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var CatList = _db.Categories.ToList();

                var filename = "Category.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Code"),
                    new DataColumn("Category Name"),
                    new DataColumn("Category Posting Date")
                });

                foreach (var a in CatList)
                {
                    dt.Rows.Add(a.Code, a.CategoryName, a.CategoryPostingDate);
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
                    sheet1.Rows(2, CatList.Count() + 1).CellsUsed().Style.Fill.SetBackgroundColor(XLColor.SkyBlue);

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
