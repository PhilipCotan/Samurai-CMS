﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Samurai_CMS.DAL;
using Samurai_CMS.Models;
using Samurai_CMS.ViewModels;

namespace Samurai_CMS.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly UnitOfWork _repositories = new UnitOfWork();

        // GET: Enrollments
        public ActionResult Index()
        {
            var enrollments = _repositories.EnrollmentRepository.GetAll(includeProperties: "Edition,Paper,Role,User");

            return View(enrollments.ToList());
        }

        // GET: Enrollments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enrollment = _repositories.EnrollmentRepository.GetById(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.EditionTitle = _repositories.EditionRepository.GetById(id).Title;
            ViewBag.RoleId = new SelectList(_repositories.RoleRepository.GetAll(), "Id", "Name");

            return View();
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id, EnrollmentViewModel enrollmentViewModel)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Conferences");
            }

            AuthorPaper paper = null;

            if (enrollmentViewModel.IsSpeaker)
            {
                var abstractFile = new byte[enrollmentViewModel.Abstract.InputStream.Length];
                enrollmentViewModel.Abstract.InputStream.Read(abstractFile, 0, abstractFile.Length);

                var paperFile = new byte[enrollmentViewModel.Paper.InputStream.Length];
                enrollmentViewModel.Paper.InputStream.Read(paperFile, 0, paperFile.Length);

                paper = new AuthorPaper
                {
                    AbstractFileName = enrollmentViewModel.Abstract.FileName,
                    Abstract = abstractFile,
                    PaperFileName = enrollmentViewModel.Paper.FileName,
                    Paper = paperFile,
                    Authors = enrollmentViewModel.Authors,
                    Keywords = enrollmentViewModel.Keywords,
                    Title = enrollmentViewModel.Title
                };

                _repositories.PaperRepository.Insert(paper); 
            }

            var roles = _repositories.RoleRepository.GetAll();
            var enumerableList = roles as IList<Role> ?? roles.ToList();
            int authorRoleId = enumerableList.First(r => r.Name == Roles.Author.ToString()).Id;
            int listenerRoleId = enumerableList.First(r => r.Name == Roles.Listener.ToString()).Id;

            var enrollment = new Enrollment
            {
                UserId = User.Identity.GetUserId(),
                EditionId = id.Value,
                PaperId = paper?.Id,
                RoleId = enrollmentViewModel.IsSpeaker ? authorRoleId : listenerRoleId,
                Affiliation = enrollmentViewModel.Affiliation
            };

            _repositories.EnrollmentRepository.Insert(enrollment);
            _repositories.Complete();

            return RedirectToAction("Details", "Editions", new { id = id });
        }

        // GET: Enrollments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enrollment = _repositories.EnrollmentRepository.GetById(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.EditionId = new SelectList(_repositories.EditionRepository.GetAll(), "Id", "Title");
            ViewBag.PaperId = new SelectList(_repositories.PaperRepository.GetAll(), "Id", "Title");
            ViewBag.RoleId = new SelectList(_repositories.RoleRepository.GetAll(), "Id", "Name");
            ViewBag.UserId = new SelectList(_repositories.UserRepository.GetAll(), "Id", "Name");

            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,EditionId,RoleId,PaperId,Affiliation")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _repositories.EnrollmentRepository.Update(enrollment);
                _repositories.Complete();

                return RedirectToAction("Index");
            }
            ViewBag.EditionId = new SelectList(_repositories.EditionRepository.GetAll(), "Id", "Title");
            ViewBag.PaperId = new SelectList(_repositories.PaperRepository.GetAll(), "Id", "Title");
            ViewBag.RoleId = new SelectList(_repositories.RoleRepository.GetAll(), "Id", "Name");
            ViewBag.UserId = new SelectList(_repositories.UserRepository.GetAll(), "Id", "Name");

            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enrollment = _repositories.EnrollmentRepository.GetById(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            _repositories.EnrollmentRepository.Delete(id);
            _repositories.Complete();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repositories.Complete();
            }
            base.Dispose(disposing);
        }
    }
}