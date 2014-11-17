using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectCreateViewModel
    {
        // to be used in the Create and Add/Remove Users

        public int ID { get; set; }
        [Required(ErrorMessage="Must enter a project name.")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage="Must enter a project description.")]
        public string ProjectDescription { get; set; }
    }
}