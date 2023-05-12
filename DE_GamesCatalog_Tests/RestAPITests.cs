using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using DE_GamesCatalog.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Json;
using DE_GamesCatalog;
using System.Collections.Generic;

namespace DE_GamesCatalog_Tests
{
    public class RestAPITests
    {
        //Uri hostAddress = new Uri("https://localhost:44341");
        Uri hostAddress = new Uri("https://de-gamescatalog.onrender.com");

        [Fact]
        public async Task ValidCreateRequestReturns201WithValidItem()
        {
            // TODO: Pass proper URLs because URLs are now required against validation

            // Send a complete and valid object, without empty strings
            GameItemModel newItem = new GameItemModel();
            newItem.name = "NewGameFromTesting";
            newItem.imageURL = "https://placekitten.com/200/138";
            newItem.genre = "test";
            newItem.shortDescription = "This 'game' is created from XUnit integration tests.";
            newItem.gameplayDensity = 0;
            newItem.RNGDensity = 0;
            newItem.glitchesAmount = 0;
            newItem.valueForMoney = 1;
            newItem.twitchDirectoryURL = "https://random.org";


            HttpClient testClient = new HttpClient();
            testClient.BaseAddress = hostAddress;
            HttpRequestMessage requestPostWithBody = new HttpRequestMessage(HttpMethod.Post, testClient.BaseAddress + "games/new");

            requestPostWithBody.Content = JsonContent.Create(newItem, typeof(GameItemModel), new MediaTypeHeaderValue("application/json"));


            HttpResponseMessage responseMessage = testClient.Send(requestPostWithBody);

            Assert.Null(requestPostWithBody.RequestUri);
            Assert.Equal(System.Net.HttpStatusCode.Created, responseMessage.StatusCode);

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            //Assert.Contain(newItem.[some property here...]);  // and we could do this 17 or so times, but perhaps it is not crucial
        }

        [Fact]
        public async Task IncompleteObjectCreateRequestReturns400()
        {
            // Try to invalidate the object by not completely satisfying the required fields (do they still get valid default values??)
            GameItemModel newItem = new GameItemModel();
            newItem.name = "NewGameIncomplete";
            newItem.imageURL = "";
            newItem.genre = "test";
            newItem.shortDescription = "This INCOMPLETE 'game' is created from XUnit integration tests.";


            HttpClient testClient = new HttpClient();
            testClient.BaseAddress = hostAddress;
            HttpRequestMessage requestPostWithBody = new HttpRequestMessage(HttpMethod.Post, testClient.BaseAddress + "games/new");

            requestPostWithBody.Content = JsonContent.Create(newItem, typeof(GameItemModel), new MediaTypeHeaderValue("application/json"));

            HttpResponseMessage responseMessage = testClient.Send(requestPostWithBody);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, responseMessage.StatusCode);

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            Assert.Contains("Invalid object", responseBody);
        }

        [Fact]
        public async Task InvalidCreateRequestReturns400()
        {
            // Try to invalidate the object with out-of-range int values (can't invalidate via type mismatch...)
            GameItemModel newItem = new GameItemModel();
            newItem.name = "NewGameBadRatings";
            newItem.imageURL = "";
            newItem.genre = "test";
            newItem.shortDescription = "This INVALID VALUES 'game' is created from XUnit integration tests.";
            newItem.gameplayDensity = -3;
            newItem.RNGDensity = -9;
            newItem.glitchesAmount = 7;
            newItem.valueForMoney = 6;
            newItem.twitchDirectoryURL = "";


            HttpClient testClient = new HttpClient();
            testClient.BaseAddress = hostAddress;
            HttpRequestMessage requestPostWithBody = new HttpRequestMessage(HttpMethod.Post, testClient.BaseAddress + "games/new");

            requestPostWithBody.Content = JsonContent.Create(newItem, typeof(GameItemModel), new MediaTypeHeaderValue("application/json"));

            


            HttpResponseMessage responseMessage = testClient.Send(requestPostWithBody);


            Assert.Equal(System.Net.HttpStatusCode.BadRequest, responseMessage.StatusCode);
            
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            Assert.Contains("Invalid object", responseBody);
        }

        //[Fact]
        // Expected: Wrong object model fails validation constraints of our desired object model and returns "bad request".
        // Actual: Succeeds and creates (returns "created") a document of our desired object model full of nulls.
        public async Task WrongModelTypeCreateRequestReturns400()
        {
            // Pass a completely unrelated object *model* to our database, here: of GameItems
            WeatherForecast newItem = new WeatherForecast();
            newItem.Date = DateTime.Now;
            newItem.TemperatureC = 10;
            newItem.Summary = "What the hell is this weather doing in games";


            HttpClient testClient = new HttpClient();
            testClient.BaseAddress = hostAddress;
            HttpRequestMessage requestPostWithBody = new HttpRequestMessage(HttpMethod.Post, testClient.BaseAddress + "games/new");

            requestPostWithBody.Content = JsonContent.Create(newItem, typeof(WeatherForecast), new MediaTypeHeaderValue("application/json"));


            HttpResponseMessage responseMessage = testClient.Send(requestPostWithBody);
            
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, responseMessage.StatusCode);

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            Assert.Contains("Invalid object", responseBody);
        }

        [Fact]
        public void SwaggerAutogenPageExists()
        {
            //https://localhost:44341/swagger/index.html - make sure it keeps being built and served
            //(or respective deployed non-localhost address - in which case also make sure that the app (in Startup) is set to UseSwagger)
            HttpClient testClient = new HttpClient();
            testClient.BaseAddress = hostAddress;
            HttpRequestMessage requestGetSwagger = new HttpRequestMessage(HttpMethod.Get, testClient.BaseAddress + "swagger/index.html");

            HttpResponseMessage responseMessage = testClient.Send(requestGetSwagger);

            Assert.Equal(System.Net.HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
