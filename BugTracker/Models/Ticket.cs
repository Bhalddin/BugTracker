//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugTracker.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ticket
    {
        public Ticket()
        {
            this.Comments = new HashSet<Comment>();
            this.Notifications = new HashSet<Notification>();
            this.Ticket_Attachments = new HashSet<Ticket_Attachments>();
            this.Ticket_History = new HashSet<Ticket_History>();
            this.Tickets1 = new HashSet<Ticket>();
        }
    
        public int ID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public int Ticket_SubmitterID { get; set; }
        public Nullable<int> Assigned_ToID { get; set; }
        public int ProjectID { get; set; }
        public string Description { get; set; }
        public string Resolution { get; set; }
        public Nullable<int> Ticket_PriorityID { get; set; }
        public Nullable<int> Ticket_StatusID { get; set; }
        public Nullable<int> Ticket_TypeID { get; set; }
        public Nullable<int> Related_TicketID { get; set; }
        public System.DateTime Date_Last_Updated { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Ticket_Attachments> Ticket_Attachments { get; set; }
        public virtual ICollection<Ticket_History> Ticket_History { get; set; }
        public virtual Ticket_Priorities Ticket_Priorities { get; set; }
        public virtual Ticket_Statuses Ticket_Statuses { get; set; }
        public virtual Ticket_Types Ticket_Types { get; set; }
        public virtual ICollection<Ticket> Tickets1 { get; set; }
        public virtual Ticket Ticket1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
