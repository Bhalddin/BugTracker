using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectUserViewModel
    {
        // need to save to db.
        [Required] // to be safe
        public int ID { get; set; }
        public string ProjectName { get; set; }

        // needed to add/remove users.
        public System.Web.Mvc.MultiSelectList Users { get; set; }
        [Required(ErrorMessage = "List cannot be empty")]
        public int[] SelectedUsers { get; set; } // this is auto-populated from the above field.
    }
}