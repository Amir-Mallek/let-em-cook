using System;
using System.ComponentModel.DataAnnotations;

namespace let_em_cook.Models
{
    public class RecipeCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Steps { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string UserId { get; set; }  // Assuming the UserId will be passed as part of the request
    }
}
