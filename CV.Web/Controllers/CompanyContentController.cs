using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CV.DataLayer;
using CV.Web.Models;

namespace CV.Web.Controllers
{
    public class CompanyContentController : BaseContentController<CV.DataLayer.CompanyContent>
    {
        #region Abstract Overrides

        protected override CompanyContent GetDataModelFromViewModel(ContentModel viewModel)
        {
            return new CompanyContent(viewModel);
        }

        protected override bool ValidateOwner(int ownerId)
        {
            var companyData = (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                               where m.ID == ownerId
                               select m).FirstOrDefault();

            return (companyData != null);
        }


        #endregion


    }
}