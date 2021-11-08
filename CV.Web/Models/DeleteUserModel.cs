using System.ComponentModel.DataAnnotations;

namespace CV.Web.Models
{
    public class DeleteUserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }
}