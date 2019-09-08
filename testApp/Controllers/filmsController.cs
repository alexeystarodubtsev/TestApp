﻿using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using testApp.Models;
using PagedList.Mvc;
using PagedList;

namespace testApp.Controllers
{
    public class filmsController : Controller
    {
        private FilmDBEntities db = new FilmDBEntities();

        // GET: films
        public ActionResult Index(int? page)
        {
            int pageSize = 18;
            int pageNumber = (page ?? 1);
            return View(db.films.ToList().ToPagedList(pageNumber, pageSize));
            
        }

        // GET: films/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            films film = db.films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // GET: films/Create
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated)
            {
                return HttpNotFound();
            }
            films film = new films();
            
            return View(film);

        }



        // POST: films/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FilmName,Desctiption,Director,Creator,Year,ImageUrl")] films film, HttpPostedFileBase Image)
        {
            if (Image != null)
            {
                // получаем имя файла
                film.Creator = System.Web.HttpContext.Current.User.Identity.Name;
                string fileName = System.IO.Path.GetFileName(Image.FileName);
                fileName = String.Format(@"{0}", System.Guid.NewGuid()) + Path.GetExtension(Image.FileName);

                // сохраняем файл в папку Files в проекте
                Image.SaveAs(Server.MapPath("~/Images/" + fileName));
                film.ImageUrl = "~/Images/" + fileName;
            }
            if (db.films.Any(o=> o.Director == film.Director && o.FilmName == film.FilmName && o.Year == film.Year))
            {
                ModelState.AddModelError(string.Empty, "Такой фильм уже есть");
                return View(film);
            }
            try
            {
                if (ModelState.IsValid)
                {

                    db.films.Add(film);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Не все поля заполнены");
            }
            return View(film);
        }
        


            // GET: films/Edit/5
            public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            films film = db.films.Find(id);

            if (film == null || System.Web.HttpContext.Current.User.Identity.Name != film.Creator)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // POST: films/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FilmName,Desctiption,Director,Creator,Year,ImageUrl")] films film, HttpPostedFileBase Image)
        {
            if (Image != null)
            {
                string fileName = System.IO.Path.GetFileName(Image.FileName);
                fileName = String.Format(@"{0}", System.Guid.NewGuid()) + Path.GetExtension(Image.FileName);
                 // сохраняем файл в папку Files в проекте
                Image.SaveAs(Server.MapPath("~/Images/" + fileName));
                film.ImageUrl = "~/Images/" + fileName;
            }
            try
            {
                if (ModelState.IsValid)
                {

                    db.Entry(film).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Не все поля заполнены");
            }
            return View(film);
        }

        // GET: films/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            films film = db.films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // POST: films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            films film = db.films.Find(id);
            db.films.Remove(film);
            db.SaveChanges();
            return RedirectToAction("Index");
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