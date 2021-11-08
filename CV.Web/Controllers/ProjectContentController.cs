using CV.DataLayer;
using CV.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web.Controllers
{
    public class ProjectContentController : BaseContentController<CV.DataLayer.ProjectContent>
    {
        #region Abstract Overrides

        protected override ProjectContent GetDataModelFromViewModel(ContentModel viewModel)
        {
            return new ProjectContent(viewModel);
        }

        protected override bool ValidateOwner(int ownerId)
        {
            var projectData = (from m in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS
                               where m.ID == ownerId
                               select m).FirstOrDefault();

            return (projectData != null);
        }

        #endregion
    }
}