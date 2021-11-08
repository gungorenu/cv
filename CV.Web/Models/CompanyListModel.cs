using CV.DataLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class CompanyListModel : IEnumerable<CompanyModel>
    {
        private IEnumerable<CompanyModel> GetResults()
        {
            var results = from c in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                          where c.Name != DataLayerConstants.PersonalProjectCompany
                          orderby c.StartYear descending 
                          select new CompanyModel() { CompanyID = c.ID, Name = c.Name, Link = c.Link, Positions = c.Positions, StartDate = c.StartYear, EndDate = c.EndYear };

            return results;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetResults().GetEnumerator();
        }

        IEnumerator<CompanyModel> IEnumerable<CompanyModel>.GetEnumerator()
        {
            return GetResults().GetEnumerator();
        }
    }
}