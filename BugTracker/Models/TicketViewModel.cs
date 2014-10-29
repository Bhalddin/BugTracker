using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Reflection;

namespace BugTracker.Models
{
    public class TicketViewModel
    {

        // Make ID fields REQUIRED! the form will create the view for these, so they will look good.

        #region id fields
        // NOT NULLS on the database.
        // Required is used here to check User input.
        [Required(ErrorMessage = "Must select a project the bug relates to.")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Must select a ticket type")]
        [Display(Name = "Type of ticket")]
        public int TicketTypeID { get; set; }

        // -- props to be automatically generated(initially) --
        [Display(Name = "Ticket submitter")]
        public int TicketSubmitterID { get; set; }

        // Props made by SQLServer??
        public int ID { get; set; }

        // auto's that can be hand altered.
        [Display(Name = "Priority Level")]
        public Nullable<int> TicketPriorityID { get; set; }

        [Display(Name = "Current status")]
        public Nullable<int> TicketStatusID { get; set; }


        // Nullables
        [Display(Name = "Assigned Developer")]
        public Nullable<int> AssignedToID { get; set; }

        [Display(Name = "Related ticket")]
        public Nullable<int> RelatedTicketID { get; set; }
        #endregion

        #region data fields

        [Required(ErrorMessage = "Must have a title.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Must enter a description")]
        [RegularExpression(@"^\W*(\w\W*){50}.*", ErrorMessage = "Please add more detail to your description. ex: where, what, why, when, etc...")]
        public string Description { get; set; }

        public string Resolution { get; set; }

        [Display(Name = "Date submitted")]
        public System.DateTime CreatedDate { get; set; }

        [Display(Name = "Date last updated")]
        public System.DateTime DateLastUpdated { get; set; }

        #endregion

        #region data from other tables

        // custom view only properties...
        public int DayAge { get; set; }

        public virtual Project Project { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }

        // Viewable ONLY in the DETAILS page.
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }

        public virtual ICollection<Ticket> Tickets1 { get; set; }
        public virtual Ticket Ticket1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }

        #endregion



        // func to help me chose the correct value for my toSearchObj function.
        Func<string, string, int, int?, int?> Chose = 
            (_prop, _check, _value, _default) => 
                (_prop == _check) ? _value : 
                (_default != 0) ? _default : null;


        // Method to create an anonymous object to be passed as the search values in a query string.
        public object ToSearchObj(string prop, int value)
        {
           // create an anonymous object. If a value is null it won't be passed back in the query string, which is what we want
            var searchObject = new 
            {
                ID = Chose(prop, "ID", value, this.ID),
                TicketPriorityID = Chose(prop, "TicketPriorityID", value, this.TicketPriorityID),
                TicketStatusID = Chose(prop, "TicketStatusID", value, this.TicketStatusID),
                AssignedToID = Chose(prop, "AssignedToID", value, this.AssignedToID),
                ProjectID = Chose(prop, "ProjectID", value, this.ProjectID),
                TicketSubmitterID = Chose(prop, "TicketSubmitterID", value, this.TicketSubmitterID),
                TicketTypeID = Chose(prop, "TicketTypeID", value, this.TicketTypeID)
            };

            return searchObject;
        }

        internal void Decode()
        {
            throw new NotImplementedException();
        }
    }
}