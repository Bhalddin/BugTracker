using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BugTracker.Utilities
{
    public static class TicketUtilities
    {
        #region Badge Extension methods
        // Extension method to easily get the count of the tickets a user has submitter.
        // used for badges.
        public static int MySubmittedTicketCount(this System.Security.Principal.IPrincipal user)
        {
            using (var db = new BugTrackerEntities())
            {
                int userID = user.GetID();
                int myTickets = db.Tickets.Count(t => t.TicketSubmitterID == userID);

                return myTickets;
            }
        }


        // Extension method to easily get the count of the tickets a dev is working on.
        // used for badges.
        public static int MyWorkingTicketCount(this System.Security.Principal.IPrincipal user)
        {
            using (var db = new BugTrackerEntities())
            {
                int userID = user.GetID();
                int myTickets = db.Tickets.Count(t => t.AssignedToID == userID);

                return myTickets;
            }
        }
        #endregion


        // Extension method to easily get the user's id that's in my user table.
        public static int GetID(this System.Security.Principal.IPrincipal user)
        {
            return new BugTrackerEntities().Users.Single(u => u.ASPUserName == user.Identity.Name).ID;
        }

    }
}