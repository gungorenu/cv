using System.ComponentModel.DataAnnotations;

namespace CV.Web.Models
{
    public class LogOnModel
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}