using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TaskOrganizer;
using TaskOrganizer.database;
using TaskOrganizer.Models;
using TaskOrganizer.Report;

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
                              select new Task { TaskId = task.TaskId, TaskName=task.TaskName, TaskDescription= task.TaskDescription, Assignee= task.Assignee, DueDate= (DateTime)task.DueDate, CompletedBy= task.CompletedBy, CompletedDate= task.CompletedDate, Status= task.Status, Priority=  task.Priority, Stage= task.Stage , TaskCreationDate = task.TaskCreationDate,UserId=task.UserId}).ToList();
                             // select task).ToList();
                
                return View(result);
            }
        }

        //get all tasks 
        public ActionResult GetAllTasks()
        {
            ViewBag.AssigneeList = getAllUsers();
            return View();
        }

        public ActionResult CreateTask()
        {
            ViewBag.AssigneeList = getAllUsers();
            return View();
        }

        [HttpPost]
        public ActionResult CreateTask(Task task)
        {
            using (ETOEntities eto = new ETOEntities())
            {
                int id = (from user in eto.UserDetails
                          join tsk in eto.TaskDetails on user.UserId equals tsk.UserId
                          where tsk.Assignee == task.Assignee
                          select user.UserId).FirstOrDefault();

                TaskDetail taskDetail = new TaskDetail();
                taskDetail.UserId = id;
                taskDetail.TaskId = task.TaskId;
                taskDetail.TaskName = task.TaskName;
                taskDetail.TaskDescription = task.TaskDescription;
                taskDetail.Assignee = task.Assignee;
                taskDetail.DueDate = task.DueDate;
                taskDetail.Priority = (int)task.Prioritytext;
                taskDetail.TaskCreationDate = DateTime.Now;
                taskDetail.Stage = task.Stage;
                
                eto.TaskDetails.Add(taskDetail);
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
            List<CommentHistory> cmt = new List<CommentHistory>();
            using (ETOEntities eto = new ETOEntities())
            {
                
                Task task = null;
                task = eto.TaskDetails.Where(x => x.TaskId == id).Select(x => new Task()
                {

                    TaskId = x.TaskId,
                    UserId = x.UserId,
                    TaskName = x.TaskName,
                    TaskDescription = x.TaskDescription,
                    DueDate = (DateTime)x.DueDate,
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
                var result = (from comment in eto.CommentHistories
                       where comment.TaskId == id
                       orderby comment.Commented_On descending
                       select new  { Comment = comment.Comment, Commenter = comment.Commenter, Commented_On = comment.Commented_On }).ToList();  //eto.CommentHistories.Where(x => x.TaskId == id).Select(x => new CommentHistory { Comment = x.Comment, Commenter = x.Commenter, Commented_On = x.Commented_On }).ToList();
                foreach (var item in result)
                {
                    CommentsHistory comnt = new CommentsHistory();
                    comnt.Commenter = item.Commenter;
                    comnt.Comment = item.Comment;
                    comnt.Commented_On = (DateTime)item.Commented_On;
                    task.commentHistory.Add(comnt);
                }
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
                assignee.Add(new SelectListItem { Text = "All", Value = "All" });
                return assignee;
            }
           
        }

        //save the edited task
        [HttpPost]
        public ActionResult Edit(Task task)
        {
            List<CommentHistory> cmt = new List<CommentHistory>();
            using (ETOEntities eto = new ETOEntities())
            {
                int id = (from user in eto.UserDetails
                          where user.UserName == User.Identity.Name
                              select user.UserId).FirstOrDefault();

                TaskDetail tsk = eto.TaskDetails.Find(task.TaskId);

                //create and add task history
                TaskHistoryDetails th = CreateTaskHistory(tsk, task);
                TaskHistory tHistory = new TaskHistory();
                tHistory.TaskId = th.TaskId;
                tHistory.UserId = id;
                tHistory.Action = th.Action;
                tHistory.Date = th.Date;

                eto.TaskHistories.Add(tHistory);
                //create and add comment
                CommentHistory comm = new CommentHistory();
                comm.TaskId = th.TaskId;
                comm.UserId = th.UserId;
                comm.Commenter = task.Assignee;
                comm.Commented_On = DateTime.Now;
                comm.Comment = task.comment;
                eto.CommentHistories.Add(comm);

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
                var result = (from comment in eto.CommentHistories
                       where comment.TaskId == th.TaskId
                              select new { Comment = comment.Comment, Commenter = comment.Commenter, Commented_On = comment.Commented_On }).ToList();
                foreach (var item in result)
                {
                    CommentsHistory comnt = new CommentsHistory();
                    comnt.Commenter = item.Commenter;
                    comnt.Comment = item.Comment;
                    comnt.Commented_On = (DateTime)item.Commented_On;
                    task.commentHistory.Add(comnt);
                }
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
       // populate data for dashboard
        public JsonResult DashboardStageCount()
        {
            int[] stageCount = new int[5];
            
            using (ETOEntities eto = new ETOEntities())
            {
                if (User.Identity.Name=="Admin")
                {
                    var result = eto.sp_GetDashboardData("-1").ToList();
                    foreach (var item in result)
                    {
                        stageCount[0] = stageCount[0] + (item.Pending ?? 0);
                        stageCount[1] = stageCount[1] + (item.OnHold ?? 0);
                        stageCount[2] = stageCount[2] + (item.InProgress ?? 0);
                        stageCount[3] = stageCount[3] + (item.NotStarted ?? 0);
                        stageCount[4] = stageCount[4] + (item.Completed ?? 0);
                    }
                }
                else
                {
                    var result = eto.sp_GetDashboardData(User.Identity.Name).ToList();
                    foreach (var item in result)
                    {
                        stageCount[0] = stageCount[0] + (item.Pending ?? 0);
                        stageCount[1] = stageCount[1] + (item.OnHold ?? 0);
                        stageCount[2] = stageCount[2] + (item.InProgress ?? 0);
                        stageCount[3] = stageCount[3] + (item.NotStarted ?? 0);
                        stageCount[4] = stageCount[4] + (item.Completed ?? 0);
                    }
                }

            }
            ViewBag.TotalTasks = stageCount[0] + stageCount[1] + stageCount[2] + stageCount[3] + stageCount[4];
            return Json(new { stageCount,totalCount= ViewBag.TotalTasks }, JsonRequestBehavior.AllowGet);
        }


        //populate pending tasks for specific user

        public ActionResult PendingTasksPerUser()
        {
            using (ETOEntities eto = new ETOEntities())
            {
                List<Task> PendingTasks = new List<Task>();
                var result = eto.sp_GetPendingTaskForUser(User.Identity.Name, DateTime.Today).ToList();
                foreach (var item in result)
                {
                    Task task = new Task();
                    task.TaskId = item.TaskId;
                    task.TaskName = item.TaskName;
                    task.Stage = item.stage;
                  
                    PendingTasks.Add(task);

                }
                return View(PendingTasks);
                
            }
        }

        public JsonResult GetFilteredData(string username,string stage,int priority,DateTime fromDate, DateTime toDate)
        {
           
            using (ETOEntities eto = new ETOEntities())
            {
                if (username == "All")
                {
                    var result = eto.sp_GetFilteredData("-1", stage, priority, fromDate, toDate).ToList();
                    return Json(new { result });
                }
                else
                {
                    var result = eto.sp_GetFilteredData(username, stage, priority, fromDate, toDate).ToList();
                    return Json(new { result });
                }
               
               
            }
        }

        //create user
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(User user)
        {
            using (var client = new HttpClient())
            {
                UserDetail ud = new UserDetail();
                ud.UserName = user.UserName;
                ud.Password = user.Password;
                ud.Email = user.Email;
                ud.Mobile = user.Mobile;
                using (ETOEntities eto = new ETOEntities())
                {
                    eto.UserDetails.Add(ud);
                    eto.SaveChanges();
                }

                return RedirectToAction("AdminDashboard", "UserMvc");

            }

        }

        public FileResult ExportPdf(Task task)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //StringReader reader = new StringReader("Hello");
                //Document PdfFile = new Document(PageSize.A4);
                //PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                //PdfFile.Open();
                //PdfFile.Add(new Chunk("Hello people"));
                //XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                //PdfFile.Close();
                //return File(stream.ToArray(), "application/pdf", "ExportData.pdf");

                TaskReport report = new TaskReport();
                byte[] reportBytes = report.PrepareReport(task);
                return File(reportBytes, "application/pdf", "export.pdf");
            }
        }
    }
}