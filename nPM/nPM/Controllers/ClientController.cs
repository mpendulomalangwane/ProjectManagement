using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ClientController : _BaseController
    {
        private const string entity_name = "Client";
        public ActionResult Index()
        {
            preparePage(entity_name, "Index");
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            var clients = getClients(currentUserId).OrderBy(o => o.Name);            
            return View("IndexClient", clients);
        }

        public List<Client> getClients(Guid currentUserId)
        {
            var clients = new List<Client>();
            clients = UnitOfWork.RepositoryClient.GetAll().ToList();
            return clients;
        }

        public ActionResult gotoCreateClientForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateClient");
        }

        public ActionResult CreateClient(Client client)
        {
            client.Id = Guid.NewGuid();
            string clNumber = _generateRandonNumber();
            if (!UnitOfWork.RepositoryClient.GetSpecific(c => new { c.ClientNumber }, w => w.ClientNumber == clNumber).Any())
            {
                clNumber = _generateRandonNumber();
            }
            client.ClientNumber = clNumber;
            Guid currentUserId = _getCurrentUserId();
            var editor = UnitOfWork.RepositoryUser.GetSpecific(u => new { u.FullName }, u => u.Id == currentUserId).Single();
            client.CreatedById = currentUserId;
            client.CreatedByName = editor.FullName;
            client.CreatedOn = DateTime.Now;
            client.ModifiedById = currentUserId;
            client.ModifiedByName = editor.FullName;
            client.ModifiedOn = DateTime.Now;
            client.StateCode = 0;
            client.StateCodeName = "Active";
            if (!UnitOfWork.RepositoryClient.GetAll().Where(w => w.Name == client.Name).Any())
            {                
                UnitOfWork.RepositoryClient.Add(client);
                UnitOfWork.Commit();
                var _client = UnitOfWork.RepositoryClient.GetById(client.Id);
                preparePage(entity_name, "Update");
                return View("UpdateClient", _client);
                //return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                
                var _company = UnitOfWork.RepositoryClient.GetById(client.Id);
                return PartialView("_view_editCompany", _company);

            }
        }

        public ActionResult gotoUpdateClientForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            var client = UnitOfWork.RepositoryClient.GetById(record_id);
            return View("UpdateClient", client);
        }

    }
}
