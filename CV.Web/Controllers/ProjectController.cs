using CV.DataLayer;
using CV.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.Web.Controllers
{
    public class ProjectController : BaseController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectController()
        {
            UsesDatabase = true;
        }

        protected override string SetTitle(string action)
        {
            switch (action.ToLower())
            {
                case "personalprojects": return "Personal Projects";
                case "commercialprojects": return "Commercial Projects";
                default: return base.SetTitle(action);
            }
        }

        #region Privates

        public int PersonalCompanyId
        {
            get
            {
                return (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                 where m.Name == DataLayerConstants.PersonalProjectCompany
                 select m.ID).First();
            }
        }

        private ProjectModel GetProjectInfo(int projectId)
        {
            CV.DataLayer.Project projectInfo = (from m in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS where m.ID == projectId select m).FirstOrDefault();
            if (projectInfo == null)
                throw new InvalidOperationException();

            ProjectModel projectInfoModel = new ProjectModel()
            {
                ProjectID = projectId,
                EndDate = projectInfo.EndYear,
                Link = projectInfo.Link,
                Name = projectInfo.Name,
                StartDate = projectInfo.StartYear,
                CompanyID = projectInfo.CompanyId
            };

            return projectInfoModel;
        }

        public IEnumerable<CompanyModel> CommercialCompanies
        {
            get
            {
                return from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                       where m.Name != DataLayerConstants.PersonalProjectCompany
                       select new CompanyModel() { Name = m.Name, CompanyID = m.ID };
            }
        }

        #endregion

        #region Actions
        public ActionResult PersonalProjects()
        {
            ViewBag.Header = "These projects are personal projects, more like about my hobbies or supportive applications for my work. Some of them are from old times (which I mention in the details). Click on a project name to learn more about details about them.";
            return View(new ProjectListModel(true));
        }

        public ActionResult CommercialProjects()
        {
            ViewBag.Header = "These projects are the work items I have worked over in companies. I cannot give detailed information on some of them due to compliance limitations. Click on a project name to learn more about details of my work there (ordered by newest to oldest).";
            return View(new ProjectListModel(false));
        }

        #endregion

        #region AJAX Calls/Queries

        /// <summary>
        /// Saves existing project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>View as result</returns>
        /// <remarks>Parameter is nullable because of return url crashes</remarks>
        [Authorize, HttpPost]
        public ActionResult Save(ProjectModel model)
        {
            if (!IsAdministrator) return RedirectToProjectIndex();

            try
            {
                //: make some checks
                if (model == null)
                    return Json("Project instance is null. Something gone wrong!");

                //: check properties
                if (string.IsNullOrEmpty(model.Name)) return Json("Company Name cannot be left empty!");
                else if (model.StartDate > DateTime.Today) return Json("Start date for position is invalid!");
                else if (model.EndDate.HasValue && model.EndDate > DateTime.Today) return Json("End date for position is invalid!");
                else if (model.EndDate.HasValue && model.EndDate < model.StartDate) return Json("End date for position is before Start date!");

                CV.DataLayer.Project projectData = null;
                if (model.ProjectID == 0)
                {
                    var otherProjects = (from m in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS
                                          where m.Name == model.Name
                                          select m.ID).Count();
                    if (otherProjects > 0)
                        return Json("Another Project with same title exists!");

                    projectData = new DataLayer.Project();
                    CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS.Add(projectData);
                }
                else
                {
                    projectData = (from m in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS
                                   where m.ID == model.ProjectID
                                   select m).FirstOrDefault();

                    if (projectData == null)
                        return Json("Project id is not valid, project not found!");
                }

                var associatedCompany = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                                                   where model.CompanyID == m.ID
                                                   select m.ID).Count();
                if (associatedCompany == 0)
                    return Json("Project's associated company is not present in system!");

                projectData.ID = model.ProjectID;
                projectData.EndYear = model.EndDate;
                projectData.Link = model.Link;
                projectData.CompanyId = model.CompanyID;
                projectData.StartYear = model.StartDate;
                projectData.Name = model.Name;

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS" + projectData.ID);
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Project info saving failed! {0}", error));
            }
        }

        [HttpGet]
        public ActionResult ShowProjectInfo(int projectId)
        {
            ProjectModel model = GetProjectInfo(projectId);
            if( model.IsPersonal)
                return PartialView("~/Views/Shared/DisplayTemplates/PersonalProjectModel.cshtml", model);
            else
                return PartialView("~/Views/Shared/DisplayTemplates/ProjectModel.cshtml", model);
        }

        [HttpGet, Authorize]
        public ActionResult EditProjectInfo(int projectId)
        {
            if (!IsAdministrator)
                return Json("UNAUTHORIZED");

            ProjectModel model = GetProjectInfo(projectId);
            if( model.IsPersonal)
                return PartialView("~/Views/Shared/EditorTemplates/PersonalProjectModel.cshtml", model);
            else
                return PartialView("~/Views/Shared/EditorTemplates/ProjectModel.cshtml", model);
        }

        [Authorize, HttpGet]
        public ActionResult New(bool personal)
        {
            ProjectModel projectInfoModel = new ProjectModel() { ProjectID = 0, CompanyID =0, StartDate = DateTime.Today };

            if( personal)
            {
                var personalCompanyId = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                                         where m.Name == DataLayerConstants.PersonalProjectCompany
                                         select m.ID).First();
                projectInfoModel.CompanyID = personalCompanyId;

                return PartialView("~/Views/Shared/EditorTemplates/PersonalProjectModel.cshtml", projectInfoModel);
            }
            else                
                return PartialView("~/Views/Shared/EditorTemplates/ProjectModel.cshtml", projectInfoModel);
        }

        [Authorize, HttpPost]
        public JsonResult Delete(int projectId)
        {
            if (!IsAdministrator) return Json("UNAUTHORIZED");

            try
            {
                var project = CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS.Single(m => m.ID == projectId);
                if (project == null)
                    return Json("Project does not exist, invalid project ID!");

                //: remove contents
                var contents = from p in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTINFO where p.OwnerId == projectId select p;
                foreach (CV.DataLayer.ProjectContent pi in contents)
                    CV.DataLayer.CVDbContext.DatabaseContext.PROJECTINFO.Remove(pi);

                //: remove project
                CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS.Remove(project);

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