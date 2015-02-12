using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class TaskController : _BaseController
    {
        private const string entity_name = "Task";

        public ActionResult Index()
        {
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            preparePage(entity_name, "Index");
            var tasks = getTasks(currentUserId).OrderBy(o => o.ProjectName);
            return View("IndexTask", tasks);
        }

        public List<Task> getTasks(Guid current_user_id)
        {
            var tasks = new List<Task>();
            if (_getCurrentUserRole(current_user_id) == "System Administrator")
            {
                tasks = UnitOfWork.RepositoryTask.GetAll().ToList();
            }
            if (_getCurrentUserRole(current_user_id) == "Project Manager")
            {
                var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == current_user_id).ToList();
                foreach (var item in project_user)
                {
                    var _tasks = UnitOfWork.RepositoryTask.GetAll().Where(w => w.ProjectId == item.ProjectId && (w.AssigneeId==current_user_id || w.ReporterId==current_user_id) ).ToList();
                    foreach (var subitem in _tasks)
                    {
                        var task = UnitOfWork.RepositoryTask.GetById(subitem.Id);
                        tasks.Add(task);
                    }
                }
            }
            if (_getCurrentUserRole(current_user_id) == "System User")
            {
                tasks = UnitOfWork.RepositoryTask.GetAll().Where(w => w.AssigneeId == current_user_id).ToList();
            }
            return tasks;
        }

        public ActionResult gotoCreateTaskForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateTask");
        }

        public ActionResult gotoUpdateTaskForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            getTaskDetails(record_id);
            var task = UnitOfWork.RepositoryTask.GetById(record_id);
            return View("UpdateTask", task);
        }
        public ActionResult CreateTaskComment(TaskComment taskcomment)
        {
            if (taskcomment != null && taskcomment.TaskId != Guid.Empty)
            {


                if (taskcomment != null)
                {

                    Guid currentUserId = _getCurrentUserId();
                    taskcomment.Id = Guid.NewGuid();
                    taskcomment.TaskName = UnitOfWork.RepositoryTask.GetById(taskcomment.TaskId).Name;
                    taskcomment.ModifiedOn = DateTime.Now;
                    taskcomment.CreatedOn = DateTime.Now;
                    taskcomment.CreatedById = _getEditor(currentUserId).Id;
                    taskcomment.CreatedByName = _getEditor(currentUserId).Name;
                    taskcomment.ModifiedById = _getEditor(currentUserId).Id;
                    taskcomment.ModifiedByName = _getEditor(currentUserId).Name;
                    taskcomment.ModifiedOn = DateTime.Now;
                    var state = UnitOfWork.RepositoryTaskState.GetAll().Where(w => w.Name == "Open").Single();
                    taskcomment.StateId = state.Id;
                    taskcomment.StateName = state.Name;
                    UnitOfWork.RepositoryTaskComment.Add(taskcomment);
                    UnitOfWork.Commit();

                    preparePage(entity_name, "Update");
                    getTaskDetails(taskcomment.TaskId);
                    var task = UnitOfWork.RepositoryTask.GetById(taskcomment.TaskId);
                    return View("UpdateTask", task);
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

        public ActionResult CreateTask(Task task)
        {
             if (task != null)
             {
                     Guid currentUserId = _getCurrentUserId();
                    //get task staste
                    var state = UnitOfWork.RepositoryTaskState.GetAll().Where(w => w.Name == "Open").Single();
                    //create task

                    task.Id = Guid.NewGuid();
                    task.StateId = _getActiveStates().Id;
                    task.StateName = _getActiveStates().Name;
                    task.TaskStateId = state.Id;
                    task.TaskStateName = state.Name;
                    task.ReporterId = _getEditor(currentUserId).Id;
                    task.ReporterName = _getEditor(currentUserId).Name;
                    task.TaskNumber = _generateRandonNumber();
                    task.IsLinked = false;
                    task.CreatedById = _getEditor(currentUserId).Id;
                    task.CreatedByName = _getEditor(currentUserId).Name;
                    task.CreatedOn = DateTime.Now;
                    task.ModifiedById = _getEditor(currentUserId).Id;
                    task.ModifiedByName = _getEditor(currentUserId).Name;
                    task.ModifiedOn = DateTime.Now;
                    if (!UnitOfWork.RepositoryTask.GetAll().Where(w => w.Name == task.Name && w.ProjectId == task.ProjectId).Any())
                    {
                        UnitOfWork.RepositoryTask.Add(task);
                        UnitOfWork.Commit();
                        preparePage(entity_name, "Update");
                        getTaskDetails(task.Id);
                        var _task = UnitOfWork.RepositoryTask.GetById(task.Id);
                        String emailSubject = "New Task (" + task.Name + ")";
                        String emailBody = " Task Description :" + task.Description + "\n Start Date :" + task.StartTime + " \n End Date :" + task.EndTime + "\n Reporter " + task.ReporterName;
                        sendEmail(UnitOfWork.RepositoryUser.GetById(task.AssigneeId).Email, UnitOfWork.RepositoryUser.GetById(task.ReporterId).Email, emailSubject, emailBody);
                        return View("UpdateTask", _task);
                    }
                    else
                    {
                        return Json(new { success = "error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var _task = UnitOfWork.RepositoryTask.GetById(task.Id);
                    return View("UpdateTask", _task);
                }
        }

        public void getTaskDetails(Guid task_id)
        {
            var tasktimes = UnitOfWork.RepositoryTaskTime.GetAll().Where(w => w.TaskId == task_id).ToList();
            var _tasktimes = UnitOfWork.RepositoryTaskTime.GetSpecific(s => new { s.Id, s.Duration }, w => w.TaskId == task_id).ToList();
            ViewData["TaskTime"] = tasktimes;
            var task_state = UnitOfWork.RepositoryTaskState.GetAll().ToList();
            ViewData["TaskState"] = task_state;
            var comments = UnitOfWork.RepositoryTaskComment.GetAll().Where(w => w.TaskId == task_id).ToList();
            ViewData["Comments"] = comments;
            var linkedtaskids = UnitOfWork.RepositoryLinkedTask.GetSpecific(s => new { s.TaskId1, s.TaskId2 }, w => w.TaskId1 == task_id || w.TaskId2 == task_id).ToList();
            var linkedtasks = new List<Task>();
            foreach(var taskid in linkedtaskids){
                if (taskid.TaskId1 != task_id)
                    linkedtasks.Add(UnitOfWork.RepositoryTask.GetById(taskid.TaskId1));
                else if (taskid.TaskId2 != task_id)
                    linkedtasks.Add(UnitOfWork.RepositoryTask.GetById(taskid.TaskId2));
            }
            ViewData["LinkedTasks"] = linkedtasks;
            var _documents = UnitOfWork.RepositoryDocument.GetSpecific(s => new { s.Id, s.Name }, w => w.TaskId == task_id).ToList();
            var documents = new List<AnyEntity>();
            foreach (var item in _documents)
            {
                var document = new AnyEntity();
                document.Id = item.Id;
                document.Name = item.Name;
                document.boolValue = false;
                documents.Add(document);
            }
            ViewData["Documents"] = documents;
        }

        public JsonResult getProjects(string term)
        {
            return Json(new { success = _getProjects(term) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getUsers(String term )
        {
            return Json(new { success = _getUsersThatMatchTerm(term) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getTask(string term)
        {            
            return Json(new { success = _getTasks(term) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult gotoCreateTaskTimeForm(Guid task_Id)
        {
            preparePage(entity_name, "Create");
            return PartialView("_CreateTaskTime");
        }

        public ActionResult gotoCreateTaskCommentForm(Guid task_Id)
        {
            preparePage(entity_name, "Create");
            return PartialView("_CreateTaskComment");
        }

        public ActionResult CreateTaskTime(TaskTime tasktime)
        {
            if (tasktime != null && tasktime.TaskId != Guid.Empty)
            {


                if (tasktime.Duration > 0.0)
                {

                    Guid currentUserId = _getCurrentUserId();
                    tasktime.Id = Guid.NewGuid();
                    var state = UnitOfWork.RepositoryTaskState.GetAll().Where(w => w.Name == "Open").Single();
                    tasktime.Duration = Math.Round(tasktime.Duration.Value, 2);
                    tasktime.StateId = _getActiveStates().Id;
                    tasktime.StateName = _getActiveStates().Name;
                    tasktime.ModifiedOn = DateTime.Now;
                    tasktime.CreatedOn = DateTime.Now;
                    tasktime.CreatedById = _getEditor(currentUserId).Id;
                    tasktime.CreatedByName = _getEditor(currentUserId).Name;
                    tasktime.ModifiedById = _getEditor(currentUserId).Id;
                    tasktime.ModifiedByName = _getEditor(currentUserId).Name;
                    tasktime.ModifiedOn = DateTime.Now;
                    UnitOfWork.RepositoryTaskTime.Add(tasktime);
                    UnitOfWork.Commit();

                    preparePage(entity_name, "Update");
                    getTaskDetails(tasktime.TaskId);
                    var task = UnitOfWork.RepositoryTask.GetById(tasktime.TaskId);
                    return View("UpdateTask", task);
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

        public double validateTaskDuration(string s_duration)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            int indexH = s_duration.IndexOf('h');
            int indexM = s_duration.IndexOf('m', indexH);
            if (indexH == -1 || indexM == -1) return -1;
            string one = s_duration.Substring(0, indexH);
            string two = s_duration.Substring(indexH + 1, s_duration.Length - s_duration.IndexOf('h') - 2);
            if ((IsInteger(one) && IsInteger(two)))
            {
                return Convert.ToDouble(one) + (Convert.ToDouble(two) / 60);
            }
            return -1;
        }

        private bool IsInteger(string str)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                {
                    return false;
                }
                if (!regex.IsMatch(str))
                {
                    return false;
                }
                return true;
            }
            catch (Exception )
            {
            }
            return false;
        }
        
        public ActionResult ResolveTask(TaskResolution taskresolution)
        {
            if (taskresolution != null && taskresolution.TaskId != Guid.Empty)
            {

                Guid currentUserId = _getCurrentUserId();                
                

                var taskcomment = new TaskComment();
                taskcomment.Id = Guid.NewGuid();
                taskcomment.TaskId = taskresolution.TaskId;
                taskcomment.TaskName = taskresolution.TaskName;
                taskcomment.Comment = taskresolution.CommentText;
                taskcomment.StateId = _getActiveStates().Id;
                taskcomment.StateName = _getActiveStates().Name;
                taskcomment.ModifiedOn = DateTime.Now;
                taskcomment.CreatedOn = DateTime.Now;
                taskcomment.CreatedById = _getEditor(currentUserId).Id;
                taskcomment.CreatedByName = _getEditor(currentUserId).Name;
                taskcomment.ModifiedById = _getEditor(currentUserId).Id;
                taskcomment.ModifiedByName = _getEditor(currentUserId).Name;
                taskcomment.ModifiedOn = DateTime.Now;
                UnitOfWork.RepositoryTaskComment.Add(taskcomment);

                var task = UnitOfWork.RepositoryTask.GetById(taskresolution.TaskId);
                var state = UnitOfWork.RepositoryTaskState.GetSpecific(s => new { s.Id, s.Name }, w => w.Name == "Resolved").Single();
                task.TaskStateId = state.Id;
                task.TaskStateName = state.Name;
                task.StateId = _getInactiveStates().Id;
                task.StateName = _getInactiveStates().Name;

                UnitOfWork.RepositoryTask.Update(task);

                UnitOfWork.Commit();


                //preparePage(entity_name, "Update");
                //getTaskDetails(taskcomment.TaskId);
                //var task = UnitOfWork.RepositoryTask.GetById(taskcomment.TaskId);
                //return View("UpdateTask", task);

       
                preparePage(entity_name, "Index");
                var tasks = getTasks(currentUserId).OrderBy(o => o.ProjectName);
                return View("IndexTask", tasks);


            }
            else
            {
                return Json(new { success = "error" }, JsonRequestBehavior.AllowGet);
            }
        
             
        }

        public ActionResult gotoAssignUserForm(Guid task_Id)
        {
            preparePage(entity_name, "Update");
            var task = UnitOfWork.RepositoryTask.GetById(task_Id);
            return PartialView("_AssignTask", task);
        }

        public ActionResult gotoLinkTaskForm(Guid task_Id)
        {
            return PartialView("_LinkTask");
        }

        public ActionResult gotoResolveTaskForm()
        {
            var task_state = UnitOfWork.RepositoryTaskState.GetAll().ToList();
            ViewData["TaskState"] = task_state;
            return PartialView("_ResolveTask");
        }

        public ActionResult UpdateTask(Task task)
        {
            var taskToUpdate = validateUpdate(task);
            if (taskToUpdate != null)
            {
                UnitOfWork.RepositoryTask.Update(taskToUpdate);
                UnitOfWork.Commit();
                preparePage(entity_name, "Update");
                getTaskDetails(taskToUpdate.Id);
                return View("UpdateTask", taskToUpdate);
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View(task);
            }
        }

        public Task validateUpdate(Task taskWithNewValues)
        {
            var taskWithOldValues = UnitOfWork.RepositoryTask.GetById(taskWithNewValues.Id);
            if (taskWithOldValues != null)
            {
                if (taskWithNewValues.Name != null)
                    taskWithOldValues.Name = taskWithNewValues.Name;
                if (taskWithNewValues.Description != null)
                    taskWithOldValues.Description = taskWithNewValues.Description;
                if (taskWithNewValues.StartTime != null)
                    taskWithOldValues.StartTime = taskWithNewValues.StartTime;
                if (taskWithNewValues.EndTime != null)
                    taskWithOldValues.EndTime = taskWithNewValues.EndTime;
                if (taskWithNewValues.TaskStateId != Guid.Empty)
                    taskWithOldValues.TaskStateId = taskWithNewValues.TaskStateId;
                if (taskWithNewValues.TaskStateName != null)
                    taskWithOldValues.TaskStateName = taskWithNewValues.TaskStateName;
                if (taskWithNewValues.AssigneeId != Guid.Empty)
                    taskWithOldValues.AssigneeId = taskWithNewValues.AssigneeId;
                if (taskWithNewValues.AssigneeName != null)
                    taskWithOldValues.AssigneeName = taskWithNewValues.AssigneeName;
                if (taskWithNewValues.ReporterId != Guid.Empty)
                    taskWithOldValues.ReporterId = taskWithNewValues.ReporterId;
                if (taskWithNewValues.ReporterName != null)
                    taskWithOldValues.ReporterName = taskWithNewValues.ReporterName;
                taskWithOldValues.IsLinked = taskWithNewValues.IsLinked;
                if (taskWithNewValues.StateName != null)
                    taskWithOldValues.StateName = taskWithNewValues.StateName;
                if (taskWithNewValues.StateId != Guid.Empty)
                    taskWithOldValues.StateId = taskWithNewValues.StateId;
                Guid currentUserId = _getCurrentUserId();
                taskWithOldValues.ModifiedById = _getEditor(currentUserId).Id;
                taskWithOldValues.ModifiedByName = _getEditor(currentUserId).Name;
                taskWithOldValues.ModifiedOn = DateTime.Now;
                return taskWithOldValues;
            }
            return null;
        }

        public JsonResult OtherRecords(Guid task_id)
        {
            var tasks = UnitOfWork.RepositoryTask.GetAll().Where(w => w.Id != task_id).ToList();
            return Json(new { success = tasks }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LinkTasks(Guid taskid1, Guid taskid2)
        {
            if (taskid1 != Guid.Empty && taskid2 != Guid.Empty)
            {
                var _task1 = UnitOfWork.RepositoryTask.GetById(taskid1);
                _task1.IsLinked = true;
                var _task2 = UnitOfWork.RepositoryTask.GetById(taskid2);
                _task2.IsLinked = true;
                var linkedTask = new LinkedTask();
                linkedTask.Id = Guid.NewGuid();
                linkedTask.TaskId1 = _task1.Id;
                linkedTask.TaskName1 = _task1.Name;
                linkedTask.TaskId2 = _task2.Id;
                linkedTask.TaskName2 = _task2.Name;
                linkedTask.Name = _task1.Name + " - " + _task2.Name;
                UnitOfWork.RepositoryLinkedTask.Add(linkedTask);


                var taskToUpdate = validateUpdate(_task1);
                UnitOfWork.RepositoryTask.Update(taskToUpdate);
                taskToUpdate = validateUpdate(_task2);
                UnitOfWork.RepositoryTask.Update(taskToUpdate);


                UnitOfWork.Commit();


                preparePage(entity_name, "Update");
                getTaskDetails(_task1.Id);
                return View("UpdateTask", _task1);


            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View();
            }
        }

        public JsonResult getOtherTasks(Guid task_id)
        {
            var tasks = UnitOfWork.RepositoryTask.GetAll().Where(w => w.Id != task_id).ToList().Take(10);
            return Json(new { success = tasks }, JsonRequestBehavior.AllowGet);
        }

    }
}
