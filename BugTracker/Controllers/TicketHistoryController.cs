using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [Authorize(Roles="Administrator,Developer")]
    public class TicketHistoryController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();


        #region Index
        // GET: TicketHistory
        public ActionResult Index(int? id)
        {
            // check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the Tickets History
            var ticketHistory = db.TicketHistories
                .Include(t => t.User)
                .Where(h => h.TicketID == id)
                .OrderBy(h => h.ID);

            // get badge count
            ViewBag.BadgeCount = ticketHistory.Count();
            ViewBag.TicketID = id;

            return PartialView("_Index", ticketHistory.ToList());
        }
        #endregion


        #region Update History

        //[ChildActionOnly]
        //public void AlterTicketAndUpdateHistory(HistoryViewModel newTicket)
        //{
        //    // check your inputs
        //    if (ModelState.IsValid)
        //    {
        //        // get the original ticket
        //        Ticket oldTicket = db.Tickets
        //                        .Include(t => t.Project)
        //                        .Include(t => t.TicketPriority)
        //                        .Include(t => t.TicketStatus)
        //                        .Include(t => t.TicketType)
        //                        .Include(t => t.User1).Single(t => t.ID == newTicket.ID); // dev.

        //        // create new


        //        // create History log variable
        //        string historyLog;

        //        // list of properties to check
        //        string[] ticketProps = new string[] {"AssignedToID", "ProjectID", "Title", "Description", "Resolution", }
        //        // save the history.
        //    }
        //}

        // POST: TicketHistory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TicketID,TicketEditorID,DateOfChange,Ticket_Alteration")] TicketHistory ticketHistory)
        {
            if (ModelState.IsValid)
            {
                db.TicketHistories.Add(ticketHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketHistory.TicketID);
            ViewBag.TicketEditorID = new SelectList(db.Users, "ID", "FirstName", ticketHistory.TicketEditorID);
            return View(ticketHistory);
        }
        #endregion


        #region stuff
        // GET: TicketHistory/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistory);
        }
        #endregion

        #region stuff
        // GET: TicketHistory/Edit/5
        public ActionResult Edit(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketHistory.TicketID);
            ViewBag.TicketEditorID = new SelectList(db.Users, "ID", "FirstName", ticketHistory.TicketEditorID);
            return View(ticketHistory);
        }

        // POST: TicketHistory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TicketID,TicketEditorID,DateOfChange,Ticket_Alteration")] TicketHistory ticketHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketHistory.TicketID);
            ViewBag.TicketEditorID = new SelectList(db.Users, "ID", "FirstName", ticketHistory.TicketEditorID);
            return View(ticketHistory);
        }

        // GET: TicketHistory/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistory);
        }

        // POST: TicketHistory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            db.TicketHistories.Remove(ticketHistory);
            db.SaveChanges();
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
        #endregion
}
