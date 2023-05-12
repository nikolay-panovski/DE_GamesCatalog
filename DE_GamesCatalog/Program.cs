using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using DE_GamesCatalog.Models;

namespace DE_GamesCatalog
{
    public class Program
    {
        public static MongoClient client;
        public const string mainDatabaseName = "DE_GamesCatalog";       // Match with the MongoDB database name.
        public const string mainDatabaseCollectionName = "gameitems";   // Match with the MongoDB collection name for the main database.

        // Purely saving horizontal space with this shortcut. (And possibly database calls.)
        public static IMongoDatabase mainDatabase;
        //public static IMongoCollection<GameItemModel> mainDatabaseCollection;

        public static void Main(string[] args)
        {
            ConnectToMongoDB();

            mainDatabase = client.GetDatabase(mainDatabaseName);


            CreateHostBuilder(args).Build().Run();
        }

        public static void ConnectToMongoDB()
        {
            // Setup instructions provided by MongoDB:
            string connectionUri = System.Environment.GetEnvironmentVariable("MONGODB_URI");
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            // "Set the ServerApi field of the settings object to Stable API version 1"
            // (and weirdly enough, that is the only available version)
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // "Create a new client and connect to the server"
            client = new MongoClient(settings);

            // "Send a ping to confirm a successful connection"
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
