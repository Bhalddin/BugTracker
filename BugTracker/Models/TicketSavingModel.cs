using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketSavingModel
    {

        // NOT NULLS on the database.
        // Required is used here to check User input.
        [Required(ErrorMessage="Must select a project the bug relates to.")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage="Must enter a description")]
        [RegularExpression(@"^\W*(\w\W*){50}.*",ErrorMessage="Please add more detail to your description. ex: where, what, why, when, etc...")]
        public string Description { get; set; }

        [Required(ErrorMessage="Must select a ticket type")]
        [Display(Name="Type of ticket")]
        public int Ticket_TypeID { get; set; }


        // -- props to be automatically generated(initially) --
        [Display(Name="User Submitting ticket")]
        public int Ticket_SubmitterID { get; set; }

        // Props made by SQLServer??
        public int ID { get; set; }

        [Display(Name="Date Submitted")]
        public System.DateTime Created_Date { get; set; }

        [Display(Name="Date last updated")]
        public System.DateTime Date_Last_Updated { get; set; }

        // auto's that can be hand altered.
        [Display(Name="")]
        public Nullable<int> Ticket_PriorityID { get; set; }

        [Display(Name = "")]
        public Nullable<int> Ticket_StatusID { get; set; }


        // Nullables
        [Display(Name="Developer to tackle ticket")]
        public Nullable<int> Assigned_ToID { get; set; }

        [Display(Name="Related ticket")]
        public Nullable<int> Related_TicketID { get; set; }

        public string Resolution { get; set; }

    }
}