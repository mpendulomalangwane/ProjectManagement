using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class UserController : _BaseController
    {
        private const string  entity_name = "User";
        public ActionResult Index()
        {
            preparePage(entity_name, "Index");
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            var users = getUsers(currentUserId).OrderBy(o => o.FullName);   
            return View("IndexUser", users);
        }

        public List<User> getUsers(Guid currentUserId)
        {
            var users = new List<User>();
            if (_getCurrentUserRole(currentUserId) == "System Administrator")
            {
                users = UnitOfWork.RepositoryUser.GetAll().ToList();
            }
            if (_getCurrentUserRole(currentUserId) == "Project Manager")
            {
                var project_user = UnitOfWork.RepositoryProjectUser.GetSpecific(pu => new {pu.UserId }, w => w.UserId == currentUserId).ToList();
                foreach (var item in project_user)
                {
                    var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                    users.Add(user);
                }
            }
            if (_getCurrentUserRole(currentUserId) == "System User")
            {
                var team_user = UnitOfWork.RepositoryTeamUser.GetSpecific(tu => new { tu.UserId } ,w => w.UserId == currentUserId).ToList();
                foreach (var item in team_user)
                {
                    var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                    users.Add(user);
                }
            }
            return users;

        }

        public ActionResult gotoCreateUserForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateUser");
        }
        
        public ActionResult CreateUser(User user)
        {
            if (user != null && user.FirstName != null)
            {
                Guid currentUserId = _getCurrentUserId();
                var editor = UnitOfWork.RepositoryUser.GetById(currentUserId);
                user.Id = Guid.NewGuid();
                user.FullName = user.LastName + ", " + user.FirstName;
                user.CreatedById = editor.Id; ;
                user.CreatedByName = editor.FullName;
                user.CreatedOn = DateTime.Now;
                user.ModifiedById = editor.Id;
                user.ModifiedByName = editor.FullName;
                user.ModifiedOn = DateTime.Now;
                user.PassWord = "password";
                var state = UnitOfWork.RepositoryState.GetAll().Where(w => w.Name == "Active").Single();
                user.StateId = state.Id;
                user.StateName = state.Name;

                UnitOfWork.RepositoryUser.Add(user);
                UnitOfWork.Commit();
                String emailSubject = "Confirmation Of New User creation ";
                String emailBody = " Hi "+user.FirstName+"\n" +  "\n Welcome to the project management system your user name is :" + user.UserName  + " and your password is :password." +"\n Both username and password are case sensitive.\n\n Thank you :)";
                sendEmail(user.Email, "", emailSubject, emailBody);
                var _user = UnitOfWork.RepositoryUser.GetById(user.Id);
                getUserDetails(user.Id);
                preparePage(entity_name, "Update");
                return View("UpdateUSer", _user);
            }
            else
            {
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult gotoUpdateUserForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            getUserDetails(record_id);
            var user = UnitOfWork.RepositoryUser.GetById(record_id);
            return View("UpdateUser", user);
        }

        public void getUserDetails(Guid user_id)
        {
            ModelState.Clear();
            var roles = new List<AnyEntity>();
            var teams = new List<AnyEntity>();
            var projects = new List<AnyEntity>();
            var role_user = UnitOfWork.RepositoryRoleUser.GetSpecific(r => new { r.RoleId }, w => w.UserId == user_id).ToList();
            
            foreach (var item in role_user)
            {
                //get security role base on id found
                var role = UnitOfWork.RepositoryRole.GetById(item.RoleId);
                var ae = new AnyEntity();
                ae.Id = role.Id;
                ae.Name = role.Name;
                ae.boolValue = true;
                roles.Add(ae);
            }
            ViewData["Rights"] = roles;
            var team_user = UnitOfWork.RepositoryTeamUser.GetSpecific( tu => new { tu.TeamId }, w => w.UserId == user_id).ToList();
            foreach (var item in team_user)
            {
                var team = UnitOfWork.RepositoryTeam.GetById(item.TeamId);
                var ae = new AnyEntity();
                ae.Id = team.Id;
                ae.Name = team.Name;
                ae.boolValue = true;
                teams.Add(ae);
            }
            ViewData["Teams"] = teams;
            var project_user = UnitOfWork.RepositoryProjectUser.GetSpecific(pu => new {pu.ProjectId}, w => w.UserId == user_id).ToList();
            foreach (var item in project_user)
            {
                var project = UnitOfWork.RepositoryProject.GetById(item.ProjectId);
                var ae = new AnyEntity();
                ae.Id = project.Id;
                ae.Name = project.Name;
                ae.boolValue = true;
                projects.Add(ae);
            }
            ViewData["Projects"] = projects;
        }

        public JsonResult generateUsername()
        {
            return Json(new { success = _generateRandonNumber() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult updateRoles(CustomModel cm)
        {
            if (cm != null)
            {
                foreach (var item in cm.anyentity)
                {
                    if (item.boolValue)
                    {
                        var existing_role_user = UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.UserId == cm.EntityId && w.RoleId == item.Id).Any();
                        if (!existing_role_user)
                        {
                            var role_user = new RoleUser();
                            role_user.Id = Guid.NewGuid();
                            role_user.RoleId = item.Id;
                            role_user.UserId = cm.EntityId;
                            UnitOfWork.RepositoryRoleUser.Add(role_user);
                            UnitOfWork.Commit();
                        }
                    }
                }
            }
            preparePage(entity_name, "Update");
            getUserDetails(cm.EntityId);
            var user = UnitOfWork.RepositoryUser.GetById(cm.EntityId);
            return View("UpdateUser", user);
        }

        public ActionResult updateProjectUsers(CustomModel cm)
        {
            if (cm != null)
            {
                foreach (var item in cm.anyentity)
                {
                    if (item.boolValue)
                    {
                        var existing_role_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.ProjectId == item.Id &&w.UserId==cm.EntityId ).Any();
                        if (!existing_role_user)
                        {
                            var prjct_user = new ProjectUser();
                            prjct_user.Id = Guid.NewGuid();
                            prjct_user.ProjectId = item.Id;
                            prjct_user.UserId = cm.EntityId;
                            UnitOfWork.RepositoryProjectUser.Add(prjct_user);
                            UnitOfWork.Commit();
                        }
                    }
                }
            }
            preparePage(entity_name, "Update");
            getUserDetails(cm.EntityId);
            var user = UnitOfWork.RepositoryUser.GetById(cm.EntityId);
            return View("UpdateUser", user);
        }
        public ActionResult removeRolesfromUser(List<Guid>  sroleIds, Guid userId)
        {
            if (sroleIds != null)
            {
                foreach (var item in sroleIds)
                {

                    var existing_role_user = UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.UserId == userId && w.RoleId==item).FirstOrDefault();
                    RoleUser usr_role= UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.UserId == userId && w.RoleId==item).FirstOrDefault();
                    if (existing_role_user!=null)
                    {
                        UnitOfWork.RepositoryRoleUser.DeleteById(usr_role.Id);
                        UnitOfWork.Commit();
                    }
                }
            }
            preparePage(entity_name, "Update");
            getUserDetails(userId);
            ModelState.Clear();
            Models.User user = UnitOfWork.RepositoryUser.GetById(userId);
            return View("UpdateUser", user);
        }
        public ActionResult removeProjectsfromUser(List<Guid> sProjectIds, Guid userId)
        {
            if (sProjectIds != null)
            {
                foreach (var item in sProjectIds)
                {

                    var existing_project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == userId && w.ProjectId == item).FirstOrDefault();
                    ProjectUser usr_prj = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == userId && w.ProjectId == item).FirstOrDefault();
                    if (existing_project_user != null)
                    {
                        UnitOfWork.RepositoryProjectUser.DeleteById(usr_prj.Id);
                        UnitOfWork.Commit();
                    }
                }
            }
            preparePage(entity_name, "Update");
            getUserDetails(userId);
            ModelState.Clear();
            Models.User user = UnitOfWork.RepositoryUser.GetById(userId);
            return View("UpdateUser", user);
        }
        public ActionResult gotoManageRole()
        {            
            var roles = UnitOfWork.RepositoryRole.GetAll().ToList();
            var _roles = new CustomModel();
            _roles.anyentity = new List<AnyEntity>();
            foreach (var item in roles)
            {
                var role = UnitOfWork.RepositoryRole.GetById(item.Id);
                var ae = new AnyEntity();
                ae.Id = role.Id;
                ae.Name = role.Name;
                ae.boolValue = false;
                _roles.anyentity.Add(ae);
            }
            return PartialView("_ManageRoles", _roles);
        }
        public ActionResult gotoManageProjectUser()
        {
            var projcts = UnitOfWork.RepositoryProject.GetAll().ToList();
            var _projcts = new CustomModel();
            _projcts.anyentity = new List<AnyEntity>();
            foreach (var item in projcts)
            {
                var role = UnitOfWork.RepositoryProject.GetById(item.Id);
                var ae = new AnyEntity();
                ae.Id = role.Id;
                ae.Name = role.Name;
                ae.boolValue = false;
                _projcts.anyentity.Add(ae);
            }
            return PartialView("_ManageProjectUsers", _projcts);
        }
    }
}
