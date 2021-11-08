using CV.DataLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class ProjectListModel : IEnumerable<ProjectModel>
    {
        public bool PersonalProjects { get; private set; }

        public ProjectListModel(bool personalProjects)
        {
            PersonalProjects = personalProjects;
        }

        private IEnumerable<ProjectModel> GetResults()
        {
            var results = from p in CV.DataLayer.CVDbContext.DatabaseContext.PROJECTS
                          join CV.DataLayer.Company c in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES on p.CompanyId equals c.ID
                          where (c.Name == DataLayerConstants.PersonalProjectCompany) == PersonalProjects
                          orderby p.StartYear descending
                          select 
                          
                          new ProjectModel() { ProjectID = p.ID, Name = p.Name, Link = p.Link, StartDate = p.StartYear, EndDate = p.EndYear, CompanyID = c.ID };

            return results;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetResults().GetEnumerator();
        }

        IEnumerator<ProjectModel> IEnumerable<ProjectModel>.GetEnumerator()
        {
            return GetResults().GetEnumerator();
        }
    }
}