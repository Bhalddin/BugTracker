using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public partial class Ticket
    {

        public Ticket(TicketViewModel ticketVM)
        {
            // if I pass in a VM with no id, this this is a new ticket and should get defaults.
            if (ticketVM.ID != null)
            {
                this.ID = (int)ticketVM.ID;
                this.CreatedDate = ticketVM.CreatedDate;
                this.DateLastUpdated = ticketVM.DateLastUpdated;
                this.TicketPriorityID = (int)ticketVM.TicketPriorityID;
                this.TicketStatusID = (int)ticketVM.TicketStatusID;
                this.TicketTypeID = (int)ticketVM.TicketTypeID;
            }
            else
            {
                // default new ticket settings.
                this.CreatedDate = DateTime.UtcNow;
                this.DateLastUpdated = DateTime.UtcNow;
                this.TicketPriorityID = 1; // undefined
                this.TicketStatusID = 1; // new
                this.TicketTypeID = ticketVM.TicketTypeID ?? 1; // undefined
            }

            this.AssignedToID = ticketVM.AssignedToID;

            this.TicketSubmitterID = (int)ticketVM.TicketSubmitterID;
            this.ProjectID = (int)ticketVM.ProjectID;
            this.Title = ticketVM.Title;
            this.Description = ticketVM.Description;
            this.Resolution = ticketVM.Resolution;
            this.RelatedTicketID = ticketVM.RelatedTicketID;
        }

        //// func to help me chose the correct value for my toSearchObj function.
        //Func<string, string, int, int?, int?> Chose =
        //    (_prop, _check, _value, _default) =>
        //        (_prop == _check) ? _value :
        //        (_default != 0) ? _default : null;


        //// Method to create an anonymous object to be passed as the search values in a query string.
        //public object ToSearchObj(string prop, int value, string sort, string orderA)
        //{
        //    // create an anonymous object. If a value is null it won't be passed back in the query string, which is what we want
        //    var searchObject = new
        //    {
        //        ID = Chose(prop, "ID", value, this.ID),
        //        TicketPriorityID = Chose(prop, "TicketPriorityID", value, this.TicketPriorityID),
        //        TicketStatusID = Chose(prop, "TicketStatusID", value, this.TicketStatusID),
        //        AssignedToID = Chose(prop, "AssignedToID", value, this.AssignedToID),
        //        ProjectID = Chose(prop, "ProjectID", value, this.ProjectID),
        //        TicketSubmitterID = Chose(prop, "TicketSubmitterID", value, this.TicketSubmitterID),
        //        TicketTypeID = Chose(prop, "TicketTypeID", value, this.TicketTypeID),
        //        sort = sort,
        //        orderAscending = orderA
        //    };

        //    return searchObject;
        //}
    }
}