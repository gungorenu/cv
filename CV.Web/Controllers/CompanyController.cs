using CV.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.Web.Controllers
{
    public class CompanyController : BaseController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CompanyController()
        {
            UsesDatabase = true;
        }

        protected override string SetTitle(string action)
        {
            switch (action.ToLower())
            {
                case "index": return "Companies I Worked At";
                default: return base.SetTitle(action);
            }
        }

        #region Actions
        public ActionResult Index(int companyId =0)
        {
            return View(new CompanyListModel());
        }

        private CompanyModel GetCompanyInfo( int companyId )
        {
            CV.DataLayer.Company companyInfo = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES where m.ID == companyId select m).FirstOrDefault();
            if (companyInfo == null)
                throw new InvalidOperationException();

            CompanyModel companyInfoModel = new CompanyModel()
            {
                CompanyID = companyId,
                EndDate = companyInfo.EndYear,
                Link = companyInfo.Link,
                Name = companyInfo.Name,
                Positions = companyInfo.Positions,
                StartDate = companyInfo.StartYear
            };

            return companyInfoModel;
        }

        #endregion

        #region AJAX Calls/Queries

        /// <summary>
        /// Saves existing company
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <returns>View as result</returns>
        /// <remarks>Parameter is nullable because of return url crashes</remarks>
        [Authorize, HttpPost]
        public ActionResult Save(CompanyModel model)
        {
            if (!IsAdministrator) return RedirectToCompanyIndex();

            try
            {
                //: make some checks
                if (model == null)
                    return Json("Company instance is null. Something gone wrong!");

                //: check properties
                if (string.IsNullOrEmpty(model.Name)) return Json("Company Name cannot be left empty!");
                else if (string.IsNullOrEmpty(model.Positions)) return Json("Position info cannot be left empty!");
                else if (model.StartDate > DateTime.Today) return Json("Start date for position is invalid!");
                else if (model.EndDate.HasValue && model.EndDate > DateTime.Today) return Json("End date for position is invalid!");
                else if (model.EndDate.HasValue && model.EndDate < model.StartDate) return Json("End date for position is before Start date!");

                CV.DataLayer.Company companyData = null;
                if (model.CompanyID == 0)
                {
                    var otherCompanies = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                                          where m.Name == model.Name
                                          select m).Count();
                    if (otherCompanies > 0)
                        return Json("Another Company with same title exists!");

                    companyData = new DataLayer.Company();
                    CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES.Add(companyData);
                }
                else
                {
                    companyData = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                                   where m.ID == model.CompanyID
                                   select m).FirstOrDefault();

                    if (companyData == null)
                        return Json("Company id is not valid, company not found!");
                }

                companyData.ID = model.CompanyID;
                companyData.EndYear = model.EndDate;
                companyData.Link = model.Link;
                companyData.Positions = model.Positions;
                companyData.StartYear = model.StartDate;
                companyData.Name = model.Name;

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS" + companyData.ID);
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Company info saving failed! {0}", error));
            }
        }

        [HttpGet]
        public ActionResult ShowCompanyInfo(int companyId)
        {
            return PartialView("~/Views/Shared/DisplayTemplates/CompanyModel.cshtml", GetCompanyInfo(companyId));
        }

        [HttpGet, Authorize]
        public ActionResult EditCompanyInfo(int companyId)
        {
            return PartialView("~/Views/Shared/EditorTemplates/CompanyModel.cshtml", GetCompanyInfo(companyId));
        }

        [Authorize, HttpGet]
        public ActionResult New()
        {
            CompanyModel companyInfoModel = new CompanyModel() { CompanyID = 0, StartDate = DateTime.Today };

            return PartialView("~/Views/Shared/EditorTemplates/CompanyModel.cshtml", companyInfoModel);
        }

        [Authorize, HttpPost]
        public JsonResult Delete(int companyId)
        {
            if (!IsAdministrator) return Json("UNAUTHORIZED");

            try
            {
                var company = CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES.Single(m => m.ID == companyId);
                if( company == null)
                    return Json("Company does not exist, invalid company ID!");

                //: remove contents
                var contents = from p in CV.DataLayer.CVDbContext.DatabaseContext.COMPANYINFO where p.OwnerId == companyId select p;
                foreach (CV.DataLayer.CompanyContent pi in contents)
                    CV.DataLayer.CVDbContext.DatabaseContext.COMPANYINFO.Remove(pi);

                //: remove project parts
                var projectContents = (from pc in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTINFO
                                       join CV.DataLayer.Project p in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS on pc.OwnerId equals p.ID
                                       where p.CompanyId == companyId
                                       select pc);
                foreach( CV.DataLayer.ProjectContent pc in projectContents)
                    CV.DataLayer.CVDbContext.DatabaseContext.PROJECTINFO.Remove(pc);

                //: remove projects
                var project = from p in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS where p.CompanyId == companyId select p;
                foreach (CV.DataLayer.Project p in project)
                    CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS.Remove(p);

                CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES.Remove(company);

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();
                return Json("SUCCESS");
            }
            catch
            {
                return Json("FAILURE");
            }
        }
        #endregion

    }
}