using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ResolutionViewModel
    {
        [Required]
        public int TicketID { get; set; }

        [Required]
        [Display(Name="Resolution")]
        public string resolutionText { get; set; }
    }
}