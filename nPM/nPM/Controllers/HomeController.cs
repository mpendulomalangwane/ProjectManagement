using Google.DataTable.Net.Wrapper;
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
    public class HomeController : _BaseController
    {

        public ActionResult Index()
        {
            //setting up left pane
            _settingNavigationPane();
            return View();
        }


        public JsonResult getTaskCompletionStatusData()
        {
            var data = new System.Data.DataTable();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
            {
                using (var command = new SqlCommand("sp_TaskCompletionStatus", connection))
                {
                    Guid currentUserId = Guid.Empty;
                    currentUserId = _getCurrentUserId();

                    var projects = UnitOfWork.RepositoryProjectUser.GetSpecific(s => new { s.Id, s.ProjectId, s.UserId }, w => w.UserId == currentUserId);
                        if (projects != null)
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@UserId", currentUserId));
                            using (var adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(data);
                            }

                            var array = createChartData(data);
                            return Json(array, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("no data", JsonRequestBehavior.AllowGet);
                        }
                }
            }

        }

        public IEnumerable<ChartData> createChartData(System.Data.DataTable dt)
        {
            var data = new List<ChartData>();         
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new ChartData(row.ItemArray[0].ToString(), Convert.ToDouble(row.ItemArray[1].ToString())));
            }
            return data;
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
                /*var project_user = UnitOfWork.RepositoryProject.GetAll().Where(w => w.ManagerId == currentUserId).ToList();
                foreach (var item in project_user)
                {
                    projects.Add(item);
                }*/

                var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == current_user_id).ToList();
                foreach (var item in project_user)
                {
                    var _projects = UnitOfWork.RepositoryProject.GetAll().Where(w => w.Id == item.ProjectId).ToList();
                    foreach (var subitem in _projects)
                    {
                        var project = UnitOfWork.RepositoryProject.GetById(subitem.Id);
                        projects.Add(project);
                    }
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



    }
}
