using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TaskOrganizer;
using TaskOrganizer.database;
using TaskOrganizer.Models;

namespace TaskOrganizer.Controllers
{
    
    public class UserMvcController : Controller
    {
        //get userwise tasks
        public ActionResult GetUserWiseTask(string username)
        {
            using (ETOEntities eto = new ETOEntities())
            {
                var result = (from user in eto.UserDetails
                              join task in eto.TaskDetails on user.UserId equals task.UserId
                              where task.Assignee == username
                              select new Task { TaskId = task.TaskId, TaskName=task.TaskName, TaskDescription= task.TaskDescription, Assignee= task.Assignee, DueDate= task.DueDate, CompletedBy= task.CompletedBy, CompletedDate= task.CompletedDate, Status= task.Status, Priority=  task.Priority, Stage= task.Stage , TaskCreationDate = task.TaskCreationDate,UserId=task.UserId}).ToList();
                             // select task).ToList();
                
                return View(result);
            }
        }

        //get all tasks 
        public ActionResult GetAllTasks()
        {
            using (ETOEntities eto = new ETOEntities())
            {
                var result = (from task in eto.TaskDetails
                              select new Task { TaskId = task.TaskId, TaskName=task.TaskName, TaskDescription= task.TaskDescription, Assignee= task.Assignee, DueDate= task.DueDate, CompletedBy= task.CompletedBy, CompletedDate= task.CompletedDate, Status= task.Status, Priority=  task.Priority, Stage= task.Stage , TaskCreationDate = task.TaskCreationDate,UserId=task.UserId}).OrderBy(t=>t.Assignee).ToList();
                             
                return View(result);
            }
        }

        public ActionResult CreateTask()
        {
            ViewBag.AssigneeList = getAllUsers();
            return View();
        }

        [HttpPost]
        public ActionResult CreateTask(TaskDetail task)
        {
            using (ETOEntities eto = new ETOEntities())
            {
                int id = (from user in eto.UserDetails
                          where user.UserName == User.Identity.Name
                          select user.UserId).FirstOrDefault();
                task.UserId = id;
                eto.TaskDetails.Add(task);
                eto.SaveChanges();

                TaskHistory th = new TaskHistory();
                th.UserId = id;
                th.TaskId = task.TaskId;
                th.Date = DateTime.Now;
                th.Action = User.Identity.Name + " " + "Created a task ";
                eto.TaskHistories.Add(th);
                eto.SaveChanges();

                return RedirectToAction("GetUserWiseTask","UserMvc", User.Identity.Name);
            }
           
        }
       
        //total count of tasks of all user
        public ActionResult AdminDashboard()
        {
            List<Dashboard> tasks = new List<Dashboard>();
         
            using (ETOEntities eto = new ETOEntities())
            {
                var result = eto.sp_GetDashboardData("-1").ToList();
                
                foreach (var item in result)
                {
                    var temp = new Dashboard();
                    temp.Assignee = item.Assignee;
                    temp.Completed = item.Completed ?? 0;
                    temp.Pending = item.Pending ?? 0;
                    temp.InProgress = item.InProgress ?? 0;
                    temp.OnHold = item.OnHold ?? 0;
                    temp.NotStarted = item.NotStarted ?? 0;
                    tasks.Add(temp);
                }

            }
            return View(tasks);
        }

        //total count of tasks of per user
        public ActionResult UserDashboard(string username)
        {
            List<Dashboard> tasks = new List<Dashboard>();

            using (ETOEntities eto = new ETOEntities())
            {
                var result = eto.sp_GetDashboardData(User.Identity.Name).ToList();
                var temp = new Dashboard();
                foreach (var item in result)
                {
                    temp.Assignee = item.Assignee;
                    temp.Completed = item.Completed ?? 0;
                    temp.Pending = item.Pending ?? 0;
                    temp.InProgress = item.InProgress ?? 0;
                    temp.OnHold = item.OnHold ?? 0;
                    temp.NotStarted = item.NotStarted ?? 0;
                    tasks.Add(temp);
                }

            }
            return View(tasks);
        }
       
        //Get edit details of specific task
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            using (ETOEntities eto = new ETOEntities())
            {
                
                Task task = null;
                task = eto.TaskDetails.Where(x => x.TaskId == id).Select(x => new Task()
                {

                    TaskId = x.TaskId,
                    UserId = x.UserId,
                    TaskName = x.TaskName,
                    TaskDescription = x.TaskDescription,
                    DueDate = x.DueDate,
                    Assignee = x.Assignee,
                    CompletedDate = x.CompletedDate,
                    CompletedBy = x.CompletedBy,
                    TaskCreationDate = x.TaskCreationDate,
                    Status = x.Status,
                    Prioritytext = (priority)x.Priority,
                    Stage = x.Stage,
                    Priority = x.Priority


                }).FirstOrDefault<Task>();
                ViewBag.AssigneeList = getAllUsers();
                //get task history
                task.taskHistory = eto.TaskHistories.Where(x => x.TaskId == id).Select(x => x.Action).ToList();
                
                return View(task);
            }
        }

        //get all users for assignee list
        public IList<SelectListItem> getAllUsers()
        {
            List<SelectListItem> assignee = new List<SelectListItem>();

            using (ETOEntities eto = new ETOEntities())
            {
                var result = (from user in eto.UserDetails
                              select user.UserName).ToList();
                foreach (var item in result)
                {
                    assignee.Add(new SelectListItem { Text = item, Value = item });
                }
                return assignee;
            }
           
        }

        //save the edited task
        [HttpPost]
        public ActionResult Edit(Task task)
        {

            using (ETOEntities eto = new ETOEntities())
            {
                int id = (from user in eto.UserDetails
                          where user.UserName == User.Identity.Name
                              select user.UserId).FirstOrDefault();

                TaskDetail tsk = eto.TaskDetails.Find(task.TaskId);
                TaskHistoryDetails th = CreateTaskHistory(tsk, task);
                TaskHistory tHistory = new TaskHistory();
                tHistory.TaskId = th.TaskId;
                tHistory.UserId = id;
                tHistory.Action = th.Action;
                tHistory.Date = th.Date;

                eto.TaskHistories.Add(tHistory);

                tsk.UserId = id;
                tsk.Assignee = task.Assignee;
                tsk.TaskName = task.TaskName;
                tsk.DueDate = task.DueDate;
                tsk.TaskDescription = task.TaskDescription;
                tsk.Stage = task.Stage;
                tsk.Priority = (int)task.Prioritytext;
                if (task.Stage == "Completed")
                {
                    tsk.CompletedDate = DateTime.Now;
                    tsk.CompletedBy = task.CompletedBy;
                    tsk.Status = true;
                }
                else
                {
                    tsk.CompletedDate = null;
                    tsk.CompletedBy = null;
                    tsk.Status = false;
                }
                eto.SaveChanges();
                task.taskHistory = eto.TaskHistories.Where(x => x.TaskId == th.TaskId).Select(x => x.Action).ToList();
            }
           
            ViewBag.AssigneeList = getAllUsers();
            return View(task);
        }

        private TaskHistoryDetails CreateTaskHistory(TaskDetail tsk, Task task)
        {
            string username = User.Identity.Name;
            string assignee = " Changed Assignee to ";
            string taskName = " Changed Task name  ";
            string taskDescription = " Changed Task Description";
            string dueDate = " Changed Due Date to ";
            string priority = " Changed Priority to ";
            string stage = " Changed Stage to ";

            TaskHistoryDetails th = new TaskHistoryDetails();
            th.TaskId = tsk.TaskId;
            th.UserId = tsk.UserId;
            th.Date = DateTime.Now;
            if (tsk.Assignee != task.Assignee)
            {
                th.Action += username + assignee + task.Assignee + " |";
            }
            if (tsk.TaskName != task.TaskName)
            {
                th.Action += username + taskName  + " |";
            }
            if (tsk.TaskDescription != task.TaskDescription)
            {
                th.Action += username + taskDescription + " |";
            }
            if (tsk.DueDate != task.DueDate)
            {
                th.Action += username + dueDate + task.DueDate + " |";
            }
            if (tsk.Priority != (int)task.Prioritytext)
            {
                th.Action += username + priority + task.Prioritytext + " |";
            }
            if (tsk.Stage != task.Stage)
            {
                th.Action += username + stage + task.Stage + " |";
            }
            return th;
        }

        //delete task
        public ActionResult Delete(int? id)
        {
            using (ETOEntities eto = new ETOEntities())
            {
                TaskDetail task = eto.TaskDetails.Find(id);
                eto.TaskDetails.Remove(task);
                eto.SaveChanges();
                return RedirectToAction("Task", "UserMvc");
            }

        }
    }
}