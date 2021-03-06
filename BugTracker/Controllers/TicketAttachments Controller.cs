﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.IO;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketAttachmentsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        #region Index
        // GET: TicketAttachments
        [Authorize(Roles = "Administrator,Developer")]
        public ActionResult Index(int? id)
        {
            // save params
            ViewBag.TicketID = id;

            // filter attachments if id is there.
            var ticketAttachments = db.TicketAttachments
                                        .Include(t => t.Ticket)
                                        .Include(t => t.User)
                                        .Where(a => id == null || a.TicketID == id)
                                        .OrderBy(a => a.ID);

            // if this is an ajax request or child action return a partial.
            if (Request.IsAjaxRequest() || ControllerContext.IsChildAction)
            {
                ViewBag.BadgeCount = ticketAttachments.Count();
                return PartialView("_Index", ticketAttachments.ToList());
            }

            return View(ticketAttachments.ToList());

        }
        #endregion

        #region create
        // GET: TicketAttachments/Create
        public ActionResult Create(int? count, int id)
        {
            // content both views need.
            ViewBag.Count = count ?? 0;

            if (count != null)
            {
                return PartialView("_AttachmentForm");
            }

            ViewBag.TicketID = id;

            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int TicketID)
        {
            if (Request.Form.Count > 0)
            {
                // get Summiter's ID
                var SubmitterID = db.Users.Single(u => u.ASPUserName == HttpContext.User.Identity.Name).ID;

                // save attachments to db.
                foreach (string key in Request.Files)
                {
                    // check that file exists.
                    var file = Request.Files[key];
                    if (file == null || file.ContentLength == 0) continue;

                    string currentDesc = Request.Form[key];
                    string serverFolderPath = Server.MapPath("~/App_Data/Attachments/");

                    // save file to server and database.
                    TicketAttachment.SaveAsAttachment(file, serverFolderPath, db, TicketID, SubmitterID, currentDesc);
                }

                return RedirectToAction("Details", "Tickets", new { id = TicketID });
            }

            return View("Error");
        }
        #endregion

        #region Download
        [Authorize(Roles = "Administrator,Developer")]
        public ActionResult Download(int? id)
        {
            // check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Attach = db.TicketAttachments.Find(id);
            var contType = TicketAttachment.GetMimeType(Attach.OriginalName);
            var file = File(Attach.AttachmentFilePath, contType, Attach.OriginalName);

            return file;
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
