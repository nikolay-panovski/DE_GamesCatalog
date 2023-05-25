using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using DE_GamesCatalog.Models;

namespace DE_GamesCatalog.Controllers
{
    public class GameItemController : Controller
    {
        /// -- [Route]: The "default" convention is that the method name will serve as the route name(?).
        /// In any case, overwrite that by putting this attribute and being explicit about the route name. It helps Swagger autogen too.
        [Route("games")]
        [HttpGet]
        public ActionResult AllGamesPage()
        {
            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");

            // return all documents, AKA make everything pass the "filter":
            List<GameItemModel> allDBGames = dbCollection.FindSync<GameItemModel>(/*_ => true /*OR*/ new BsonDocument()).ToList();

            /// -- View(): If the "default" convention is NOT followed, AKA the method name here does not match
            /// a view page name in the corresponding folder (here: GameItem) OR Shared folder, the view page name MUST be passed as a string.
            /// If a model is required for that view page to render information from, said model MUST be passed as the second argument here (object).
            return View("Index", allDBGames);
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

            return View(requestedGame); // Json() is also possible, for an API that would allow a frontend part to handle the data on its own.
        }

        [Route("games/new")]
        [HttpGet]               // GET a create page, the actual creation should be handled by that page + the POST route below.
        public ActionResult Create()
        {
            return View("CreateItem");
        }

        // HTTP 405 Unsupported Media Type: Form input from the pages is bad. (Swagger JSON object within the validation constraints succeeds 201.)
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
                string errors = string.Empty;
                foreach (ValidationResult result in validationResults)
                {
                    errors = string.Concat(errors, result, " ");
                }
                return BadRequest("Invalid object: " + newItem + "(errors: " + errors + ")");    //StatusCode(400);
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

        [HttpGet]                       // GET an edit page for this item, the actual edit (assumed POST) is handled by the forms generated there.
        [Route("games/edit/{name}")]
        public ActionResult Edit(string name)
        {
            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");

            // It is more sensible to use ID identification instead of name, because the former is guaranteed to be unique.
            // Here this is assumed for the latter as well, as a design detail.
            // (This also conveniently sidesteps dealing with ObjectId conversions.)
            GameItemModel requestedGame = dbCollection.FindSync<GameItemModel>(g => g.name == name).Single();

            return View("EditItem", requestedGame);
        }

        // HTTP 405 Method Not Allowed: ASP.NET internals *somewhere* prevent requests other than GET and PUT from passing to server.
        // https://stackoverflow.com/questions/12276316/mvc-4-iis-7-5-put-returning-405 (and many others)
        [HttpPut]
        [Route("games/edit/{name}")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([FromBody] GameItemModel editedItem)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(editedItem, new ValidationContext(editedItem), validationResults, true);

            if (!isValid)
            {
                string errors = string.Empty;
                foreach (ValidationResult result in validationResults)
                {
                    errors = string.Concat(errors, result, " ");
                }
                return BadRequest("Invalid object: " + editedItem + "(errors: " + errors + ")");    //StatusCode(400);
            }

            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");
            try
            {
                dbCollection.UpdateOne(Builders<GameItemModel>.Filter.Eq("_id", editedItem._id),
                                                   Builders<GameItemModel>.Update.Set(nameof(GameItemModel.name), editedItem.name)
                                                                                 .Set(nameof(GameItemModel.imageURL), editedItem.imageURL)
                                                                                 .Set(nameof(GameItemModel.genre), editedItem.genre)
                                                                                 .Set(nameof(GameItemModel.shortDescription), editedItem.shortDescription)
                                                                                 .Set(nameof(GameItemModel.twitchDirectoryURL), editedItem.twitchDirectoryURL));

                return RedirectToAction(nameof(Details), new { editedItem.name });
            }
            catch
            {
                return StatusCode(500, "Could not edit the object. Let the person working on this figure it out...");
            } 
        }

        // HTTP 405 Method Not Allowed: ASP.NET internals *somewhere* prevent requests other than GET and PUT from passing to server.
        [HttpDelete]
        [Route("games/delete/{id}")]
        public ActionResult Delete(string id)
        {
            IMongoCollection<GameItemModel> dbCollection = Program.mainDatabase.GetCollection<GameItemModel>("gameitems");

            // ... I forgot that ObjectId is comparable to string, not int. Anyway, the below does not feel right.
            try
            {
                dbCollection.FindOneAndDelete<GameItemModel>(g => g._id.ToString() == id);
            }
            catch
            {
                // ...And of course, blame the user. (Better is to find the document and then delete it, separating the possible error moments.)
                return BadRequest("Could not find and delete object with ID: " + id + ". Did you use a valid ID?");
            }

            return RedirectToAction(nameof(AllGamesPage));
        }
    }
}
