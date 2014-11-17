using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class CommentViewModel
    {

        [Required]
        public int ID { get; set; }

        [Required]
        public int TicketID { get; set; }

        [Required]
        public int CommentorID { get; set; }

        [Required]
        public System.DateTime CommentDate { get; set; }

        [Required(ErrorMessage="You must enter a comment.")]
        public string Comment1 { get; set; }


        public virtual Ticket Ticket { get; set; }
        public virtual User User { get; set; }


        // easily convert commentVM to comment.
        public Comment ToComment()
        {
            var com = new Comment
            {
                ID = this.ID,
                CommentDate = this.CommentDate,
                TicketID = this.TicketID,
                CommentorID = this.CommentorID,
                Comment1 = this.Comment1
            };

            return com;
        }
    }
}