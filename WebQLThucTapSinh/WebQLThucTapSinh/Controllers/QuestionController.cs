using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebQLThucTapSinh.Common;
using WebQLThucTapSinh.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace WebQLThucTapSinh.Controllers
{
    public class QuestionController : Controller
    {
        // GET: Question
        public ActionResult Index()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            // Lấy roleID
            var role = Convert.ToInt32(Session["Role"].ToString());
            // Đối với Leader
            if (role == 4)
            {
                //Sau khi làm sprint Login thì mở lại
                var personID = Session["Person"].ToString();
                var list = (from a in database.Person
                            join c in database.Task on a.PersonID equals c.PersonID
                            join d in database.Question on c.TaskID equals d.TaskID
                            where a.PersonID == personID
                            select new QuestionClass()
                            {
                                QuestionID = d.QuestionID,
                                Content = d.Content,
                                Answer = d.Answer,
                                A = d.A,
                                B = d.B,
                                C = d.C,
                                D = d.D,
                                TaskID = c.TaskID,
                                TaskName = c.TaskName,
                                PersonID = a.PersonID,
                                FullName = a.LastName + " " + a.FirstName
                            }).OrderBy(x => x.TaskID).ToList();
                var count = list.Count();
                return View(list);
            }
            //Đối với manager
            else
            {
                //Sau khi làm sprint Login thì mở lại
                var companyID = Session["CompanyID"].ToString();
                var list = (from a in database.Person
                            join c in database.Task on a.PersonID equals c.PersonID
                            join d in database.Question on c.TaskID equals d.TaskID
                            where a.CompanyID == companyID && a.RoleID == 4
                            select new QuestionClass()
                            {
                                QuestionID = d.QuestionID,
                                Content = d.Content,
                                Answer = d.Answer,
                                A = d.A,
                                B = d.B,
                                C = d.C,
                                D = d.D,
                                TaskID = c.TaskID,
                                TaskName = c.TaskName,
                                PersonID = a.PersonID,
                                FullName = a.LastName + " " + a.FirstName
                            }).OrderBy(x => x.TaskID).ToList();
                var count = list.Count();
                return View(list);
            }
        }

        public ActionResult Create()
        {
            SetViewBag(1);
            return View();
        }

        public void SetViewBag(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //Sau khi làm sprint Login thì mở lại
            var personID = Session["Person"].ToString();
            List<Task> Task = database.Task.Where(x => x.PersonID == personID).ToList();
            if (id == 1)
            {
                SelectList QueList = new SelectList(Task, "TaskID", "TaskName");
                ViewBag.QueList = QueList;
            }
            else
            {
                ViewBag.QueList = Task;
            }
        }

        public bool CreateQ(Question question)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                Question q = new Question();
                int count = database.Question.Count();
                q.QuestionID = count + 1;
                q.Content = question.Content;
                q.Answer = question.Answer;
                q.A = question.A;
                q.B = question.B;
                q.C = question.C;
                q.D = question.D;
                q.TaskID = question.TaskID;
                database.Question.Add(q);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Create(Question question)
        {
            if (CreateQ(question))
            {
                ModelState.AddModelError("", "Thêm Câu hỏi thành công");
            }
            else
            {
                ModelState.AddModelError("", "Thêm Câu hỏi thất bại");
            }
            SetViewBag(1);
            return View("Create");
        }

        public ActionResult CreateExcel()
        {
            SetViewBag(2);
            return View();
        }

        [HttpPost]
        public ActionResult CreateExcel(HttpPostedFileBase excelfile, int taskID)
        {
            // Kiểm tra file đó có tồn tại hay không
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                SetViewBag(2);
                ViewBag.Error = "Thêm File mới<br /> ";
                return View("CreateExcel");
            }
            else
            {
                // kiểm tra đuôi file có phải là file Excel hay không
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    // Khai báo đường dẫn
                    string path = Path.Combine("D:/", excelfile.FileName);
                    // Tạo đối tượng COM. Tạo một đối tượng COM cho mọi thứ được tham chiếu
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    //Lặp lại qua các hàng và cột và in ra bàn điều khiển khi nó xuất hiện trong tệp
                    //excel is not zero based!!
                    for (int i = 2; i < range.Rows.Count; i++)
                    {
                        Question q = new Question();
                        q.Content = ((Excel.Range)range.Cells[i, 2]).Text;
                        q.Answer = ((Excel.Range)range.Cells[i, 3]).Text;
                        q.A = ((Excel.Range)range.Cells[i, 4]).Text;
                        q.B = ((Excel.Range)range.Cells[i, 5]).Text;
                        q.C = ((Excel.Range)range.Cells[i, 6]).Text;
                        q.D = ((Excel.Range)range.Cells[i, 7]).Text;
                        q.TaskID = taskID;
                        CreateQ(q);
                    }

                    //cleanup
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //xuất các đối tượng com để dừng hoàn toàn quá trình excel chạy trong nền
                    Marshal.ReleaseComObject(range);
                    Marshal.ReleaseComObject(worksheet);
                    //đóng lại và xuất thông tin
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                    //thoát và xuất thông tin
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                    SetViewBag(2);
                    ViewBag.Error = "Thêm thành công<br /> ";
                    return View("CreateExcel");
                }
                else
                {
                    SetViewBag(2);
                    ViewBag.Error = "File không hợp lệ<br /> ";
                    return View("CreateExcel");
                }
            }

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Question.Find(id);
            SetViewBag(1);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Question q)
        {
            if (ModelState.IsValid)
            {
                if (Update(q))
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "cập nhật thất bại");
                }
            }
            SetViewBag(1);
            return View("Edit");
        }

        public bool Update(Question question)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Question.Find(question.QuestionID);
                model.Content = question.Content;
                model.Answer = question.Answer;
                model.TaskID = question.TaskID;
                model.A = question.A;
                model.B = question.B;
                model.C = question.C;
                model.D = question.D;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteQuestion(int id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                //Đếm table quesition có báo nhiêu row
                var count = database.Question.ToList().Count;
                // kiểm tra count và id có bằng nhau không
                if (count == id)
                {
                    database.Question.Remove(database.Question.Find(id));
                }
                else
                {
                    var modelend = database.Question.Find(count);
                    // Tìm model
                    var model = database.Question.Find(id);
                    // remove model đối đối tượng chứ không phải remove id
                    model.TaskID = modelend.TaskID;
                    model.Content = modelend.Content;
                    model.Answer = modelend.Answer;
                    model.A = modelend.A;
                    model.B = modelend.B;
                    model.C = modelend.C;
                    model.D = modelend.D;
                    database.Question.Remove(modelend);
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        // Chú ý Method này được gọi trong file JS admin
        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            if (DeleteQuestion(id))
            {
                result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);
            }
            else
            {
                result = JsonConvert.SerializeObject("Xóa thất bại", Formatting.Indented, json);
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListQuestion(long? taskid = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            string taikhoan = Session["TenTK"].ToString();
            var listQuestion = database.Question.Where(x => x.TaskID == taskid).ToList();
            var model = database.Task.SingleOrDefault(c=>c.TaskID == taskid);
            int count = listQuestion.Count() - Convert.ToInt32(model.NumberOfQuestions);
            for (int i = 0; i < count; i++)
            {
                Random rd = new Random();
                int j = rd.Next(count);
                listQuestion.RemoveAt(j);
            }
            return PartialView(listQuestion);
        }

        public bool UpdateAnswer(string id, int answer, int task)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var number = database.Task.SingleOrDefault(c => c.TaskID == task).Result;
            if(answer >= number)
            {
                TestResults testResults = new TestResults();
                testResults.ID = database.TestResults.Count() + 1;
                testResults.PersonID = id;
                testResults.TaskID = task;
                testResults.Answer = answer;
                database.TestResults.Add(testResults);
                var model = database.Intern.Find(id);
                model.Result = model.Result + 1;
                database.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}