using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using DE_GamesCatalog.Models;

namespace DE_GamesCatalog.Controllers
{
    public class GameItemController : Controller
    {
        [Route("games")]
        [HttpGet]
        public ActionResult AllGamesPage()
        {
            // todo: page populated with database entries

            return View();
        }

        [Route("games/{name}")]  // do not constrain a route param by type unless the constraint itself is set up, sort of like here:
                                 // https://stackoverflow.com/questions/71859662/the-constraint-reference-string-could-not-be-resolved-to-a-type-type-with-mi
        [HttpGet]
        public ActionResult Details(string name)
        {
            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");

            // Single(): If exactly one game is found with this "name", returns it. Else gets an exception.
            // Forces the application/the database to implement a way to distinguish between different games with accidentally same names.
            GameItemModel requestedGame = dbCollection.FindSync<GameItemModel>(g => g.name == name).Single();

            return View(requestedGame); // Json() for testing purposes
            // TODO NEXT: Error: "The View 'Details' was not found."
        }

        [Route("games/new")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        // https://learn.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api
        // [FromBody] allows to pass a type from the request body instead of from the URI.
        // The above link makes it seem like by default simple types get passed in the URI and complex types (class objects) in the request body,
        // but that does not appear to be true?
        public ActionResult Create([FromBody] GameItemModel newItem)
        {
            // TODO: Separate into method
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(newItem, new ValidationContext(newItem), validationResults, true);

            if (!isValid)
            {
                return BadRequest("Invalid object: " + newItem + "(errors: " + validationResults + ")");    //StatusCode(400);
            }

            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");
            dbCollection.InsertOne(newItem);

            try
            {
                Uri newAddressRelative = new Uri($"/games/{newItem.name}/", UriKind.Relative);
                return Created(newAddressRelative, newItem);
                //return RedirectToAction(nameof(Index));
            }
            catch
            {
                // can do a few status codes via their names, but not all
                // https://learn.microsoft.com/en-us/dotnet/api/system.web.http.apicontroller?view=aspnetcore-2.2 (look at methods)
                return StatusCode(500);
            }
        }

        /// -- do not try to InsertMany; design-wise it doesn't make sense, for response purposes (Created()) it's a hassle too

        // GET: GameItemController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GameItemController/Edit/5
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

        // GET: GameItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GameItemController/Delete/5
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
