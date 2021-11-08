using CV.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CV.Web.Models
{
    public class ProjectModel
    {
        public int ProjectID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "CompanyID")]
        public int CompanyID { get; set; }

        [DataMember]
        public string Company
        {
            get
            {
                return (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                        where m.ID == CompanyID
                        select m.Name).FirstOrDefault();
            }
        }

        [IgnoreDataMember]
        public bool IsPersonal
        {
            get
            {
                return (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                 where m.ID == this.CompanyID && m.Name == DataLayerConstants.PersonalProjectCompany
                 select 1).Count() > 0;
            }
        }

        public ProjectModel()
        {
            CompanyID = 0;
            ProjectID = 0;
        }

        //[IgnoreDataMember]
        //public IEnumerable<CompanyModel> CommercialCompanies
        //{
        //    get
        //    {
        //        return from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
        //               where m.Name != DataLayerConstants.PersonalProjectCompany
        //               select new CompanyModel() { Name = m.Name, CompanyID = m.ID };
        //    }
        //}
    }
    
}