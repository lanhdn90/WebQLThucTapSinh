using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using WebQLThucTapSinh.Common;
using WebQLThucTapSinh.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace WebQLThucTapSinh.Controllers
{
    public class InternController : Controller
    {
        // GET: Intern
        public ActionResult Index()
        {
            var role = Convert.ToInt32(Session["Role"]);
            if(role == 6)
            {
                return RedirectToAction("IndexOfSchool");
            }else if(role == 3)
            {
                return RedirectToAction("IndexOfFaculty");
            }
            else
            {
                return RedirectToAction("IndexOfCompany");
            }
        }

        public ActionResult IndexOfFaculty()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var id = Session["SchoolID"].ToString();
            var listIntern = database.Person.Where(x => x.SchoolID == id && x.RoleID == 5);
            var dem = listIntern.Count();
            List<Organization> listOrg = new List<Organization>();
            foreach (var item in listIntern)
            {
                var number = listOrg.Where(a => a.ID == item.CompanyID).Count();
                if (number == 0)
                {
                    listOrg.Add(database.Organization.SingleOrDefault(c => c.ID == item.CompanyID));
                }
            }
            var dem2 = listOrg.Count();
            ViewBag.listOrganization = new SelectList(listOrg, "ID", "Name");
            return View();
        }

        public ActionResult IndexOfSchool()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            string id = Session["Person"].ToString();
            var list = database.Organization.Where(x => x.PersonID == id).ToList();
            var companyID = database.Person.Find(id).CompanyID;
            list.Remove(list.SingleOrDefault(x => x.ID == companyID));
            ViewBag.listOrganization = new SelectList(list, "ID", "Name");
            return View();
        }

        public ActionResult IndexOfCompany(string id)
        {
            var model = listIShip();
            if(id == null)
            {
                Session["InternshipID"] = model[model.Count() - 1].InternshipID;
            }
            else
            {
                Session["InternshipID"] = id;
            }
            ViewBag.listin = model;
            return View();
        }

        public List<InternShip> listIShip()
        {
            var personID = Session["Person"].ToString();
            var role = Convert.ToInt32(Session["Role"]);
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<InternShip> model = new List<InternShip>();
            if (role == 4)
            {
                var mo = database.InternShip.Where(x => x.PersonID == personID).ToList();
                foreach (var item in mo)
                {
                    model.Add(item);
                }
            }
            else
            {
                var find = database.Person.Find(personID);
                var list = database.InternShip.Where(x => x.CompanyID == find.CompanyID);
                foreach (var item in list)
                {
                    model.Add(item);
                }
            }
            return model.ToList();
        }

        [HttpPost]
        public JsonResult GetCompanyById(string ID)
        {

            WebDatabaseEntities database = new WebDatabaseEntities();
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var listIntern = database.Person.Where(x => x.SchoolID == ID && x.RoleID == 5);
            List<Organization> listOrg = new List<Organization>();
            foreach (var item in listIntern)
            {
                var number = listOrg.Where(a => a.ID == item.CompanyID).Count();
                if (number == 0)
                {
                    listOrg.Add(database.Organization.SingleOrDefault(c => c.ID == item.CompanyID));
                }
            }
            SelectList list = new SelectList(listOrg, "ID", "Name");
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFacultyById(string ID)
        {

            WebDatabaseEntities database = new WebDatabaseEntities();
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var model = database.Organization.SingleOrDefault(x => x.ID == ID).PersonID;
            List<Organization> listOrg = listFaculty(model);
            SelectList list = new SelectList(listOrg, "ID", "Name");
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }

        public List<Organization> listFaculty(string id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var list = database.Organization.Where(x => x.PersonID == id).ToList();
            var companyId = database.Person.Find(id).CompanyID;
            list.Remove(list.SingleOrDefault(x => x.ID == companyId));
            return list;
        }

        public List<InternClass> ListIntern(string facultyID, string companyID, bool check)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<InternClass> listIntern = new List<InternClass>();
            if (check == false)
            {
                listIntern = (from a in database.Person
                              join b in database.Intern on a.PersonID equals b.PersonID into joinl1
                              from j in joinl1.DefaultIfEmpty()
                              join f in database.Organization on a.SchoolID equals f.ID into join4
                              from p in join4.DefaultIfEmpty()
                              where a.SchoolID == facultyID && a.RoleID == 5 && j.InternshipID == null
                              select new InternClass()
                              {
                                  StudentCode = j.StudentCode,
                                  PersonID = a.PersonID,
                                  FullName = a.LastName + " " + a.FirstName,
                                  Birthday = a.Birthday,
                                  CompanyID = a.CompanyID,
                                  NameOfCompany = database.Organization.FirstOrDefault(x => x.ID == a.CompanyID).Name,
                                  InternshipID = j.InternshipID,
                                  NameOfInternship = database.InternShip.FirstOrDefault(c => c.InternshipID == j.InternshipID).CourseName,
                                  SchoolID = a.SchoolID,
                                  NameOfSchool = p.Name,
                                  Result = j.Result,
                              }).ToList();
            }
            else
            {
                listIntern = (from a in database.Person
                              join b in database.Intern on a.PersonID equals b.PersonID into joinl1
                              from j in joinl1.DefaultIfEmpty()
                              join f in database.Organization on a.SchoolID equals f.ID into join4
                              from p in join4.DefaultIfEmpty()
                              where a.SchoolID == facultyID && a.RoleID == 5 && j.InternshipID != null
                              select new InternClass()
                              {
                                  StudentCode = j.StudentCode,
                                  PersonID = a.PersonID,
                                  FullName = a.LastName + " " + a.FirstName,
                                  Birthday = a.Birthday,
                                  CompanyID = a.CompanyID,
                                  NameOfCompany = database.Organization.FirstOrDefault(x => x.ID == a.CompanyID).Name,
                                  InternshipID = j.InternshipID,
                                  NameOfInternship = database.InternShip.FirstOrDefault(c => c.InternshipID == j.InternshipID).CourseName,
                                  SchoolID = a.SchoolID,
                                  NameOfSchool = p.Name,
                                  Result = j.Result,
                              }).ToList();
                if (companyID != null)
                {
                    listIntern = listIntern.Where(x => x.CompanyID == companyID).ToList();
                }
            }
            return listIntern;
        }

        public List<InternClass> ListInternOfCompany(string companyID, int internShipId)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<InternClass> listIntern = new List<InternClass>();
            if (internShipId == 0)
            {
                listIntern = (from a in database.Person
                              join b in database.Intern on a.PersonID equals b.PersonID into joinl1
                              from j in joinl1.DefaultIfEmpty()
                              join f in database.Organization on a.SchoolID equals f.ID into join4
                              from p in join4.DefaultIfEmpty()
                              where a.CompanyID == companyID && a.RoleID == 5 && j.InternshipID == null
                              select new InternClass()
                              {
                                  StudentCode = j.StudentCode,
                                  PersonID = a.PersonID,
                                  FullName = a.LastName + " " + a.FirstName,
                                  Birthday = a.Birthday,
                                  CompanyID = a.CompanyID,
                                  NameOfCompany = database.Organization.FirstOrDefault(x => x.ID == a.CompanyID).Name,
                                  InternshipID = j.InternshipID,
                                  NameOfInternship = database.InternShip.FirstOrDefault(c => c.InternshipID == j.InternshipID).CourseName,
                                  SchoolID = a.SchoolID,
                                  NameOfSchool = database.Organization.FirstOrDefault(h => h.ID == (database.Person.FirstOrDefault(c => c.PersonID == (database.Organization.FirstOrDefault(x => x.ID == a.SchoolID).PersonID)).CompanyID)).Name,

                                  Result = j.Result,
                              }).ToList();
            }
            else
            {
                listIntern = (from a in database.Person
                              join b in database.Intern on a.PersonID equals b.PersonID into joinl1
                              from j in joinl1.DefaultIfEmpty()
                              join f in database.Organization on a.SchoolID equals f.ID into join4
                              from p in join4.DefaultIfEmpty()
                              where a.CompanyID == companyID && a.RoleID == 5 && j.InternshipID != null
                              select new InternClass()
                              {
                                  StudentCode = j.StudentCode,
                                  PersonID = a.PersonID,
                                  FullName = a.LastName + " " + a.FirstName,
                                  Birthday = a.Birthday,
                                  CompanyID = a.CompanyID,
                                  NameOfCompany = database.Organization.FirstOrDefault(x => x.ID == a.CompanyID).Name,
                                  InternshipID = j.InternshipID,
                                  NameOfInternship = database.InternShip.FirstOrDefault(c => c.InternshipID == j.InternshipID).CourseName,
                                  SchoolID = a.SchoolID,
                                  NameOfSchool = database.Organization.FirstOrDefault(h => h.ID == (database.Person.FirstOrDefault(c => c.PersonID == (database.Organization.FirstOrDefault(x => x.ID == a.SchoolID).PersonID)).CompanyID)).Name,
                                  //NameOfSchool = p.Name,
                                  Result = j.Result,
                              }).ToList();
                if (internShipId != 0)
                {
                    listIntern = listIntern.Where(x => x.InternshipID == internShipId).ToList();
                }
            }
            return listIntern;
        }

        [HttpGet]
        public JsonResult LoadData(string facultyID, string companyID)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<InternClass> listInterns = new List<InternClass>();
            List<InternClass> listNotInterShips = new List<InternClass>();
            List<Organization> list = new List<Organization>();
            //Chưa có Khoa và công ty
            if (facultyID == "" && companyID == "")
            {
                var id = Session["Person"].ToString();
                list = listFaculty(id);
                foreach (var item in list)
                {
                    var listOne = ListIntern(item.ID, null, true);
                    foreach (var modelOne in listOne)
                    {
                        listInterns.Add(modelOne);
                    }
                    var listTwo = ListIntern(item.ID, null, false);
                    foreach (var modelTwo in listTwo)
                    {
                        listNotInterShips.Add(modelTwo);
                    }
                }
            }
            else
            {
                // Có Khoa nhưng chưa có công ty
                if (facultyID != null && companyID == "")
                {
                    listInterns = ListIntern(facultyID, null, true);
                    listNotInterShips = ListIntern(facultyID, null, false);
                }
                else
                //Có khoa và công ty
                {
                    listInterns = ListIntern(facultyID, companyID, true);
                    listNotInterShips = ListIntern(facultyID, companyID, false);
                }
            }
            listInterns = listInterns.OrderByDescending(x => x.SchoolID).ToList();
            listNotInterShips = listNotInterShips.OrderByDescending(x => x.SchoolID).ToList();
            int totalRowOne = listInterns.Count();
            int totalRowTwo = listNotInterShips.Count();
            return Json(new
            {
                dataOne = listInterns,
                totalOne = totalRowOne,
                dataTwo = listNotInterShips,
                totalTwo = listNotInterShips,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadDataOfCompany(int internShipId)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<InternClass> listInterns = new List<InternClass>();
            List<InternClass> listNotInterShips = new List<InternClass>();
            var companyid = Session["CompanyID"].ToString();
            if (internShipId == 0)
            {
                var list = listIShip();
                foreach (var item in list)
                {
                    var listOne = ListInternOfCompany(companyid, item.InternshipID);
                    foreach (var modelOne in listOne)
                    {
                        listInterns.Add(modelOne);
                    }
                }
            }
            else
            {
                var listOne = ListInternOfCompany(companyid, internShipId);
                foreach (var modelOne in listOne)
                {
                    listInterns.Add(modelOne);
                }
            }
            listInterns = listInterns.OrderByDescending(x => x.SchoolID).ToList();
            var listTwo = ListInternOfCompany(companyid, 0);
            foreach (var modelTwo in listTwo)
            {
                listNotInterShips.Add(modelTwo);
            }
            listNotInterShips = listNotInterShips.OrderByDescending(x => x.SchoolID).ToList();
            int totalRowOne = listInterns.Count();
            int totalRowTwo = listNotInterShips.Count();
            return Json(new
            {
                dataOne = listInterns,
                totalOne = totalRowOne,
                dataTwo = listNotInterShips,
                totalTwo = listNotInterShips,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public bool AddIntern(List<string> listIntern, int id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                foreach (var i in listIntern)
                {
                    database.Intern.Find(i).InternshipID = id;
                    database.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var role = Convert.ToInt32(Session["Role"]);
            SetViewBag();
            SetViewBagS();
            SetViewBagG();
            if (role != 3)
            {
                SetViewBagI();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(InternClass per)
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            if (ModelState.IsValid)
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                Person person = new Person();
                string personID;
                do
                {
                    personID = new Share().RandomText();
                } while (new Share().FindPerson(personID) == false);
                person.PersonID = personID;
                database.Person.Add(person);
                person.RoleID = 5;
                person.LastName = per.LastName;
                person.FirstName = per.FirstName;
                person.Birthday = per.Birthday;
                person.Gender = per.Gender;
                person.Address = per.Address;
                person.Phone = per.Phone;
                person.Email = per.Email;


                if (role == 3)
                {
                    var schoolID = Session["SchoolID"].ToString();
                    person.SchoolID = schoolID;
                    person.CompanyID = per.CompanyID;
                }
                else
                {
                    var companyID = Session["CompanyID"].ToString();
                    person.SchoolID = per.FacultyId;
                    person.CompanyID = companyID;
                }

                if (new Share().InsertPerson(person))
                {
                    if (SendMailTK(personID))
                    {
                        Intern intern = new Intern();
                        intern.PersonID = personID;
                        intern.StudentCode = per.StudentCode;
                        if (per.InternshipID != null)
                        {
                            intern.InternshipID = per.InternshipID;
                        }
                        intern.Result = 0;
                        InsertInt(intern);
                        ModelState.AddModelError("", "Thêm Thực tập sinh thành công");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể gửi Email kích hoạt");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Thực tập sinh thất bại");
                }

            }
            SetViewBag();
            SetViewBagS();
            SetViewBagG();
            if (role != 3)
            {
                SetViewBagI();
            }
            return View("Create");
        }

        [HttpGet]
        public ActionResult imPortExcel()
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            if(role == 3)
            {
                var list = new Share().listOrgan(2).ToList();
                ViewBag.listOrganization = new SelectList(list, "ID", "Name");
            }
            else
            {
                var list = new Share().listOrgan(6).ToList();
                ViewBag.listOrganization = new SelectList(list, "ID", "Name");
            }
            return View();
        }

        [HttpPost]
        public ActionResult imPortExcel(HttpPostedFileBase excelfile, string companyId, string FacultyId)
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            // Kiểm tra file đó có tồn tại hay không
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Thêm File mới<br /> ";
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
                    // Tạo application cái này là mở ms Excel
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    // Mở WorkBook Mở file Excel mình truyền vào
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    // Mở worksheet Mở sheet đầu tiên
                    Excel.Range range = worksheet.UsedRange;

                    //Lặp lại qua các hàng và cột và in ra bàn điều khiển khi nó xuất hiện trong tệp
                    //excel is not zero based!!
                    
                    if (role == 3)
                    {
                        var schoolID = Session["SchoolID"].ToString();
                        for (int i = 2; i < range.Rows.Count; i++)
                        {
                            Person person = new Person();
                            string personID;
                            do
                            {
                                personID = new Share().RandomText();
                            } while (new Share().FindPerson(personID) == false);
                            person.PersonID = personID;
                            person.LastName = ((Excel.Range)range.Cells[i, 3]).Text;
                            person.FirstName = ((Excel.Range)range.Cells[i, 4]).Text;
                            DateTime dateValue = DateTime.FromOADate(Convert.ToDouble(((Excel.Range)range.Cells[i, 5]).Value));
                            // Dòng code này có ý nghĩa là nó sẽ chuyển đối kiểu số thành ngày lại
                            person.Birthday = dateValue;
                            int gender = int.Parse(((Excel.Range)range.Cells[i, 6]).Text);
                            person.Gender = Convert.ToBoolean(gender);
                            person.Address = ((Excel.Range)range.Cells[i, 7]).Text;
                            person.Phone = ((Excel.Range)range.Cells[i, 8]).Text;
                            person.Email = ((Excel.Range)range.Cells[i, 9]).Text;
                            person.SchoolID = schoolID;
                            person.CompanyID = companyId;
                            person.RoleID = 5;
                            //listproducts.Add(product);

                            new Share().InsertPerson(person);
                            if (SendMailTK(personID))
                            {
                                Intern intern = new Intern();
                                intern.PersonID = personID;
                                intern.StudentCode = ((Excel.Range)range.Cells[i, 2]).Text;
                                intern.Result = 0;
                                InsertInt(intern);
                            }

                        }
                    }
                    else
                    {
                        var companyID = Session["CompanyID"].ToString();
                        for (int i = 2; i < range.Rows.Count; i++)
                        {
                            Person person = new Person();
                            string personID;
                            do
                            {
                                personID = new Share().RandomText();
                            } while (new Share().FindPerson(personID) == false);
                            person.PersonID = personID;
                            person.LastName = ((Excel.Range)range.Cells[i, 3]).Text;
                            person.FirstName = ((Excel.Range)range.Cells[i, 4]).Text;
                            //person.Birthday = DateTime.ParseExact(((Excel.Range)range.Cells[i, 4]).Text,"yyyy/MM/dd",null);
                            DateTime dateValue = DateTime.FromOADate(Convert.ToDouble(((Excel.Range)range.Cells[i, 5]).Value));
                            person.Birthday = dateValue;
                            int gender = int.Parse(((Excel.Range)range.Cells[i, 6]).Text);
                            //person.Gender = bool.Parse(Convert.ToUInt32(((Excel.Range)range.Cells[i, 5]).Value));
                            person.Gender = Convert.ToBoolean(gender);
                            person.Address = ((Excel.Range)range.Cells[i, 7]).Text;
                            person.Phone = ((Excel.Range)range.Cells[i, 8]).Text;
                            person.Email = ((Excel.Range)range.Cells[i, 9]).Text;
                            person.CompanyID = companyID;
                            person.SchoolID = FacultyId;
                            person.RoleID = 5;
                            //listproducts.Add(product);
                            new Share().InsertPerson(person);

                            if (SendMailTK(personID))
                            {
                                Intern intern = new Intern();
                                intern.PersonID = personID;
                                intern.StudentCode = ((Excel.Range)range.Cells[i, 2]).Text;
                                intern.Result = 0;
                                InsertInt(intern);
                            }

                        }
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
                    //ViewBag.ListProduct = listproducts;
                    //dem = listproducts.Count();
                    ViewBag.Error = "Thêm thành công<br /> ";
                }
                else
                {
                    ViewBag.Error = "File không hợp lệ<br /> ";
                }
            }
            if (role == 3)
            {
                var list = new Share().listOrgan(2).ToList();
                ViewBag.listOrganization = new SelectList(list, "ID", "Name");
            }
            else
            {
                var list = new Share().listOrgan(6).ToList();
                ViewBag.listOrganization = new SelectList(list, "ID", "Name");
            }
            return View("imPortExcel");
        }

        public void SetViewBag(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var list = database.Person.Where(x => x.RoleID == 2).ToList();
            List<Organization> Organ = new List<Organization>();
            foreach (var item in list)
            {
                var model = database.Organization.Find(item.CompanyID);
                Organ.Add(model);
            }
            SelectList OrganList = new SelectList(Organ, "ID", "Name");
            ViewBag.OrganList = OrganList;
        }

        public void SetViewBagS(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var list = database.Person.Where(x => x.RoleID == 6).ToList();
            List<Organization> Organ = new List<Organization>();
            foreach (var item in list)
            {
                var model = database.Organization.Find(item.CompanyID);
                Organ.Add(model);
            }
            SelectList SchoolList = new SelectList(Organ, "ID", "Name");
            ViewBag.SchoolList = SchoolList;
        }

        public void SetViewBagG(string selectedID = null)
        {
            SelectList GenGender = new SelectList(new[] {
                new {Text = "Nam", Value = true},
                new {Text = "Nữ", Value = false},
            }, "Value", "Text");
            ViewBag.GenGender = GenGender;
        }

        public void SetViewBagI(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var role = Convert.ToInt32(Session["Role"]);
            var list = new List<InternShip>();
            if (role == 4)
            {
                var per = Session["Person"].ToString();
                list = database.InternShip.Where(x => x.PersonID == per).ToList();
            }
            else
            {
                var companyID = Session["CompanyID"].ToString();
                list = database.InternShip.Where(x => x.CompanyID == companyID).ToList();
            }
            SelectList IList = new SelectList(list, "InternshipID", "CourseName");
            ViewBag.IList = IList;
        }
        
        public void InsertInt(Intern intern)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            database.Intern.Add(intern);
            database.SaveChanges();
        }

        public bool SendMailTK(string personID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var res = database.Person.Find(personID);
                string cus = res.LastName + " " + res.FirstName;
                var com = database.Organization.Find(res.CompanyID);
                string compa = com.Name;
                string email = res.Email;
                string nd = personID;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/SendEmailPerson.html"));
                content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{CompanyName}}", compa);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo đăng ký Tài khoản";
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.UseDefaultCredentials = false;
                client.Credentials = nc;
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ActionResult CVIntern(string id = null)
        {
            if (id == null)
            {
                id = Session["Person"].ToString();
            }
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Person.Find(id);
            var listIn = (from a in database.TestResults
                          join e in database.Task on a.TaskID equals e.TaskID
                          where a.PersonID == id
                          select new TestResultsClass()
                          {
                              PersonID = a.PersonID,
                              TaskID = a.TaskID,
                              TaskName = e.TaskName,
                              Answer = a.Answer,
                          }).OrderBy(x => x.TaskID).ToList();
            ViewBag.listI = listIn;
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var role = Convert.ToInt32(Session["Role"]);
            SetViewBag();
            SetViewBagS();
            SetViewBagG();
            if (role != 3)
            {
                SetViewBagI();
            }
            WebDatabaseEntities database = new WebDatabaseEntities();
            var findP = database.Person.Find(id);
            var findI = database.Intern.Find(id);
            InternClass model = new InternClass();
            model.PersonID = findP.PersonID;
            model.LastName = findP.LastName;
            model.FirstName = findP.FirstName;
            model.Birthday = findP.Birthday;
            model.Gender = findP.Gender;
            model.Address = findP.Address;
            model.Phone = findP.Phone;
            model.Email = findP.Email;
            model.Image = findP.Image;
            model.SchoolID = findP.SchoolID;
            model.SchoolID = findP.CompanyID;
            model.StudentCode = findI.StudentCode;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(InternClass intern)
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            if (ModelState.IsValid)
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Person.Find(intern.PersonID);
                model.LastName = intern.LastName;
                model.FirstName = intern.FirstName;
                model.Birthday = intern.Birthday;
                model.Gender = intern.Gender;
                model.Address = intern.Address;
                model.Phone = intern.Phone;
                var email = model.Email;
                model.Email = intern.Email;
                model.Image = intern.Image;
                if (role == 3 || role == 6)
                {
                    if(intern.CompanyID != null)
                    {
                        model.CompanyID = intern.CompanyID;
                    }
                    UpdateIntern(intern.PersonID, intern.StudentCode);
                }
                else
                {
                    if(intern.FacultyId != null)
                    {
                        model.SchoolID = intern.FacultyId;
                    }
                }
                database.SaveChanges();
                if(email != intern.Email)
                {
                    SendMailTK(intern.PersonID);
                }
               
                ModelState.AddModelError("", "Cập nhật Thực tập sinh thành công");
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật Thực tập sinh thất bại");
            }
            SetViewBag();
            SetViewBagS();
            SetViewBagG();
            if (role != 3)
            {
                SetViewBagI();
            }
            return View("Edit");
        }

        public bool UpdateIntern(string id, string studentCode)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model1 = database.Intern.Find(id);
                model1.StudentCode = studentCode;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            if(DeleteIntern(id))
            {
                result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);

            }
            else
            {
                result = JsonConvert.SerializeObject("Xóa thất bại", Formatting.Indented, json);

            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteIntern(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Intern.Find(id);
                var internshipID = model.InternshipID;
                database.Intern.Remove(model);
                var model2 = database.Users.SingleOrDefault(x => x.PersonID == id);
                if (model2 != null)
                {
                    database.Users.Remove(model2);
                }
                var model1 = database.Person.Find(id);
                database.Person.Remove(model1);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }


        }
    }
}