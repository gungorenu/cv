using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace CV.Web.Models
{
    public class CompanyModel
    {
        public int CompanyID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Positions")]
        public string Positions { get; set; }

        [Display(Name = "Link")]
        public string Link { get; set; }

        public CompanyModel()
        {
            CompanyID = 0;
        }
    }
}