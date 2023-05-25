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

        [Display(Name = "Name")]
        [Required]
        public string name { get; set; }

        [Url]
        [Display(Name = "Image URL")]
        [Required]
        public string imageURL { get; set; }

        [Display(Name = "Genre")]
        [Required]
        public string genre { get; set; }   // enum possible?

        [Display(Name = "Short description")]
        [Required]
        public string shortDescription { get; set; }

        /// -- Ratings: a Rating is an int between 0-5 that results in a customized display (such as stars).
        /// (ideally should be a float for accurate average values)
        /// int? = Nullable<int>. Default to null. This should correspond to a "no rating yet" representation.
        [Range(0, 5)]
        [Display(Name = "Gameplay Density")]
        public float? gameplayDensity { get; set; } = null;

        [Range(0, 5)]
        [Display(Name = "RNG Density")]
        public float? RNGDensity { get; set; } = null;

        [Range(0, 5)]
        [Display(Name = "Amount of glitches")]
        public float? glitchesAmount { get; set; } = null;

        [Range(0, 5)]
        [Display(Name = "Value for money")]
        public float? valueForMoney { get; set; } = null;


        /// <summary>
        /// The URL passed to the composite visual object of a game item in the app,
        /// for users to go to and find content for their desired game.
        /// </summary>
        // Intended to be passed to the final object (on a link or button).
        // In theory we could be less discriminatory and make the field
        // without a site affiliation(just pass any useful redirection URL), array, or at least add another one for YouTube.
        [Url]
        [Display(Name = "Twitch Directory URL")]
        [Required]
        public string twitchDirectoryURL { get; set; }
    }
}
