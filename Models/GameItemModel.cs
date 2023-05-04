using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace DE_GamesCatalog.Models
{
    public class GameItemModel
    {
        public ObjectId _id { get; set; }

        public string name { get; set; }
        [Url]
        public string imageURL { get; set; }
        public string genre { get; set; }   // enum possible?
        public string shortDescription { get; set; }

        /// -- Ratings: a Rating is an int between 0-5 that results in a customized display (such as stars).
        /// (ideally should be a float for accurate average values)
        /// int? = Nullable<int>. Default to null. This should correspond to a "no rating yet" representation.
        [Range(0, 5)]
        public int? gameplayDensity { get; set; } = null;
        [Range(0, 5)]
        public int? RNGDensity { get; set; } = null;
        [Range(0, 5)]
        public int? glitchesAmount { get; set; } = null;
        [Range(0, 5)]
        public int? valueForMoney { get; set; } = null;


        /// <summary>
        /// The URL passed to the composite visual object of a game item in the app,
        /// for users to go to and find content for their desired game.
        /// </summary>
        // Intended to be passed to the final object (on a link or button).
        // In theory we could be less discriminatory and make the field
        // without a site affiliation(just pass any useful redirection URL), array, or at least add another one for YouTube.
        [Url]
        public string twitchDirectoryURL { get; set; }
    }
}
