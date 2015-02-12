using Microsoft.Reporting.WebForms;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    public class ReportController : _BaseController
    {

        private const string entity_name = "Report";
        public ActionResult Index()
        {
            var reports = UnitOfWork.RepositoryReport.GetAll().ToList();
            preparePage(entity_name, "Index");
            return View("IndexReport", reports);
        }

        public ActionResult gotoUpdateReportForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            var report = UnitOfWork.RepositoryReport.GetById(record_id);
            return View("ViewReport", report);
        }

        public JsonResult getProjects(string term)
        {
            return Json(new { success = _getProjects(term) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getReport(Guid projectId, string format, string schema)
        {

            try
            {

                LocalReport local_report = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), schema + ".rdlc");

                if (System.IO.File.Exists(path))
                {
                    local_report.ReportPath = path;
                }

                #region Task Details

                var dtProjectName = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_ProjectName", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtProjectName);
                        }

                    }

                }

                var rdsProjectName = new ReportDataSource("dsProjectName", dtProjectName);
                local_report.DataSources.Add(rdsProjectName);

                #endregion

                #region Task Details

                var dtTaskDetails = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_TaskDetails", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtTaskDetails);
                        }

                    }

                }

                var rdsTaskDetails = new ReportDataSource("dsTaskDetails", dtTaskDetails);
                local_report.DataSources.Add(rdsTaskDetails);

                #endregion

                #region Task Completion Status

                var dtTaskCompletionStatus = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_TaskCompletionStatus", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtTaskCompletionStatus);
                        }

                    }

                }

                var rdsAllTaskPieChart = new ReportDataSource("dsTaskCompletionStatus", dtTaskCompletionStatus);
                local_report.DataSources.Add(rdsAllTaskPieChart);

                #endregion

                #region Daily Total Time Logged

                var dsProgressByDay = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_TaskTimeLogProgress", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dsProgressByDay);
                        }

                    }

                }

                var rdsProgressByDay = new ReportDataSource("dsTaskTimeLoggedProgress", dsProgressByDay);
                local_report.DataSources.Add(rdsProgressByDay);

                #endregion

                #region Task Complete Vs InProgress

                var dtTaskCompleteVsInProgress = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_TaskCompleteVsInProgress", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtTaskCompleteVsInProgress);
                        }

                    }

                }

                var rdsTaskCompleteVsInProgress = new ReportDataSource("dsTaskCompletedVsInProgress", dtTaskCompleteVsInProgress);
                local_report.DataSources.Add(rdsTaskCompleteVsInProgress);

                #endregion

                #region Uploaded Document

                var dtUploadedDoc = new DataTable();

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["nPM"].ConnectionString))
                {

                    using (var command = new SqlCommand("sp_UploadedDoc", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtUploadedDoc);
                        }

                    }

                }

                var rdsUploadedDoc = new ReportDataSource("dsUploadedDoc", dtUploadedDoc);
                local_report.DataSources.Add(rdsUploadedDoc);

                #endregion


                var reportType = format;
                string mimeType;
                string encoding;
                string fileNameExtension;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                var deviceInfo = string.Format(
                        "<DeviceInfo>" +
                        "<OutputFormat>" + format + "</OutputFormat>" +
                        "<PageWidth>8.5in</PageWidth>" +
                        "<PageHeight>11in</PageHeight>" +
                        "<MarginTop>0.2in</MarginTop>" +
                        "<MarginLeft>0.2in</MarginLeft>" +
                        "<MarginRight>0.2in</MarginRight>" +
                        "<MarginBottom>0.2in</MarginBottom>" +
                        "</DeviceInfo>");

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report
                renderedBytes = local_report.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //Response.Clear();
                //Response.ContentType =mimeType;
                //Response.AppendHeader("content-disposition", "inline; filename=foo.pdf");
                //Response.BinaryWrite(renderedBytes);
                //Response.End();

                return File(renderedBytes, mimeType);
            }
            catch(Exception )
            {
            }

            return View();

        }

    }
}
