﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LmsTool.Models;
using LmsTool.Models.DbModels;
using LmsTool.Models.Viewmodels;
using System.IO;

namespace LmsTool.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class ModulController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modul
        public ActionResult Index(int id, string error)
        {
            var course = db.Courses.Find(id);
    
            ViewBag.ErrorModul = error;
            var moduls = db.Moduls.Include(m => m.Activities).Where(c => c.CourseId == id)
                .OrderBy(d => d.StartDate);
            ViewBag.CourseName = course.Name;
            ViewBag.CurrentCourse = course.Id;

            List<ViewModuls> model = new List<ViewModuls>();
            if (moduls.Any())
            {

                foreach (var modul in moduls)
                {
                    model.Add(new ViewModuls
                    {
                        NrOfActivitys = modul.Activities.Count,
                        CourseId = modul.CourseId,
                        Description = modul.Description,
                        EndDate = modul.EndDate,
                        StartDate = modul.StartDate,
                        Id = modul.Id,
                        Name = modul.Name,
                        Document = modul.Document
                    });
                }
            }



            return View(model);
        }

        // GET: Modul/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModulModel modulModel = db.Moduls.Find(id);
            if (modulModel == null)
            {
                return HttpNotFound();
            }
            return View(modulModel);
        }

        // GET: Modul/Create
        public ActionResult Create(int id)
        {
            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");
            ModulModel model = new ModulModel { CourseId = id };

            return PartialView(model);
        }

        // POST: Modul/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,file")] ModulModel modulModel, HttpPostedFileBase file)
        {


            if (ModelState.IsValid)
            {

                var query = db.Courses.Find(modulModel.CourseId);

                if (file != null && file.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Documents"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    modulModel.Document = file.FileName;
                }

                if (query.StartDate > modulModel.StartDate)
                {
                    modulModel.StartDate = query.StartDate;
                    

                    db.Moduls.Add(modulModel);
                    db.SaveChanges();


                    return RedirectToAction("Index", new
                    {
                        id = modulModel.CourseId,
                        error = "Din modul hade ett start datum som var tidgare än kursens, modulens start fick samma värde som kursens"
                    });
                }
                var startDate = modulModel.StartDate.Date;
                var endDate = modulModel.EndDate.Date;

                modulModel.StartDate = startDate.AddHours(8);
                modulModel.EndDate = endDate.AddHours(17);
                
                db.Moduls.Add(modulModel);
                db.SaveChanges();
                return RedirectToAction("Index", "Modul", new { id = modulModel.CourseId});
            }

            //ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", modulModel.CourseId);
            return PartialView(modulModel);
        }

        // GET: Modul/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            ModulModel modulModel = db.Moduls.Find(id);
            var course = db.Courses.Find(modulModel.CourseId);
            modulModel.Course = course;
            if (modulModel == null)
            {
                return HttpNotFound();
            }

            return PartialView(modulModel);
        }

        // POST: Modul/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,Document")] ModulModel modulModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Documents"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    modulModel.Document = file.FileName;
                }

                db.Entry(modulModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", modulModel.CourseId);
            return View(modulModel);
        }

        // GET: Modul/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModulModel modulModel = db.Moduls.Find(id);
            modulModel.Course = db.Courses.Find(modulModel.CourseId);
            if (modulModel == null)
            {
                return HttpNotFound();
            }
            return PartialView(modulModel);
        }

        // POST: Modul/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModulModel modulModel = db.Moduls.Find(id);
            db.Moduls.Remove(modulModel);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public FileResult Download(string FileName)
        {
            var FileVirtualPath = "~/Documents/" + FileName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
