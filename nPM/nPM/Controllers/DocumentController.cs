using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class DocumentController : _BaseController
    {
        private const string entity_name = "Document";
        public ActionResult Index()
        {
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            preparePage(entity_name, "Index");
            var tasks = getDocuments(currentUserId).OrderBy(o => o.Name);
            return View("IndexDocument", tasks);
        }

        public List<Document> getDocuments(Guid currentUserId)
        {
            var documents = new List<Document>();

            if (_getCurrentUserRole(currentUserId) == "System Administrator")
            {
                documents = UnitOfWork.RepositoryDocument.GetAll().ToList();
            }
            if (_getCurrentUserRole(currentUserId) == "Project Manager")
            {
                var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == currentUserId).ToList();
                foreach (var item in project_user)
                {
                    var _docs = UnitOfWork.RepositoryDocument.GetAll().Where(w => w.UploadedById == item.UserId).ToList();
                    foreach (var subitem in _docs)
                    {
                        var doc = UnitOfWork.RepositoryDocument.GetById(subitem.Id);
                        documents.Add(doc);
                    }
                }
            }
            if (_getCurrentUserRole(currentUserId) == "System User")
            {
                documents = UnitOfWork.RepositoryDocument.GetAll().Where(w => w.UploadedById == currentUserId).ToList();
            }
            return documents;
        }

        public ActionResult gotoCreateDocumentForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateDocument");
        }

        public JsonResult getTask(string term)
       {
            return Json(new { success = _getTasks(term) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult gotoUpdateDocumentForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            var document = UnitOfWork.RepositoryDocument.GetById(record_id);
            return View("UpdateDocument", document);
        }

        public ActionResult CreateDocument(HttpPostedFileBase Content, Guid TaskId, string TaskName, string Latitude, string Longitude, string Location, DateTime TimeStamp)
        {

            if (Content != null)
            {
                var state = UnitOfWork.RepositoryTaskState.GetAll().Where(w => w.Name == "Open").Single();
                Guid currentUserId = _getCurrentUserId();
                Document doc = new Document();
                doc.Name = Content.FileName;
                doc.Id = Guid.NewGuid();
                doc.Type = Content.ContentType;
                doc.Latitude = doc.Latitude;
                doc.Longitude = doc.Longitude;
                doc.Location = Location;
                if (TimeStamp.Equals(""))
                {
                    TimeStamp = DateTime.Now;
                }
                else
                {
                    doc.TimeStamp = TimeStamp;
                }
                BinaryReader b = new BinaryReader(Content.InputStream);
                byte[] f = b.ReadBytes(Content.ContentLength);
                doc.UploadedById = currentUserId;
                doc.StateId = state.Id;
                doc.StateName = state.Name;
                doc.CreatedOn = DateTime.Now;
                doc.CreatedById = _getEditor(currentUserId).Id;
                doc.CreatedByName = _getEditor(currentUserId).Name;
                doc.UploadedByName = _getEditor(currentUserId).Name;
                doc.TaskId = TaskId;
                doc.TaskName = TaskName;
                doc.ModifiedById = _getEditor(currentUserId).Id;
                doc.ModifiedOn = DateTime.Now;
                doc.ModifiedByName = _getEditor(currentUserId).Name;

                doc.Content = f;
                doc.Type = Content.ContentType;
                doc.Size = Content.ContentLength;

                UnitOfWork.RepositoryDocument.Add(doc);
                UnitOfWork.Commit();

                preparePage(entity_name, "Update");
                var _doc = UnitOfWork.RepositoryDocument.GetById(doc.Id);
                return View("UpdateDocument", _doc);
            }
            else
            {
                return Json(new { success = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getDocument(Guid record_id)
        {
            preparePage(entity_name, "Update");
            var document = UnitOfWork.RepositoryDocument.GetById(record_id);

            if (document.Type == "application/pdf")
            {
                Response.Clear();
                Response.ContentType = document.Type;
                Response.AppendHeader("content-disposition", "inline; filename=foo.pdf");
                Response.BinaryWrite(document.Content);
                Response.End();
            }
            return File(document.Content, document.Type);
        }
    }
}
