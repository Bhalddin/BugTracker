using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketViewingModel
    {
        // I will need the ID of a ticket to help find things later.
        public int ID { get; set; }

        // to do sorting on a field, I will need the...value! I will just look it up and THEN query that id.
        public int ProjectID { get; set; }
        public int Ticket_TypeID { get; set; }
        // Nullables
        [Display(Name = "Developer working on it")]
        public Nullable<int> Assigned_ToID { get; set; }

        public Nullable<int> Related_TicketID { get; set; }
        

        [Display(Name = "Description of ticket")]
        public string Description { get; set; }

        [Display(Name = "Date created")]
        public System.DateTime Created_Date { get; set; }

        [Display(Name = "Date last updated")]
        public System.DateTime Date_Last_Updated { get; set; }

        [Display(Name = "Priority")]
        public Nullable<int> Ticket_PriorityID { get; set; }

        [Display(Name = "Current Status")]
        public Nullable<int> Ticket_StatusID { get; set; }

        public string Resolution { get; set; }


        // custom view only properties...
        public TimeSpan DayAge { get; set; }



        public string _Project { get; set; }
        public int _NumberOfAttachments { get; set; }
        public int _LastComment { get; set; } //???
        public int MyProperty { get; set; }


        // Viewable ONLY in the DETAILS page.
        public int Ticket_SubmitterID { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Ticket_Attachments> Ticket_Attachments { get; set; }
        public virtual ICollection<Ticket_History> Ticket_History { get; set; }

        // extra properties from other tables --- probably need abreviated versions of these objects.

        public virtual Project Project { get; set; }
        public virtual Ticket_Priorities Ticket_Priorities { get; set; }
        public virtual Ticket_Statuses Ticket_Statuses { get; set; }
        public virtual Ticket_Types Ticket_Types { get; set; }
        public virtual ICollection<Ticket> Tickets1 { get; set; }
        public virtual Ticket Ticket1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}