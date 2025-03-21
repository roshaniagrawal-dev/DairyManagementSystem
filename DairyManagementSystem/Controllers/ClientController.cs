using ClosedXML.Excel;
using DairyManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DairyManagementSystem.Controllers
{
    public class ClientController : Controller
    {
        private readonly DMSContext _db;

        public ClientController(DMSContext db)
        {
            _db = db;
        }

        // GET: ClientController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetList()
        {
            var catList = _db.Clients.Where(x => x.IsActive == true).ToList();
            return Json(new { data = catList });
        }

        // GET: ClientController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClientController/Create
        public ActionResult Create()
        {
            var LastClient = _db.Clients.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ClientId).FirstOrDefault();
            int newNumber = (LastClient != null) ? int.Parse(LastClient.ClientCode.Split('_')[1]) + 1 : 1;
            string newCode = "Cli_" + newNumber.ToString("D3");
            ViewBag.Code = newCode;
            return View();
        }

        // POST: ClientController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client cat)
        {
            try
            {
                if (cat == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var LastClient = _db.Clients.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ClientId).FirstOrDefault();
                    int newNumber = (LastClient != null) ? int.Parse(LastClient.ClientCode.Split('_')[1]) + 1 : 1;
                    string newCode = "Cli_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                }
                else
                {
                    ModelState.Remove("ClientPostingDate");
                    ModelState.Remove("ClientId");
                    if (ModelState.IsValid)
                    {
                        cat.ClientPostingDate = DateTime.Now;
                        _db.Add(cat);
                        _db.SaveChanges();


                        TempData["Create"] = "Data for Client : " + cat.ClientName + " is successfully submitted.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Client : " + cat.ClientName + " is invalid.";
                        var LastClient = _db.Clients.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ClientId).FirstOrDefault();
                        int newNumber = (LastClient != null) ? int.Parse(LastClient.ClientCode.Split('_')[1]) + 1 : 1;
                        string newCode = "Cli_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClientController/Edit/5
        public ActionResult Edit(int id)
        {
            Client cat = _db.Clients.Where(z => z.ClientId == id).First();
            return View(cat);
        }

        // POST: ClientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client collection)
        {
            try
            {
                if (collection == null)
                {
                    TempData["Error"] = "Data is not existing, please try to reload and try again.";
                    var LastClient = _db.Clients.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ClientId).FirstOrDefault();
                    int newNumber = (LastClient != null) ? int.Parse(LastClient.ClientCode.Split('_')[1]) + 1 : 1;
                    string newCode = "Cli_" + newNumber.ToString("D3");
                    ViewBag.Code = newCode;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _db.Update(collection);
                        _db.SaveChanges();

                        TempData["Edit"] = "Data for Client : " + collection.ClientName + " is updated successfully.";

                    }
                    else
                    {
                        TempData["Error"] = "Data for Client : " + collection.ClientName + " is invalid.";
                        var LastClient = _db.Clients.Where(x => x.IsActive == true).Where(x => x.IsActive == true).OrderByDescending(c => c.ClientId).FirstOrDefault();
                        int newNumber = (LastClient != null) ? int.Parse(LastClient.ClientCode.Split('_')[1]) + 1 : 1;
                        string newCode = "Cli_" + newNumber.ToString("D3");
                        ViewBag.Code = newCode;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // POST: ClientController/Delete/5
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
                    Client cat = _db.Clients.Find(id);
                    cat.IsActive = false;
                    _db.SaveChanges();

                    TempData["Delete"] = "Data for Client : " + cat.ClientName + " is deleted sucessfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SearchDetails(Client cat)
        {
           
            bool  IsActive = Convert.ToBoolean(cat.IsActiveStr);
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@ClientName", Value = cat.ClientName == null ? DBNull.Value : cat.ClientName},
                    new SqlParameter { ParameterName = "@IsActive", Value = cat.IsActiveStr == null ? true : IsActive},
                    new SqlParameter { ParameterName = "@ClientCode", Value = cat.ClientCode == null ? DBNull.Value : cat.ClientCode },
                    new SqlParameter { ParameterName = "@ClientAddress", Value = cat.ClientAddress == null ? DBNull.Value : cat.ClientAddress },
                    new SqlParameter { ParameterName = "@MobileNo", Value = cat.MobileNo == null ? DBNull.Value : cat.MobileNo},
                };

            var catList = _db.Set<Client>().FromSqlRaw($"[dbo].[Sp_GetClientList_Search] " +
                $"@ClientName," +
                $"@IsActive," +
                $"@ClientCode," +
                $"@ClientAddress," +
                $"@MobileNo" +
                $"", parms.ToArray()).ToList();

            return Json(new { data = catList });

        }

        public ActionResult ExportExcel()
        {
            try
            {
                var CatList = _db.Clients.ToList();

                var filename = "Client.xlsx";

                DataTable dt = new DataTable();
                dt.TableName = "DataList";
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Client Name"),
                    new DataColumn("Code"),
                    new DataColumn("Address"),
                    new DataColumn("Mobile No."),
                    new DataColumn("Client Posting Date")
                });

                foreach (var a in CatList)
                {
                    dt.Rows.Add(a.ClientName, a.ClientCode, a.ClientAddress, a.MobileNo, a.ClientPostingDate);
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
