using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class TeamController : _BaseController
    {
        private const string entity_name = "Team";
        public ActionResult Index()
        {
            Guid currentUserId = Guid.Empty;
            currentUserId = _getCurrentUserId();
            preparePage(entity_name, "Index");
            var teams = getTeams(currentUserId).OrderBy(o => o.Name);
            return View("IndexTeam", teams);
        }

        public List<Team> getTeams(Guid currentUserId)
        {
            var teams = new List<Team>();
            if (_getCurrentUserRole(currentUserId) == "System Administrator")
            {
                teams = UnitOfWork.RepositoryTeam.GetAll().ToList();
            }
            if (_getCurrentUserRole(currentUserId) == "Project Manager" || _getCurrentUserRole(currentUserId) == "System User")
            {
                var team_user = UnitOfWork.RepositoryTeamUser.GetAll().Where(w => w.UserId == currentUserId).ToList();
                var project_user = UnitOfWork.RepositoryProjectUser.GetAll().Where(w => w.UserId == currentUserId).ToList();
                foreach (var item in team_user)
                {
                    var team = UnitOfWork.RepositoryTeam.GetById(item.TeamId);
                    teams.Add(team);
                }
            }
            return teams;
        }

        public ActionResult gotoCreateTeamForm()
        {
            preparePage(entity_name, "Create");
            return View("CreateTeam");
        }
                
        public void getTeamDetails(Guid team_id)
        {
            Guid currentUserId = _getCurrentUserId();
            //get team based on Id 
            var team = UnitOfWork.RepositoryTeam.GetById(team_id);
            //get all data from bridge table between team and user where teamid is the same
            var teamuser = UnitOfWork.RepositoryTeamUser.GetAll().Where(w => w.TeamId == team_id).ToList();
            //get security role for user only
            var role = UnitOfWork.RepositoryRole.GetAll().Where(sr => sr.Name == "System User").FirstOrDefault();
            //get all user security role that have user has security role
            var srUSer = UnitOfWork.RepositoryRoleUser.GetAll().Where(u => u.RoleId == role.Id).ToList();
            var userNotInTeam = new List<User>();
            var userInTeam = new List<User>();
            foreach (var item in teamuser)
            {
                var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                userInTeam.Add(user);
            }
            foreach (var item in srUSer)
            {
                var user = UnitOfWork.RepositoryUser.GetById(item.UserId);
                //add only if it is a user and it does not have a team
                if (!userInTeam.Contains(user))
                {
                    userNotInTeam.Add(user);
                }
            }
            ViewData["Users"] = userInTeam;

        }

        public ActionResult CreateTeam(Team team)
        {
            if (team != null)
            {
                Guid currentUserId = _getCurrentUserId();
                var state = UnitOfWork.RepositoryTaskState.GetAll().Where(w => w.Name == "Open").Single();

                team.Id = Guid.NewGuid();
                team.StateId = _getActiveStates().Id;
                team.StateName = _getActiveStates().Name;
                team.CreatedById = _getEditor(currentUserId).Id;
                team.CreatedByName = _getEditor(currentUserId).Name;
                team.CreatedOn = DateTime.Now;
                team.ModifiedById = _getEditor(currentUserId).Id;
                team.ModifiedByName = _getEditor(currentUserId).Name;
                team.ModifiedOn = DateTime.Now;
                
                if (!UnitOfWork.RepositoryTask.GetAll().Where(w => w.Name == team.Name).Any())
                {
                    UnitOfWork.RepositoryTeam.Add(team);
                    UnitOfWork.Commit();
                    preparePage(entity_name, "Update");
                    getTeamDetails(team.Id);
                    var _team = UnitOfWork.RepositoryTeam.GetById(team.Id);
                    return View("UpdateTeam", _team);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult gotoUpdateTeamForm(Guid record_id)
        {
            preparePage(entity_name, "Update");
            getTeamDetails(record_id);
            var team = UnitOfWork.RepositoryTeam.GetById(record_id);
            //var team = UnitOfWork.RepositoryTeam.GetSpecific(s => new { s.Name, s.Id }, w => w.Id == record_id) as Team;
            return View("UpdateTeam", team);
        }

        public ActionResult gotoTeamAddUser(Guid record_id)
        {
            preparePage(entity_name, "Update");
            var teamuser = new TeamAddUser();
            teamuser.team = UnitOfWork.RepositoryTeam.GetById(record_id);
            var role = UnitOfWork.RepositoryRole.GetAll().Where(w => w.Name == "System User").Single();
            var userorle = UnitOfWork.RepositoryRoleUser.GetAll().Where(w => w.RoleId == role.Id);
            teamuser.users = new List<User>();
            foreach (var item in userorle)
            {
                teamuser.users.Add(UnitOfWork.RepositoryUser.GetById(item.UserId));
            }
            return PartialView("_AddUser", teamuser);
        }

        public ActionResult CreateTeamMember(Guid team_id, List<Guid>users)
        {
            if (users != null & team_id != null)
            {
                foreach (var item in users)
                {
                    var tm = new TeamUser();
                    tm.Id = Guid.NewGuid();
                    tm.UserId = item;
                    tm.TeamId = team_id;
                    UnitOfWork.RepositoryTeamUser.Add(tm);                   
                }
                UnitOfWork.Commit();
            }
            var team = UnitOfWork.RepositoryTeam.GetById(team_id);
            return View("UpdateTeam", team);
        }

    }
}
