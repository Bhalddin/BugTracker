using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.Linq.Dynamic;
using PagedList;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace BugTracker.Models
{
    public partial class TicketAttachment
    {

        // init ctor
        public TicketAttachment()
        {
        }

        // custom ctor
        public TicketAttachment(int _TicketID, int _SubmitterID, string _AttachmentFilePath, string _OriginalName, string _Description)
        {
            TicketID = _TicketID;
            SubmitterID = _SubmitterID;
            AttachmentFilePath = _AttachmentFilePath;
            OriginalName = _OriginalName;
            Description = _Description ?? "No Description";
        }

        // helper func.
        public string GetUrl()
        {
            return Regex.Replace(AttachmentFilePath, @"^(.*?)(?=/App_Data/)", "~");
        }

        public string GetDownloadUrl()
        {
            return "~/TicketAttachments/Download/" + this.ID;
        }

        // Static method.
        public static void SaveAsAttachment(HttpPostedFileBase attachment, string serverFolderPath, BugTrackerEntities db, int ticketID, int SubmitterID, string attDescription)
        {
            // The safest way to safe a file is to NOT use it's given name, b/c names might colide and cause files to be overridden
            // it is safer to use the hash of the file and name it according to that, so different files with the same name will be saves seperatly.

            using (var md5 = MD5.Create())
            {   
                string fileHash = BitConverter.ToString(md5.ComputeHash(attachment.InputStream)).Replace("-", "").ToLower();
                string fileExtension = Path.GetExtension(attachment.FileName);

                // filepath is the full path on the server, this is used for saving and deleting the file.
                string saveFileName = fileHash + fileExtension;
                string filePath = Path.Combine(serverFolderPath, saveFileName);

                // make sure that folder exists
                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }

                // now save the file
                var newAttach = new TicketAttachment(ticketID, SubmitterID, filePath, attachment.FileName, attDescription);

                db.TicketAttachments.Add(newAttach);
                db.SaveChanges();

                attachment.SaveAs(newAttach.AttachmentFilePath);
            }
        }


        // Helper found online to get the mimetype
        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }



}