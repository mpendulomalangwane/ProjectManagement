using Logic;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    public class _BaseController : Controller
    {

        private PMUow unitOfWork;
        public Guid currentUserId;

        public _BaseController()
        {
        }

        public PMUow UnitOfWork
        {
            get
            {
                if (this.unitOfWork == null)
                    this.unitOfWork = new PMUow();
                return unitOfWork;
            }
        }

        public Guid _getCurrentUserId()
        {
            if (Request.Cookies != null)
            {
                if (Request.Cookies["UserSettings"] != null)
                {
                    if (Request.Cookies["UserSettings"]["UserId"] != null)
                    {
                        this.currentUserId = Guid.Parse(Request.Cookies["UserSettings"]["UserId"]);
                    }
                }
            }
            return currentUserId;
        }

        public void _settingNavigationPane()
        {
            //setting up left pane
            var group = UnitOfWork.RepositoryGroup.GetAll().OrderBy(o => o.Order).ToList();
            ViewData["Group"] = group;
            var group_item = UnitOfWork.RepositoryGroupItem.GetAll().OrderBy(o => o.Order).ToList();
            ViewData["GroupItem"] = group_item;
        }

        public bool _isCurrentUserAdmin(Guid user_id)
        {
            bool retvalue = false;
            //get list of security role ids belonging to current user
            var role_user = UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.UserId == user_id).ToList();
            foreach (var item in role_user)
            {
                //check if of the security role is admin
                var role = UnitOfWork.RepositoryRole.GetSpecific(r => new { r.Id}, w => w.Name == "System Administrator" && w.Id == item.RoleId).FirstOrDefault();
                if (role != null)
                {
                    retvalue = true;
                }
            }
            return retvalue;
        }
        public bool sendEmail(String reciever, String sender, String subject, string body)
        {
            try
            {
                MailAddress mailfrom = new MailAddress("mpendulomalangwane@gmail.com");
                MailAddress mailto = new MailAddress(reciever);
                MailMessage newmsg = new MailMessage(mailfrom, mailto);
                newmsg.Subject = subject;
                newmsg.Body = body;
                if (sender != "")
                {
                    MailAddress copy = new MailAddress(sender);
                    newmsg.CC.Add(copy);
                }
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("mpendulomalangwane@gmail.com", "200936584");
                smtp.EnableSsl = true;
                smtp.Send(newmsg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string _getCurrentUserRole(Guid current_user_id)
        {
            string retvalue = string.Empty;
            //get list of security role ids belonging to current user
            var role_user = UnitOfWork.RepositoryRoleUser.GetSpecific(ru => new { ru.RoleId }, w => w.UserId == current_user_id).ToList();
            foreach (var item in role_user)
            {
                var role = UnitOfWork.RepositoryRole.GetSpecific(ru => new { ru.Name },w => w.Name == "System Administrator" && w.Id == item.RoleId).FirstOrDefault();
                if (role == null)
                {
                    role = UnitOfWork.RepositoryRole.GetSpecific(ru => new { ru.Name }, w => w.Name == "System Administrator" && w.Id == item.RoleId).FirstOrDefault();
                    //check if of the security role is admin
                    if (role == null)
                    {
                        role = UnitOfWork.RepositoryRole.GetSpecific(ru => new { ru.Name },w => w.Name == "Project Manager" && w.Id == item.RoleId).FirstOrDefault();
                        //check if user is manager
                        if (role == null)
                        {
                            role = UnitOfWork.RepositoryRole.GetSpecific(ru => new { ru.Name },w => w.Name == "System User" && w.Id == item.RoleId).FirstOrDefault();
                        }
                    }
                }
                retvalue = role.Name;
                break;
            }
            return retvalue;
        }

        public DataTable _loadRibbon(string entity_name, string page_name)
        {
            //var ribbon_button = new List<RibbonButton>();
            var ribbon_button = new DataTable();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
            {
                using (var command = new SqlCommand("sp_RibbonButtons", connection))
                {

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@PageName", page_name));
                    command.Parameters.Add(new SqlParameter("@EntityName", entity_name));
                    command.Parameters.Add(new SqlParameter("@UserId", _getCurrentUserId()));

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(ribbon_button);
                    }
                }
            }
            return ribbon_button;
        }

        public string _generateRandonNumber()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public State _getActiveStates()
        {
            return UnitOfWork.RepositoryState.GetAll().Where(w => w.Name == "Active").Single();
        }

        public State _getInactiveStates()
        {
            return UnitOfWork.RepositoryState.GetAll().Where(w => w.Name == "Inactive").Single();
        }

        public Editor _getEditor(Guid editor_id)
        {
            var _editor = UnitOfWork.RepositoryUser.GetSpecific(u => new { u.Id, u.FullName }, w => w.Id == editor_id).Single();
            var editor = new Editor(_editor.Id, _editor.FullName);
            return editor;
        }

        public void preparePage(string entity_name, string page_name)
        {
            _settingNavigationPane();
            var ribbon = _loadRibbon(entity_name, page_name);
            ViewData["Ribbon"] = ribbon;
            var states = UnitOfWork.RepositoryState.GetAll().ToList();
            ViewData["States"] = states;
            ViewBag.Page = entity_name;

        }

        public List<AnyEntity> _getUsersWithSpecificRole(string term, string role_name)
        {
            var users = new List<AnyEntity>();
            //get all users where fullnames matche term
            var _users = UnitOfWork.RepositoryUser.GetSpecific(s => new { s.Id, s.FullName }, w => w.FullName.Contains(term)).ToList();
            foreach (var user in _users)
            {
                //get the id of the project manager role
               var role = UnitOfWork.RepositoryRole.GetSpecific(s => new { s.Id }, w => w.Name == role_name).Single();
                //get user with the role "project manager" role
                var userorle = UnitOfWork.RepositoryRoleUser.GetSpecific(s => new { s.Id }, w => w.RoleId == role.Id && w.UserId == user.Id).SingleOrDefault();
                if (userorle != null)
                {
                    var ae = new AnyEntity();
                    ae.Id = user.Id;
                    ae.Name = user.FullName;
                    ae.boolValue = false;
                    users.Add(ae);
                }
            }
            return users;
        }
        public List<AnyEntity> _getUsersThatMatchTerm(string term)
        {
            var users = new List<AnyEntity>();
            //get all users where fullnames matche term
            var _users = UnitOfWork.RepositoryUser.GetSpecific(s => new { s.Id, s.FullName }, w => w.FullName.Contains(term)).ToList();
            foreach (var user in _users)
            {
                //get the id of the project manager role
                // var role = UnitOfWork.RepositoryRole.GetSpecific(s => new { s.Id }, w => w.Name == role_name).Single();
                //get user with the role "project manager" role
                if (users != null)
                {
                    var ae = new AnyEntity();
                    ae.Id = user.Id;
                    ae.Name = user.FullName;
                    ae.boolValue = false;
                    users.Add(ae);
                }
            }
            return users;
        }
        public List<AnyEntity> _getProjctUsers(Guid projectID)
        {
            var users = new List<AnyEntity>();
            //get all users where fullnames matche term
            var _users = UnitOfWork.RepositoryProjectUser.GetSpecific(s => new { s.Id }, w => w.ProjectId==projectID).ToList();
            foreach (var user in _users)
            {
                //get user with the role "project manager" role
                var userorle = UnitOfWork.RepositoryUser.GetSpecific(s => new { s.Id,s.FullName }, w => w.Id == user.Id).SingleOrDefault();
                if (userorle != null)
                {
                    var ae = new AnyEntity();
                    ae.Id = userorle.Id;
                    ae.Name = userorle.FullName;
                    ae.boolValue = false;
                    users.Add(ae);
                }
            }
            return users;
        }
        public List<AnyEntity> _getProjects(string term)
        {
            var projects = new List<AnyEntity>();
            var _projects = UnitOfWork.RepositoryProject.GetSpecific(s => new { s.Id, s.Name }, w => w.Name.Contains(term)).ToList();
            foreach (var item in _projects)
            {
                var ae = new AnyEntity();
                ae.Id = item.Id;
                ae.Name = item.Name;
                ae.boolValue = false;
                projects.Add(ae);
            }
            return projects;
        }

        public List<AnyEntity> _getTasks(string term)
        {
            var tasks = new List<AnyEntity>();
            var _tasks = UnitOfWork.RepositoryTask.GetSpecific(s => new { s.Id, s.Name }, w => (w.Name.Contains(term) || w.TaskNumber.Contains(term)) && w.StateName == "Active").ToList();
            foreach (var item in _tasks)
            {
                var ae = new AnyEntity();
                ae.Id = item.Id;
                ae.Name = item.Name;
                ae.boolValue = false;
                tasks.Add(ae);
            }
            return tasks;
        }

    }
}
