using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        // to be used in the Create and Add/Remove Users
        [Required]
        public int ID { get; set; }
        [Required(ErrorMessage="Must enter a project name.")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage="Must enter a project description.")]
        public string ProjectDescription { get; set; }

        // needed for the add/remove users.

        [Display(Name = "Users")]
        public System.Web.Mvc.MultiSelectList Users { get; set; }
        [Required(ErrorMessage="List cannot be empty")]
        public int[] SelectedUsers { get; set; } // this is auto-populated from the above field.
    }
}