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
    public class TicketsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();


        // --- HELPER FUNCTIONS ---

        // Check to only allow ajax and child actions.
        //Func<HttpRequestBase, bool> NotPartialRequest = (_request) => !(_request.IsAjaxRequest() || ControllerContext.IsChildAction);



        /// <summary>
        /// Returns a partialView, a table of Tickets.
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketTable()
        {
            // Check to only allow ajax and child actions.
            var notPartialRequest = !(Request.IsAjaxRequest() || ControllerContext.IsChildAction);
            if (notPartialRequest)
                return View("Error");


            // Check your inputs.


            // query the db for the filtered version of the tickets




            //return PartialView("_TicketTable", pagedTickets);
            return PartialView("_TicketTable");
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
        public async Task<ActionResult> Index()
        {
            var tickets = db.Tickets.Include(t => t.Project)
                .Include(t => t.Ticket_Priorities)
                .Include(t => t.Ticket_Statuses)
                .Include(t => t.Ticket_Types)
                .Include(t => t.Ticket1)
                .Include(t => t.User)
                .Include(t => t.User1);

            return View(await tickets.ToListAsync());
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
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Project_Name");
            ViewBag.Ticket_PriorityID = new SelectList(db.Ticket_Priorities, "PriorityID", "Priority");
            ViewBag.Ticket_StatusID = new SelectList(db.Ticket_Statuses, "StatusID", "Status");
            ViewBag.Ticket_TypeID = new SelectList(db.Ticket_Types, "TypeID", "Type");
            ViewBag.Related_TicketID = new SelectList(db.Tickets, "ID", "Description");
            ViewBag.Ticket_SubmitterID = new SelectList(db.Users, "UserID", "First_Name");
            ViewBag.Assigned_ToID = new SelectList(db.Users, "UserID", "First_Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Created_Date,Ticket_SubmitterID,Assigned_ToID,ProjectID,Description,Resolution,Ticket_PriorityID,Ticket_StatusID,Ticket_TypeID,Related_TicketID")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Project_Name", ticket.ProjectID);
            ViewBag.Ticket_PriorityID = new SelectList(db.Ticket_Priorities, "PriorityID", "Priority", ticket.Ticket_PriorityID);
            ViewBag.Ticket_StatusID = new SelectList(db.Ticket_Statuses, "StatusID", "Status", ticket.Ticket_StatusID);
            ViewBag.Ticket_TypeID = new SelectList(db.Ticket_Types, "TypeID", "Type", ticket.Ticket_TypeID);
            ViewBag.Related_TicketID = new SelectList(db.Tickets, "ID", "Description", ticket.Related_TicketID);
            ViewBag.Ticket_SubmitterID = new SelectList(db.Users, "UserID", "First_Name", ticket.Ticket_SubmitterID);
            ViewBag.Assigned_ToID = new SelectList(db.Users, "UserID", "First_Name", ticket.Assigned_ToID);
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
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Project_Name", ticket.ProjectID);
            ViewBag.Ticket_PriorityID = new SelectList(db.Ticket_Priorities, "PriorityID", "Priority", ticket.Ticket_PriorityID);
            ViewBag.Ticket_StatusID = new SelectList(db.Ticket_Statuses, "StatusID", "Status", ticket.Ticket_StatusID);
            ViewBag.Ticket_TypeID = new SelectList(db.Ticket_Types, "TypeID", "Type", ticket.Ticket_TypeID);
            ViewBag.Related_TicketID = new SelectList(db.Tickets, "ID", "Description", ticket.Related_TicketID);
            ViewBag.Ticket_SubmitterID = new SelectList(db.Users, "UserID", "First_Name", ticket.Ticket_SubmitterID);
            ViewBag.Assigned_ToID = new SelectList(db.Users, "UserID", "First_Name", ticket.Assigned_ToID);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Created_Date,Ticket_SubmitterID,Assigned_ToID,ProjectID,Description,Resolution,Ticket_PriorityID,Ticket_StatusID,Ticket_TypeID,Related_TicketID")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Project_Name", ticket.ProjectID);
            ViewBag.Ticket_PriorityID = new SelectList(db.Ticket_Priorities, "PriorityID", "Priority", ticket.Ticket_PriorityID);
            ViewBag.Ticket_StatusID = new SelectList(db.Ticket_Statuses, "StatusID", "Status", ticket.Ticket_StatusID);
            ViewBag.Ticket_TypeID = new SelectList(db.Ticket_Types, "TypeID", "Type", ticket.Ticket_TypeID);
            ViewBag.Related_TicketID = new SelectList(db.Tickets, "ID", "Description", ticket.Related_TicketID);
            ViewBag.Ticket_SubmitterID = new SelectList(db.Users, "UserID", "First_Name", ticket.Ticket_SubmitterID);
            ViewBag.Assigned_ToID = new SelectList(db.Users, "UserID", "First_Name", ticket.Assigned_ToID);
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
