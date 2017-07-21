using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.Linq.Dynamic;
using PagedList;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using BugTracker.Utilities;

namespace BugTracker.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();


        #region Partials
        //[OutputCache(Duration=60, VaryByParam="*")] // Slight difference by User, so I need to cache by user somehow as well.
        /// <summary>
        /// Returns a partialView, a table of filtered Tickets.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult TicketTable(
            [Bind(Include = "TicketSubmitterID,AssignedToID,ProjectID,TicketPriorityID,TicketStatusID,TicketTypeID,CreatedDate")]TicketViewModel filters,
            string sort,
            string orderAscending,
            string textSearchField = "",
            string textSearchValue = "",
            int page = 1,
            int pageSize = 10
        )
        {
            // Only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            // Save filter paramaters in viewbag.
            // Saving of inputs is happening before checking of inputs b/c we want to save nulls if thats what we are given, but we cannot use nulls.
            ViewBag.CurrentFilters = filters;
            ViewBag.Sort = sort;
            ViewBag.OrderA = orderAscending;


            // Check your inputs!
            //(b/c our inputs are mostly numbers, which won't have spaces to convert,we don't have to check those inputs)
            orderAscending = orderAscending ?? "true"; // default is set to be true.
            sort = (sort == null || sort == "") ? "false" : sort;
            string[] SearchArray = textSearchValue.Split();

            // get the tickets with all of their data.
            var tickets = db.Tickets
                                .Include(t => t.Project)
                                .Include(t => t.TicketPriority)
                                .Include(t => t.TicketStatus)
                                .Include(t => t.TicketType)
                                //.Include(t => t.Ticket1)
                                .Include(t => t.User)
                                .Include(t => t.User1);


            // add filtering to tickets.
            tickets = tickets
                        .Where(t => filters.TicketPriorityID == null || t.TicketPriorityID == filters.TicketPriorityID)
                        .Where(t => filters.TicketStatusID == null || t.TicketStatusID == filters.TicketStatusID)
                        .Where(t => filters.TicketTypeID == null || t.TicketTypeID == filters.TicketTypeID)
                        .Where(t => filters.ProjectID == null || t.ProjectID == filters.ProjectID)
                        .Where(t => filters.TicketSubmitterID == null || t.TicketSubmitterID == filters.TicketSubmitterID)
                        .Where(t => filters.AssignedToID == null || t.AssignedToID == filters.AssignedToID)
                        .Where(t => t.CreatedDate.CompareTo(filters.CreatedDate) >= 0)
                        .Where(t => textSearchValue == ""
                                    || (textSearchField == "Title" && SearchArray.All(word => t.Title.Contains(word)))
                                    || (textSearchField == "Description" && SearchArray.All(word => t.Description.Contains(word))));

            // apply sorting only if we need to.
            // by default it should be ascending, ONLY when we are passed false should it be descending.
            if (sort != "false")
            {
                if (orderAscending == "false") // sort descending if orderA is false.
                {
                    return PartialView("_TicketTable", tickets.OrderBy(sort).ToPagedList(page, pageSize));
                }
                else // sort ascending otherwise.
                {
                    var reversedTickets = tickets.OrderBy(sort).ToList();
                    reversedTickets.Reverse();

                    return PartialView("_TicketTable", reversedTickets.ToPagedList(page, pageSize));
                }
            }

            // no sorting.
            return PartialView("_TicketTable", tickets.OrderBy(t => false).ToPagedList(page, pageSize));
        }


        //[OutputCache(Duration = 3600)]
        /// <summary>
        /// Return partialView, select elements that are filtered according to other inputs.
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketFilterForm(int? statusId, int? typeId, int? priorityId, int? submitterId, int? devId, int? projectId)
        {

            // Check to only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
            {
                return View("Error");
            }

            // Creating all of the Select Dropdowns.
            ViewBag.Statuses = new SelectList(db.TicketStatuses, "ID", "Status", statusId);
            ViewBag.Types = new SelectList(db.TicketTypes, "ID", "Type", typeId);
            ViewBag.Priorities = new SelectList(db.TicketPriorities, "ID", "Priority", priorityId);
            ViewBag.Projects = new SelectList(db.Projects, "ID", "ProjectName", projectId);

            // getting only the Developers
            var aspUserDb = new ApplicationDbContext();
            string developerRoleID = aspUserDb.Roles.Single(r => r.Name == "Developer").Id;
            var listOfDevNames = aspUserDb.Users.Where(u => u.Roles.Any(r => r.RoleId == developerRoleID)).Select(u => u.UserName).ToList();
            var listOfDevelopers = db.Users.Where(u => listOfDevNames.Any(n => n == u.ASPUserName)).OrderByDescending(u => u.ASPUserName);

            ViewBag.AssignedDevs = new SelectList(listOfDevelopers, "ID", "ASPUserName", devId);

            // and finally all users for the submit list.
            ViewBag.TicketSubmitters =
                new SelectList(db.Users.Select(u => new { u.ID, u.ASPUserName }).OrderByDescending(u => u.ASPUserName), "ID", "ASPUserName", submitterId);

            return PartialView("_TicketFilterForm");
        }

        #endregion


        #region Index -  A.K.A. the Ticket Search page
        // GET: Tickets
        public ActionResult Index()
        {
            // list of fields to perform a custom text search on.
            List<SelectListItem> textSearchFields = new List<SelectListItem> {
                new SelectListItem{Text="Title",Value="Title"},
                new SelectListItem{Text="Description",Value="Description"},
            };

            // Select list FOR the custom text search box.
            ViewBag.TextSearchFields = new SelectList(textSearchFields, "Value", "Text", "Title");

            return View();
        }
        #endregion


        #region personal Ticket page
        // return a page to view all of your tickets.
        // this page is asy
        public ActionResult SubmittedTickets()
        {
            // get the user id
            int userID = User.GetID();

            // get tickets that they have submitted
            var theirTickets = db.Tickets
                                    .Where(t => t.TicketSubmitterID == userID)
                                    .OrderByDescending(t => t.ID);
            //.ToPagedList(1, 100); // need to think more about whether to page or not.

            // return a list view of their tickets.
            return View(theirTickets);
        }


        // return a page to view all of your tickets.
        public ActionResult WorkingTickets()
        {
            // get the user id
            int userID = User.GetID();

            // get tickets that they have submitted
            var theirTickets = db.Tickets
                                    .Include(t => t.TicketStatus)
                                    .Where(t => t.AssignedToID == userID)
                                    .OrderByDescending(t => t.ID);
            //.ToPagedList(1, 100); // need to think more about whether to page or not.

            // Tickets with Waiting Notifications.
            ViewBag.WaitingTickets = db.Notifications
                                        .Include(n => n.Ticket.TicketStatus)
                                        .Where(n => !n.BeenRead && n.ToID == userID)
                                        .Select(n => n.Ticket)
                                        .Distinct();


            // return a list view of their tickets.
            return View(theirTickets);
        }

        // action to return a list of Tickets with waiting notifications
        public ActionResult NewNotifications()
        {
            // get the user id
            int userID = User.GetID();

            // Tickets with Waiting Notifications.
            var awaitingTickets = db.Notifications
                                        .Include(n => n.Ticket.TicketStatus)
                                        .Where(n => !n.BeenRead && n.ToID == userID)
                                        .Select(n => n.Ticket)
                                        .Distinct();

            // if this is a child action, return a partial
            if (ControllerContext.IsChildAction)
            {
                return PartialView("_PersonalTicketTable", awaitingTickets);
            }

            return View(awaitingTickets);
        }



        #endregion


        #region Details
        // GET: Tickets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket;

            // if this Details action is a child action, then we will have a problem b/c it's async.
            // and as far as i see a child action can't? be async.
            // but it becomes syncronous if we just remove the await sections.
            if (!ControllerContext.IsChildAction)
            {
                ticket = await db.Tickets
                                    .Include(t => t.TicketStatus)
                                    .Include(t => t.TicketType)
                                    .Include(t => t.TicketPriority)
                                    .SingleOrDefaultAsync(t => t.ID == id);
            }
            else
            {
                ticket = db.Tickets
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.TicketPriority)
                    .Single(t => t.ID == id);
            }

            // if ticket not return bad request
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(ticket);
        }

        #endregion


        #region Create
        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName");
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type");
            //ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description");

            ViewBag.TicketTitle = "";
            ViewBag.Description = "";
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectID,Title,Description,TicketTypeID,RelatedTicketID")] TicketViewModel ticketVM)
        {
            if (ModelState.IsValid)
            {
                // get user name from cookie.
                var userName = HttpContext.User.Identity.Name;
                ticketVM.TicketSubmitterID = db.Users.Single(u => u.ASPUserName == userName).ID;

                // create ticket with default values to save.
                Ticket ticket = new Ticket(ticketVM);

                db.Tickets.Add(ticket);
                db.SaveChanges();

                // Ticket is made, now is time to add attachments to the database.
                foreach (string key in Request.Files)
                {
                    // make sure that key is not to an empty file
                    if (Request.Files[key].ContentLength > 0)
                    {
                        string currentDesc = Request.Form[key];
                        string serverFolderPath = Server.MapPath("~/App_Data/Attachments/");
                        TicketAttachment.SaveAsAttachment(Request.Files[key], serverFolderPath, db, ticket.ID, ticket.TicketSubmitterID, currentDesc);
                    }
                }

                return RedirectToAction("Index");
            }


            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName", ticketVM.ProjectID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticketVM.TicketTypeID);
            //ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticketVM.RelatedTicketID);

            ViewBag.TicketTitle = ticketVM.Title;
            ViewBag.Description = ticketVM.Description;

            return View(ticketVM);
        }

        #endregion


        #region Edit
        // GET: Tickets/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Edit(int? id)
        {
            // check your inputs.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = await db.Tickets.Include(t => t.Ticket1).SingleAsync(t => t.ID == id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "ID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Title", ticket.RelatedTicketID);

            // fix the view to display info on the tickets using the ticket1 item.
            // and disable fields that shouldn't be edited.
            var aspUserDb = new ApplicationDbContext();
            var developerRole = aspUserDb.Roles.Single(r => r.Name == "Developer");
            var listOfDevNames = aspUserDb.Users.Where(u => u.Roles.Any(r => r.RoleId == developerRole.Id)).Select(u => u.UserName).ToList();
            var allDevelopers = db.Users.Where(u => listOfDevNames.Any(d => d == u.ASPUserName));

            // Limit Items in the Submitter to just the submitter!
            ViewBag.TicketSubmitterID = new SelectList(db.Users.Where(u => u.ID == ticket.TicketSubmitterID), "ID", "ASPUserName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(allDevelopers, "ID", "ASPUserName", ticket.AssignedToID);
            return View(ticket);
        }



        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketViewModel ticketVM)
        {
            // create ticket from ticketViewmodel
            Ticket ticket = new Ticket(ticketVM);

            // check your inputs
            if (ModelState.IsValid)
            {
                //db.Entry(ticket).State = EntityState.Modified;
                //await db.SaveChangesAsync();

                var currentUser = HttpContext.User.Identity.Name;
                var userID = db.Users.Single(u => u.ASPUserName == currentUser).ID;

                // save and update history
                HistoryUtilities.UpdateTicketAndLog(ticket, userID);


                return RedirectToAction("Details", new { id = ticket.ID });
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "ID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Title", ticket.RelatedTicketID);

            // fix the view to display info on the tickets using the ticket1 item.
            // and disable fields that shouldn't be edited.
            var aspUserDb = new ApplicationDbContext();
            var developerRole = aspUserDb.Roles.Single(r => r.Name == "Developer");
            var listOfDevNames = aspUserDb.Users.Where(u => u.Roles.Any(r => r.RoleId == developerRole.Id)).Select(u => u.UserName).ToList();
            var allDevelopers = db.Users.Where(u => listOfDevNames.Any(d => d == u.ASPUserName));

            // Limit Items in the Submitter to just the submitter!
            ViewBag.TicketSubmitterID = new SelectList(db.Users.Where(u => u.ID == ticket.TicketSubmitterID), "ID", "ASPUserName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(allDevelopers, "ID", "ASPUserName", ticket.AssignedToID);
            return View(ticket);
        }
        #endregion


        #region Add Resolution
        // action so that a developer can add a resolution to a ticket, even though they can't add anything else.
        [Authorize(Roles = "Administrator,Developer")]
        public ActionResult AddResolution(int? id)
        {
            // check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // -- should only allow the developer that's assigned to the ticket to get here. --

            var isAdmin = User.IsInRole("Administrator");
            var workingDeveloper = false;

            // the developer chould be unassigned, which is a case we should check for.
            var ticket = db.Tickets.Find(id);
            if (ticket.AssignedToID != null)
            {
                workingDeveloper = ticket.User1.ASPUserName == HttpContext.User.Identity.Name;
            }

            // only let an admin or the working developer create a resolution.
            if (isAdmin || workingDeveloper)
            {
                // return page that lets you add a resolution.
                ViewBag.TicketID = id;

                return View();
            }

            // if the caller reaches this point then they are NOT AUTHORIZED!
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }


        // action to save the resolution to the db and to update the history.
        [HttpPost]
        [Authorize(Roles = "Administrator,Developer")]
        [ValidateAntiForgeryToken]
        public ActionResult AddResolution(ResolutionViewModel resolutionVM)
        {
            if (ModelState.IsValid)
            {
                // get the backing ticket
                var ticket = db.Tickets.AsNoTracking().Single(t => t.ID == resolutionVM.TicketID);

                // if ticket already has a resolution then do nothing and redirect to details
                var HasResolution = !(ticket.Resolution == null || ticket.Resolution == "");
                if (HasResolution)
                {
                    return RedirectToAction("Details", new { id = resolutionVM.TicketID });
                }

                // get the user's id for the history
                var userName = HttpContext.User.Identity.Name;
                var userID = db.Users.Single(u => u.ASPUserName == userName).ID;

                // this should add the resolutino and update the status to RESOLVED.
                ticket.Resolution = resolutionVM.resolutionText;
                ticket.TicketStatusID = db.TicketStatuses.Single(s => s.Status == "Resolved").ID;


                // update ticket and history
                HistoryUtilities.UpdateTicketAndLog(ticket, userID);

                // return to the details page of the ticket
                return RedirectToAction("Details", new { id = resolutionVM.TicketID });
            }

            // model wasn't valid, return it for fixing.
            ViewBag.TicketID = resolutionVM.TicketID;
            return View(resolutionVM);

        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
