using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class RoleViewModel
    {
        // to easily list all the roles

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }


    public class UserRoleViewModel
    {
        // to easily list all the users in(or not) a role

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        [Required]
        public string RoleId { get; set; }

        [Display(Name = "Users")]
        public System.Web.Mvc.MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; } // this is auto-populated from the above field.
    }
}