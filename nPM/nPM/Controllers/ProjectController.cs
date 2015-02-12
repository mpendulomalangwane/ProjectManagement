using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ProjectController : _BaseController
    {
        private const string entity_name = "Project";
        public ActionResult Index()
        {            
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            var projets = getProjects(currentUserId).OrderBy(o => o.Name);
            preparePage(entity_name, "Index");
            return View("IndexProject", projets);
        }
                
        public List<Project> getProjects(Guid current_user_id)
        {
            var projects = new List<Project>();
            if (_getCurrentUserRole(current_user_id) == "System Administrator")
            {
                projects = UnitOfWork.RepositoryProject.GetAll().ToList();
            }
            if (_getCurrentUserRole(current_user_id) == "Project Manager")
            {
                var project_user = UnitOfWork.RepositoryProject.GetAll().Where(w => w.ManagerId == currentUserId).ToList();
                foreach (var item in project_user)
                {
                    projects.Add(item);
                }
            }
            if (_getCurrentUserRole(current_user_id) == "System User")
            {
                var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == currentUserId).ToList();
                foreach (var item in project_user)
                {
                    var project = UnitOfWork.RepositoryProject.GetById(item.ProjectId);
                    projects.Add(project);
                }
            }
            return projects;
        }

        public ActionResult gotoCreateProjectForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateProject");
        }
        public ActionResult gotoCreateProjectTaskForm(Guid projectID)
        {
            //preparePage("Task", "Create");
            return View("CreateProjectTask");
        }
        public JsonResult getUsers(String term)
        {
            return Json(new { success = _getUsersThatMatchTerm(term) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult gotoUpdateProjectForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            getProjectDetails(record_id);
            var project = UnitOfWork.RepositoryProject.GetById(record_id);
            return View("UpdateProject", project);
        }

        public ActionResult CreateProject(Project project)
        {
            if (project != null && project.Name != null && ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();
                Guid currentUserId = _getCurrentUserId();
                string prNumber = _generateRandonNumber();
                if (!UnitOfWork.RepositoryProject.GetSpecific(p => new { p.ProjectNumber }, w => w.ProjectNumber == prNumber).Any())
                {
                    prNumber = _generateRandonNumber();                    
                }
                project.ProjectNumber = prNumber;
                var state = UnitOfWork.RepositoryProjectState.GetAll().Where(w => w.Name == "Open").Single();
                project.StateId = state.Id;
                project.StateName = state.Name;
                project.StateCode = 0;
                project.StateCodeName = "Active";
                var editor = UnitOfWork.RepositoryUser.GetSpecific(u => new { u.FullName, u.Id }, u => u.Id == currentUserId).Single();
                project.CreatedById = editor.Id;
                project.CreatedByName = editor.FullName;
                project.CreatedOn = DateTime.Now;
                project.ModifiedById = editor.Id;
                project.ModifiedByName = editor.FullName;
                project.ModifiedOn = DateTime.Now;

                UnitOfWork.RepositoryProject.Add(project);

                //update bridge table between project and manager
                var projectuser = new ProjectUser();
                projectuser.Id = Guid.NewGuid();
                projectuser.ProjectId = project.Id;
                projectuser.UserId = project.ManagerId;

                UnitOfWork.RepositoryProjectUser.Add(projectuser);

                //check if another project exists with the same name
                if (!UnitOfWork.RepositoryProject.GetSpecific(p => new { p.ProjectNumber }, w => w.ProjectNumber == prNumber).Any())
                {
                    UnitOfWork.Commit();
                    var _project = UnitOfWork.RepositoryProject.GetById(project.Id);
                    preparePage(entity_name, "Update");
                    getProjectDetails(_project.Id);
                    return View("UpdateProject", _project);
                }
                else
                {
                    return Json(new { success = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getClients(string term)
        {
            var companies = UnitOfWork.RepositoryClient.GetSpecific(c => new { c.Id, c.Name }, w => w.Id != null).ToList();
            return Json(new { success = companies }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPM(string term)
        {
            return Json(new { success = _getUsersWithSpecificRole(term, "Project Manager") }, JsonRequestBehavior.AllowGet);
        }

        public void getProjectDetails(Guid projectId)
        {

            var tasks = new List<AnyEntity>();
            var users = new List<AnyEntity>();

            /*
             * Get project's tasks
             * 
             * */
            var project = UnitOfWork.RepositoryProject.GetById(projectId);
            var _tasks = UnitOfWork.RepositoryTask.GetAll().Where(w => w.ProjectId == projectId).ToList();
            foreach (var item in _tasks)
            {
                var _task = UnitOfWork.RepositoryTask.GetById(item.Id);
                var ae = new AnyEntity();
                ae.Id = _task.Id;
                ae.Name = _task.Name;
                ae.boolValue = false;
                tasks.Add(ae);
            }

            ViewData["Tasks"] = tasks;

            /*
             * Get project's members
             * 
             * */

            var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.ProjectId == projectId).ToList();
            foreach (var item in project_user)
            {
                var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                var ae = new AnyEntity();
                ae.Id = user.Id;
                ae.Name = user.FullName;
                ae.boolValue = false;
                users.Add(ae);
            }

            ViewData["Users"] = users;

            /*
             * Get states
             * 
             * */

            var states = new List<AnyEntity>();

            foreach (var item in UnitOfWork.RepositoryProjectState.GetAll().ToList())
            {
                if (item.Name != project.StateName)
                {
                    var ae = new AnyEntity();
                    ae.Id = item.Id;
                    ae.Name = item.Name;
                    ae.boolValue = false;
                    states.Add(ae);
                }
            }

            ViewData["States"] = states;

        }

        public ActionResult gotoProjectAddUser()
        {
            var role = UnitOfWork.RepositoryRole.GetAll().Where(w => w.Name == "System User").Single();
            var userorle = UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.RoleId == role.Id);
            var users = new List<User>();
            var _users = new CustomModel();
            _users.anyentity = new List<AnyEntity>();
            foreach (var item in userorle)
            {
                users.Add(UnitOfWork.RepositoryUser.GetById(item.UserId));
                var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                var ae = new AnyEntity();
                ae.Id = user.Id;
                ae.Name = user.FullName;
                ae.boolValue = false;
                _users.anyentity.Add(ae);
            }
            return PartialView("_ProjectAddUser", _users);

        }

        //[HttpPost]
        public ActionResult addUsertoProject(CustomModel project_add_user)
        {
            if (project_add_user != null)
            {
                foreach (var item in project_add_user.anyentity)
                {
                    if (item.boolValue)
                    {
                        var existing_user = UnitOfWork.RepositoryProjectUser.GetSpecific(s => new { s.UserId }, w => w.ProjectId == project_add_user.EntityId && w.UserId == item.Id).Any();
                        if (!existing_user)
                        {
                            var project_user = new ProjectUser();
                            project_user.Id = Guid.NewGuid();
                            project_user.ProjectId = project_add_user.EntityId;
                            project_user.UserId = item.Id;
                            UnitOfWork.RepositoryProjectUser.Add(project_user);
                            UnitOfWork.Commit();
                        }
                    }
                }
                
            }

            preparePage(entity_name, "Update");
            getProjectDetails(project_add_user.EntityId);
            var project = UnitOfWork.RepositoryProject.GetById(project_add_user.EntityId);
            return View("UpdateProject", project);
            
        }

        public ActionResult UpdateProject(Project project)
        {
            var projectToUpdate = validateUpdate(project);
            if (projectToUpdate != null)
            {
                UnitOfWork.RepositoryProject.Update(projectToUpdate);
                UnitOfWork.Commit();
                preparePage(entity_name, "Update");
                getProjectDetails(projectToUpdate.Id);
                return View("/Project/UpdateProject", projectToUpdate);
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View(project);
            }
        }

        public Project validateUpdate(Project projectWithNewValues)
        {
            var taskWithOldValues = UnitOfWork.RepositoryProject.GetById(projectWithNewValues.Id);
            if (taskWithOldValues != null)
            {
                if (projectWithNewValues.Name != null)
                    taskWithOldValues.Name = projectWithNewValues.Name;
                if (projectWithNewValues.Description != null)
                    taskWithOldValues.Description = projectWithNewValues.Description;
                if (projectWithNewValues.StartTime != null)
                    taskWithOldValues.StartTime = projectWithNewValues.StartTime;
                if (projectWithNewValues.EndTime != null)
                    taskWithOldValues.EndTime = projectWithNewValues.EndTime;
                if (projectWithNewValues.ClientId != Guid.Empty)
                    taskWithOldValues.ClientId = projectWithNewValues.ClientId;
                if (projectWithNewValues.ClientName != null)
                    taskWithOldValues.ClientName = projectWithNewValues.ClientName;
                if (projectWithNewValues.ManagerId != Guid.Empty)
                    taskWithOldValues.ManagerId = projectWithNewValues.ManagerId;
                if (projectWithNewValues.ManagerName != null)
                    taskWithOldValues.ManagerName = projectWithNewValues.ManagerName;
                if (projectWithNewValues.StateId != Guid.Empty)
                    taskWithOldValues.StateId = projectWithNewValues.StateId;
                if (projectWithNewValues.StateName != null)
                    taskWithOldValues.StateName = projectWithNewValues.StateName;
                Guid currentUserId = _getCurrentUserId();
                taskWithOldValues.ModifiedById = _getEditor(currentUserId).Id;
                taskWithOldValues.ModifiedByName = _getEditor(currentUserId).Name;
                taskWithOldValues.ModifiedOn = DateTime.Now;
                return taskWithOldValues;
            }
            return null;
        }
    }
}
