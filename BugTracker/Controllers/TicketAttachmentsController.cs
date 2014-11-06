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
    public class TicketAttachmentsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        #region Index
        // GET: TicketAttachments
        public async Task<ActionResult> Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
            return View(await ticketAttachments.ToListAsync());
        }
        #endregion

        #region details
        // GET: TicketAttachments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = await db.TicketAttachments.FindAsync(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }
        #endregion

        #region create
        // GET: TicketAttachments/Create
        public ActionResult Create(int? count)
        {
            if(count != null)
            {
                return PartialView("_AttachmentForm", count);
            }

            count = 0;
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title");
            ViewBag.SubmitterID = new SelectList(db.Users, "ID", "FirstName");
            return View(count);
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,SubmitterID,AttachmentFilePath,OriginalName,Description")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.TicketAttachments.Add(ticketAttachment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketAttachment.TicketID);
            ViewBag.SubmitterID = new SelectList(db.Users, "ID", "FirstName", ticketAttachment.SubmitterID);
            return View(ticketAttachment);
        }
        #endregion

        #region edit
        // GET: TicketAttachments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = await db.TicketAttachments.FindAsync(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketAttachment.TicketID);
            ViewBag.SubmitterID = new SelectList(db.Users, "ID", "FirstName", ticketAttachment.SubmitterID);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TicketID,SubmitterID,AttachmentFilePath,OriginalName,Description")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", ticketAttachment.TicketID);
            ViewBag.SubmitterID = new SelectList(db.Users, "ID", "FirstName", ticketAttachment.SubmitterID);
            return View(ticketAttachment);
        }
        #endregion

        #region delete
        // GET: TicketAttachments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = await db.TicketAttachments.FindAsync(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = await db.TicketAttachments.FindAsync(id);
            db.TicketAttachments.Remove(ticketAttachment);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
