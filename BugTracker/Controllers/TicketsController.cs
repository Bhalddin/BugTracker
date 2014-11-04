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

namespace BugTracker.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        // --- HELPER FUNCTIONS ---

        // Check to only allow ajax and child actions.
        //Func<HttpRequestBase, bool> NotPartialRequest = (_request) => !(_request.IsAjaxRequest() || ControllerContext.IsChildAction);


        #region Partials
        /// <summary>
        /// Returns a partialView, a table of filtered Tickets.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult TicketTable(
            [Bind(Include = "TicketSubmitterID,AssignedToID,ProjectID,TicketPriorityID,TicketStatusID,TicketTypeID,RelatedTicketID")]TicketViewModel filters,
            string sort,
            string orderAscending,
            int page = 1,
            int pageSize = 10
        )
        {
            // Only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
                return View("Error");


            // Save filter paramaters in viewbag.
            // Saving of inputs is happening before checking of inputs b/c we want to save nulls if thats what we are given, but we cannot use nulls.
            ViewBag.CurrentFilters = filters;
            ViewBag.Sort = sort;
            ViewBag.OrderA = orderAscending;


            // Check your inputs!
            //(b/c our inputs are mostly numbers, which won't have spaces to convert,we don't have to check those inputs)
            orderAscending = orderAscending ?? "true"; // default is set to be true.
            sort = (sort == null || sort == "") ? "false" : sort;


            // get the tickets with all of their data.
            var tickets = db.Tickets
                                .Include(t => t.Project)
                                .Include(t => t.TicketPriority)
                                .Include(t => t.TicketStatus)
                                .Include(t => t.TicketType)
                                .Include(t => t.Ticket1)
                                .Include(t => t.User)
                                .Include(t => t.User1);

            // add filtering to tickets.
            tickets = tickets
                        .Where(t => filters.ID == null || t.ID == filters.ID)
                        .Where(t => filters.TicketPriorityID == null || t.TicketPriorityID == filters.TicketPriorityID)
                        .Where(t => filters.TicketStatusID == null || t.TicketStatusID == filters.TicketStatusID)
                        .Where(t => filters.AssignedToID == null || t.AssignedToID == filters.AssignedToID)
                        .Where(t => filters.ProjectID == null || t.ProjectID == filters.ProjectID)
                        .Where(t => filters.TicketSubmitterID == null || t.TicketSubmitterID == filters.TicketSubmitterID)
                        .Where(t => filters.TicketTypeID == null || t.TicketTypeID == filters.TicketTypeID);

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



        /// <summary>
        /// Return partialView, select elements that are filtered according to other inputs.
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketFilterForm()
        {

            // Check to only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
                return View("Error");


            // Check your inputs.


            // query the db for the filtered version of the tickets

            return PartialView("_TicketFilterForm");
        }

        #endregion


        #region Index
        // GET: Tickets
        public ActionResult Index()
        {
            return View();
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

            Ticket ticket = await db.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.TicketPriority)
                .SingleOrDefaultAsync(t => t.ID == id);

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
            //ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority");
            //ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "ID", "Status");
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type");
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description");
            //ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName");
            //ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName");

            ViewBag.TicketTitle = "";
            ViewBag.Description = "";
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProjectID,Title,Description,TicketTypeID,RelatedTicketID")] TicketViewModel ticketVM)
        {
            if (ModelState.IsValid)
            {
                // get user name from cookie.
                var userName = HttpContext.User.Identity.Name;
                ticketVM.TicketSubmitterID = db.Users.Single(u => u.ASPUserName == userName).ID;

                // create ticket with default values to save.
                Ticket ticket = new Ticket(ticketVM);


                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            // FIX WHEN YOU HAVE TIME.
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName", ticketVM.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticketVM.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status", ticketVM.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticketVM.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticketVM.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName", ticketVM.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName", ticketVM.AssignedToID);

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

            // fix the view to display info on the tickets using the ticket1 item.
            // and disable fields that shouldn't be edited.
            var aspUserDb = new ApplicationDbContext();
            var developerRole = aspUserDb.Roles.Single(r => r.Name == "Developer");
            var listOfDevelopers = aspUserDb.Users.Where(u => u.Roles.Any(r => r.RoleId == developerRole.Id)).Select(u => u.UserName).ToList();

            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Title", ticket.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users.Where(u => u.ID == ticket.TicketSubmitterID), "ID", "ASPUserName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users.Where(u => listOfDevelopers.Any(d => d == u.ASPUserName)), "ID", "ASPUserName", ticket.AssignedToID);
            return View(ticket);
        }



        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TicketViewModel ticketVM)
        {
            var req = Request.Form;

            // create ticket from ticketViewmodel
            Ticket ticket = new Ticket(ticketVM);

            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = ticket.ID });
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "ProjectName", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticket.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName", ticket.AssignedToID);

            ViewBag.TicketTitle = ticket.Title;
            ViewBag.Description = ticket.Description;

            return View(ticket);
        }
        #endregion


        #region Delete
        // GET: Tickets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ticket ticket = await db.Tickets.FindAsync(id);
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        public ActionResult Comments(int? id)
        {
            //check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.TicketID = id;
            var comments = db.Comments.Where(c => c.TicketID == id);

            return View(comments.ToList());
        }


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
