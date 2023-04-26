using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DE_GamesCatalog.Models
{
    public class TestTaskModel
    {
        public enum StateVisibility
        {
            [BsonRepresentation(BsonType.String)] Personal,
            [BsonRepresentation(BsonType.String)] Unpublished,
            [BsonRepresentation(BsonType.String)] Completed
        }

        public enum StateCompletion
        {
            [BsonRepresentation(BsonType.String)] Incomplete,
            [BsonRepresentation(BsonType.String)] Completed,
            [BsonRepresentation(BsonType.String)] Cancelled
        }

        // ---- OBJECT ID ----
        // https://mongodb.github.io/mongo-csharp-driver/2.14/reference/bson/mapping/
        // "By convention, a public member called Id, id, or _id will be used as the identifier. You can be specific about this using the BsonIdAttribute."
        // Use either of the two following lines.
        public ObjectId _id { get; set; }   
        //[BsonRepresentation(BsonType.ObjectId)] public string _id { get; set; }     // NB: cannot deserialize Int or String to ObjectId, unless the attribute is applied

        [Range(1, 255)]
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime finished_at { get; set; }
        public long time_estimated { get; set; } //{ type: Number /* where Number = minutes; formatted separately */, default: 0 },
        public long time_registered { get; set; } //{ type: Number /* see above */, default: 0 }

        // error: cannot resolve string to enum type (logically)
        public StateVisibility state_visibility { get; set; } = StateVisibility.Personal;
        public StateCompletion state_completion { get; set; } = StateCompletion.Incomplete;

        /**/
        //project: { type: mongoose.Schema.Types.ObjectId, ref: "projects" },   // careful - leave refs for later
        //assignee: { type: mongoose.Schema.Types.ObjectId, ref: "users" },
        //deadline: { type: Date | ObjectId ref with ref: project.deadline },
        //assigned_at: { type: Date, /* see finished_at*/ }
        /**/

        /**
        //[BsonConstructor]
        public TestTaskModel(string pName, string pDescription)
        {
            name = pName;
            description = pDescription;
        }
        /**/
    }
}
