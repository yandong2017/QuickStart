using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNetMvc.QuickStart.Models;

namespace AspNetMvc.QuickStart.Controllers
{
    public class StudentsController : Controller
    {
        private StudentDbContext db = new StudentDbContext();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password)
        {
            ViewBag.NameError = false;
            ViewBag.PasswordError = false;
            Student student = db.GetItemByName(UserName);
            if (student == null)
            {
                ViewBag.NameError = true;
                return View();
            }            
            if (student.Password.Equals(Password))
            {                
                return RedirectToAction("Details", new { Name = student.Name });
            }
            ViewBag.PasswordError = true;
            return View();
        }

        // GET: Students
        public ActionResult Index()
        {           
            return View(db.DictStudents.Values);
        }

        // GET: Students/Details/5
        public ActionResult Details(string Name)
        {
            if (Name == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.GetItemByName(Name); 
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Password,Gender,Major,EntranceDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                if (db.GetItemByName(student.Name) == null)
                {
                    db.Careate(student);
                    return RedirectToAction("Index");
                }
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string Name)
        {
            if (Name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.GetItemByName(Name);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Password,Gender,Major,EntranceDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Save(student);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(string Name)
        {
            if (Name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.GetItemByName(Name);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Name)
        {
            Student student = db.GetItemByName(Name);
            db.Remove(student);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }      
    }
}
