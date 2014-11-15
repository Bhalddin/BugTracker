using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;

namespace BugTracker.Utilities
{

    // Class to Hold all of the History Utility Methods.
    public static class HistoryUtilities
    {

        // function to build a history statement as html code.
        public static string HistoryMessage(string prop, string oldVal, string newVal)
        {
            // need to make sure that values are html encoded BEFORE they get put into this!!
            return String.Format(
                "<b>{0}</b> changed from <b>{1}</b> to <b>{2}</b><br />",
                prop,
                new HtmlString(oldVal),
                new HtmlString(newVal) 
            );
        }



        // Main function to set up
        public static void UpdateTicketAndLog(Ticket newTicket, int editorID)
        {
            // it's BETTER to create a NEW instance of the db than to use an old copy that.
            // for example: if the 'NewTicket' was first retrieved from the db, and then passed in here, if I had used the same
            // db context to grab the 'OldTicket' it would really be grabing my modified new ticket.s
            using (var db = new BugTrackerEntities())
            {
                // Ticket Should already be valid by the time it reaches here.
                // get the original ticket and store it.
                Ticket oldTicket = db.Tickets.AsNoTracking()
                                .Include(t => t.Project)
                                .Include(t => t.TicketPriority)
                                .Include(t => t.TicketStatus)
                                .Include(t => t.TicketType)
                                .Include(t => t.User1) // dev.
                                .Single(t => t.ID == newTicket.ID);


                // Save the New Ticket
                db.Entry(newTicket).State = EntityState.Modified;

                db.SaveChanges();


                //load the new ticket with all of it's potentially new props.
                newTicket = db.Tickets
                                 .Include(t => t.Project)
                                 .Include(t => t.TicketPriority)
                                 .Include(t => t.TicketStatus)
                                 .Include(t => t.TicketType)
                                 .Include(t => t.User1) // dev.
                                 .Single(t => t.ID == newTicket.ID);

                // create History log variable
                string historyInnerHTML = null;

                // LONG list of checks.
                // so we are using the id's that are in the ticket BECAUSE if the id isn't there, the property won't be there,
                // and things get ugly when the property is not there!

                // b/c developer can be null, we need to go over all possible cases.
                if (oldTicket.AssignedToID != newTicket.AssignedToID)
                {
                    if (oldTicket.AssignedToID == null)
                        historyInnerHTML += HistoryMessage("Developer", "Unassigned", newTicket.User1.ASPUserName);

                    else if (newTicket.AssignedToID == null)
                        historyInnerHTML += HistoryMessage("Developer", oldTicket.User1.ASPUserName, "Unassigned");
                    
                    else
                        historyInnerHTML += HistoryMessage("Developer", oldTicket.User1.ASPUserName, newTicket.User1.ASPUserName);
                }

                if (oldTicket.ProjectID != newTicket.ProjectID)
                    historyInnerHTML += HistoryMessage("Project", oldTicket.Project.ProjectName, newTicket.Project.ProjectName);

                if (oldTicket.TicketPriorityID != newTicket.TicketPriorityID)
                    historyInnerHTML += HistoryMessage("Priority", oldTicket.TicketPriority.Priority, newTicket.TicketPriority.Priority);

                if (oldTicket.TicketStatusID != newTicket.TicketStatusID)
                    historyInnerHTML += HistoryMessage("Ticket Status", oldTicket.TicketStatus.Status, newTicket.TicketStatus.Status);

                if (oldTicket.TicketTypeID != newTicket.TicketTypeID)
                    historyInnerHTML += HistoryMessage("Ticket Type", oldTicket.TicketType.Type, newTicket.TicketType.Type);

                if (oldTicket.Title != newTicket.Title)
                    historyInnerHTML += HistoryMessage("Title", oldTicket.Title, newTicket.Title);

                if (oldTicket.Description != newTicket.Description)
                    historyInnerHTML += HistoryMessage("Description", oldTicket.Description, newTicket.Description);

                // b/c resolution can be null, check for all cases.
                if (oldTicket.Resolution != newTicket.Resolution)
                {
                    if (oldTicket.Resolution == null || oldTicket.Resolution == "")
                        historyInnerHTML += HistoryMessage("Resolution", "Unresolved", newTicket.Resolution);
                   
                    else
                        historyInnerHTML += HistoryMessage("Resolution", oldTicket.Resolution, newTicket.Resolution);
                }


                // should catch the case that nothing is updated.
                if (historyInnerHTML == null)
                {
                    return;
                }


                // create and save the history.
                db.TicketHistories.Add(new TicketHistory
                {
                    TicketID = newTicket.ID,
                    DateOfChange = DateTime.UtcNow,
                    TicketEditorID = editorID,
                    Ticket_Alteration = historyInnerHTML
                });

                db.SaveChanges();
            }

        }


        // function to just create initial history of ticket.
    }
}