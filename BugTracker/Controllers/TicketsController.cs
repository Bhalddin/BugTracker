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

namespace BugTracker.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        // --- HELPER FUNCTIONS ---

        // Check to only allow ajax and child actions.
        //Func<HttpRequestBase, bool> NotPartialRequest = (_request) => !(_request.IsAjaxRequest() || ControllerContext.IsChildAction);



        /// <summary>
        /// Returns a partialView, a table of filtered Tickets.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult TicketTable([Bind(Include = "CreatedDate,TicketSubmitterID,AssignedToID,ProjectID,TicketPriorityID,TicketStatusID,TicketTypeID,RelatedTicketID,DateLastUpdated")]TicketViewModel search, int page = 1, int pageSize = 10)
        {
            // was worrying about how the controller was going to pass the paramaters from the starting action to this action but the paramaters will always be given explicit from ajax call.

            // Check to only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
                return View("Error");


            // Check your inputs!!
            // 

            var tickets = db.Tickets
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Ticket1)
                .Include(t => t.User)
                .Include(t => t.User1);

            // FIGURE OUT HOW TO DO A DYNAMIC SEARCH.

            // select everything to a the ViewModel
            var model = tickets.Select(t => new TicketViewModel
            {
                ID = t.ID,
                User = t.User,
                User1 = t.User1,
                Comments = t.Comments,
                CreatedDate = t.CreatedDate,
                DateLastUpdated = (DateTime)t.DateLastUpdated,
                Description = t.Description,
                Notifications = t.Notifications,
                Project = t.Project,
                Resolution = t.Resolution,
                Ticket1 = t.Ticket1,
                TicketPriority = t.TicketPriority,
                TicketStatus = t.TicketStatus,
                Title = t.Title,
                TicketType = t.TicketType
            });


            return PartialView("_TicketTable", model.ToList());
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





        // GET: Tickets
        public ActionResult Index()
        {
            //var tickets = db.Tickets
            //    .Include(t => t.Project)
            //    .Include(t => t.TicketPriority)
            //    .Include(t => t.TicketStatus)
            //    .Include(t => t.TicketType)
            //    .Include(t => t.Ticket1)
            //    .Include(t => t.User)
            //    .Include(t => t.User1);

            //return View(await tickets.ToListAsync());
            return View();
        }

        // GET: Tickets/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Project_Name");
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority");
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status");
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type");
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description");
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName");
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CreatedDate,TicketSubmitterID,AssignedToID,ProjectID,Description,Resolution,TicketPriorityID,TicketStatusID,TicketTypeID,RelatedTicketID,DateLastUpdated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Project_Name", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticket.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName", ticket.AssignedToID);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Project_Name", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticket.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName", ticket.AssignedToID);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CreatedDate,TicketSubmitterID,AssignedToID,ProjectID,Description,Resolution,TicketPriorityID,TicketStatusID,TicketTypeID,RelatedTicketID,DateLastUpdated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Project_Name", ticket.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "ID", "Priority", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "StatusID", "Status", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "ID", "Type", ticket.TicketTypeID);
            ViewBag.RelatedTicketID = new SelectList(db.Tickets, "ID", "Description", ticket.RelatedTicketID);
            ViewBag.TicketSubmitterID = new SelectList(db.Users, "ID", "FirstName", ticket.TicketSubmitterID);
            ViewBag.AssignedToID = new SelectList(db.Users, "ID", "FirstName", ticket.AssignedToID);
            return View(ticket);
        }

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
