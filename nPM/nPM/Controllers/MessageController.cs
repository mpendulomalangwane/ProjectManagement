using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class MessageController :_BaseController
    {
        private const string entity_name = "Message";
        public ActionResult Index()
        {
            var messages = new List<Message>();
            Guid currentUserId = _getCurrentUserId();

            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
            {
                using (var command = new SqlCommand("sp_MyMessageIds", connection))
                {

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserId", currentUserId));

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }

                }

            }

            foreach (DataRow row in dt.Rows)
            {
                messages.Add(UnitOfWork.RepositoryMessage.GetById(Guid.Parse(row.ItemArray[0].ToString())));
            }
            messages.OrderByDescending(o => o.ReceivedTime).Reverse();
            preparePage(entity_name, "Index");
            return View("IndexMessage", messages);
        }

        public ActionResult gotoCreateMessageForm()
        {
            preparePage(entity_name, "Create");
            var editor = _getEditor(_getCurrentUserId());
            ViewBag.FromId = editor.Id;
            ViewBag.FromName = editor.Name;
            return View("CreateMessage");
        }

        public JsonResult getUsers(string term)
        {
            var users = new List<AnyEntity>();
            var _users = UnitOfWork.RepositoryUser.GetAll().Where(w => w.FullName.Contains(term)).ToList();
            foreach (var item in _users)
            {
                var ae = new AnyEntity();
                ae.Id = item.Id;
                ae.Name = item.FullName;
                ae.boolValue = false;
                users.Add(ae);
            }
            return Json(new { success = users }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateMessage(Message message)
        {
            if (message != null)
            {
                message.Id = Guid.NewGuid();
                var state = UnitOfWork.RepositoryProjectState.GetAll().Where(w => w.Name == "Open").Single();
                message.StateId = state.Id;
                message.StateName = state.Name;
                Guid currentUserId = _getCurrentUserId();
                var editor = UnitOfWork.RepositoryUser.GetSpecific(u => new { u.FullName, u.Id }, u => u.Id == currentUserId).Single();
                message.CreatedById = editor.Id;
                message.CreatedByName = editor.FullName;
                message.CreatedOn = DateTime.Now;
                message.ModifiedById = editor.Id;
                message.ModifiedByName = editor.FullName;
                message.ModifiedOn = DateTime.Now;
                message.ReceivedTime = DateTime.Now;
                var messagetype = UnitOfWork.RepositoryMessageType.GetSpecific(s => new { s.Id, s.Name }, w => w.Name == "Outgoing").Single();

                message.MessageTypeId = messagetype.Id;
                message.MessageTypeName = messagetype.Name;

                message.recipient.Id = Guid.NewGuid();
                message.recipient.MessageId = message.Id;
                message.recipient.ToId = Guid.Parse("B6F95D2A-3843-45D6-96F9-D047463E6E3D");

                message.RecipientId = message.recipient.Id;
                
                UnitOfWork.RepositoryMessage.Add(message);
                UnitOfWork.RepositoryRecipient.Add(message.recipient);
                UnitOfWork.Commit();

                preparePage(entity_name, "Index");
                return View("IndexMessage");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }

        }

        public ActionResult gotoUpdateMessageForm(Guid record_id)
        {
            var message = UnitOfWork.RepositoryMessage.GetById(record_id);
            preparePage(entity_name, "Update");
            return PartialView("_UpdateMessage", message);
        }

    }
}
