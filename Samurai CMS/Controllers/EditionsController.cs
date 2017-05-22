﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Samurai_CMS.DAL;
using Samurai_CMS.Models;

namespace Samurai_CMS.Controllers
{
    public class EditionsController : Controller
    {
        private readonly UnitOfWork _repositories = new UnitOfWork();

        // GET: Editions
        public ActionResult Index()
        {
            var editions = _repositories.EditionRepository.GetAll(includeProperties: "Conference");

            return View(editions.ToList());
        }

        // GET: Editions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var edition = _repositories.EditionRepository.GetById(id);

            if (edition == null)
            {
                return HttpNotFound();
            }

            return View(edition);
        }

        // GET: Editions/Create
        public ActionResult Create()
        {
            ViewBag.ConferenceId = new SelectList(_repositories.ConferenceRepository.GetAll(), "Id", "Name");

            return View();
        }

        // POST: Editions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Year,AbstractDeadline,PaperDeadline,ResultsDeadline,StartDate,EndDate,ConferenceId")] Edition edition)
        {
            if (ModelState.IsValid)
            {
                _repositories.EditionRepository.Insert(edition);
                _repositories.Complete();
               
                return RedirectToAction("Index");
            }

            ViewBag.ConferenceId = new SelectList(_repositories.ConferenceRepository.GetAll(), "Id", "Name", edition.ConferenceId);

            return View(edition);
        }

        // GET: Editions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var edition = _repositories.EditionRepository.GetById(id);

            if (edition == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConferenceId = new SelectList(_repositories.ConferenceRepository.GetAll(), "Id", "Name", edition.ConferenceId);

            return View(edition);
        }

        // POST: Editions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Year,AbstractDeadline,PaperDeadline,ResultsDeadline,StartDate,EndDate,ConferenceId")] Edition edition)
        {
            if (ModelState.IsValid)
            {
                _repositories.EditionRepository.Update(edition);
                _repositories.Complete();

                return RedirectToAction("Index");
            }

            ViewBag.ConferenceId = new SelectList(_repositories.ConferenceRepository.GetAll(), "Id", "Name", edition.ConferenceId);

            return View(edition);
        }

        // GET: Editions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var edition = _repositories.EditionRepository.GetById(id);
            if (edition == null)
            {
                return HttpNotFound();
            }
            return View(edition);
        }

        // POST: Editions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var edition = _repositories.EditionRepository.GetById(id);

            _repositories.EditionRepository.Delete(edition);
            _repositories.Complete();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repositories.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}