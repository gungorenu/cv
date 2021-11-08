using CV.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.Web.Controllers
{
    public class AdminController : BaseController
    {

        public ActionResult Database()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View(new FileListModel());
        }

        [HttpPost, Authorize]
        public ActionResult UploadFile()
        {
            if (!IsAdministrator)
                return Json("UNAUTHORIZED");

            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/images/custom", fileName);
                        file.SaveAs(path);
                    }
                }

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("File upload failed! {0}", error));
            }
        }

        [Authorize]
        public ActionResult PrepareDatabase()
        {
            if (!IsAdministrator)
                return RedirectToIndex();

            try
            {
                CV.DataLayer.CVDbContext.DatabaseContext.CreateTablesWithCheck();
                ViewBag.Result = "SUCCESS";
            }
            catch ( Exception ex )
            {
                ViewBag.Result = ex.Message + "\r\n" + ex.StackTrace;
            }

            return View();
        }

    }
}