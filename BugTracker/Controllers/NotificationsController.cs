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
    [Authorize(Roles = "Administrator,Developer")]
    public class NotificationsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        #region Index
        // GET: Notifications
        public ActionResult Index(int TicketID)
        {
            // check you inputs
            if (TicketID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var notifications = db.Notifications
                                    .Include(n => n.Ticket)
                                    .Include(n => n.User)
                                    .Where(n => n.TicketID == TicketID)
                                    .OrderBy(n => n.ID);

            if (Request.IsAjaxRequest() || ControllerContext.IsChildAction)
            {
                ViewBag.TicketID = TicketID;
                return PartialView("_Index", notifications.ToList());
            }

            return View(notifications.ToList());
        }
        #endregion

        #region Details
        // GET: Notifications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }
        #endregion

        #region Create
        // GET: Notifications/Create
        public ActionResult Create(int TicketID)
        {
            // Check your inputs
            if (TicketID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.TicketID = TicketID;
            ViewBag.ToID = new SelectList(db.Users, "ID", "ASPUserName");
            return View(new NotificationViewModel() );
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TicketID,ToID,Notification1")] NotificationViewModel notificationVM)
        {
            // add date
            // add fromID
            notificationVM.OnDate = DateTime.UtcNow;
            notificationVM.FromID = db.Users.Single(u => u.ASPUserName == HttpContext.User.Identity.Name).ID;

            // check your inputs.
            if (ModelState.IsValid)
            {
                Notification notification = new Notification
                {
                    FromID = notificationVM.FromID,
                    ToID = notificationVM.ToID,
                    TicketID = notificationVM.TicketID,
                    OnDate = notificationVM.OnDate,
                    Notification1 = notificationVM.Notification1
                };

                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { id = notification.TicketID });
            }

            // error saving somehow.
            ViewBag.TicketID = notificationVM.TicketID;
            ViewBag.ToID = new SelectList(db.Users, "ID", "ASPUserName", notificationVM.ToID);

            NotificationViewModel noteVM = new NotificationViewModel
            {
                ToID = notificationVM.ToID,
                Notification1 = notificationVM.Notification1
            };

            return View(noteVM);
        }
        #endregion

        #region Edit
        // GET: Notifications/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", notification.TicketID);
            ViewBag.ToID = new SelectList(db.Users, "ID", "FirstName", notification.ToID);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TicketID,ToID,OnDate,Notification1,BeenRead")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", notification.TicketID);
            ViewBag.ToID = new SelectList(db.Users, "ID", "FirstName", notification.ToID);
            return View(notification);
        }
        #endregion

        #region Delete
        // GET: Notifications/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = await db.Notifications.FindAsync(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Notification notification = await db.Notifications.FindAsync(id);
            db.Notifications.Remove(notification);
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
