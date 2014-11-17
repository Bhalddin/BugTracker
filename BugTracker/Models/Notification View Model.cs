using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class NotificationViewModel
    {
        public int ID { get; set; }
        [Required]
        public int TicketID { get; set; }
        [Required]
        public int FromID { get; set; }
        [Required(ErrorMessage="Must select a person to be notified.")]
        public int ToID { get; set; }
        [Required]
        public System.DateTime OnDate { get; set; }
        [Required(ErrorMessage="Notification must have a message.")]
        public string Notification1 { get; set; }
        public bool BeenRead { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}