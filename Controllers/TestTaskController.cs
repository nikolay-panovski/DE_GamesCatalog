using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using DE_GamesCatalog.Models;

namespace DE_GamesCatalog.Controllers
{
    public class TestTaskController : Controller
    {
        [Route("api/get/tasks")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("api/get/task/{id:int}")]
        [HttpGet]
        public ActionResult Details(int id)
        {
            IMongoCollection<TestTaskModel> tasks = Program.client.GetDatabase("project_manage_tasks").GetCollection<TestTaskModel>("tasks");

            //var found = tasks.Find(t => t.name == "Sample").ToJson();     // return type: IFindFluent<TestTaskModel, TestTaskModel>
                                                                            // leads down the ObjectSerializer rabbit hole
            var found = tasks.FindSync<TestTaskModel>(t => t.name == "Sample").ToJson();

            return View(found);
        }

        // GET: TestTaskController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestTaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestTaskController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestTaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestTaskController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestTaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
