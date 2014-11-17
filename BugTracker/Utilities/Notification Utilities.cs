using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.Utilities
{
    public static class NotificationUtilities
    {
        #region Badge helpers

        // returns the number of Notifications not read yet.
        public static string NotifyBadgeCount(this System.Security.Principal.IPrincipal user)
        {
            using (var db = new BugTrackerEntities())
            {
                int userId = user.GetID();
                string count = db.Notifications
                                    .Count(n => n.ToID == userId && n.BeenRead == false)
                                    .ToString();

                // add a "+" if it's greater than 1.s
                string badge = (count != "0") ? "+" + count : count;

                return badge; 
            }
        }

        #endregion

    }
}