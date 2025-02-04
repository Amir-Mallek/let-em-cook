using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

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
        
        public string? UserId { get; set; }

        [DataType(DataType.Time)]
        public DateTime? TimeOfPublishement { get; set; }
    }
}
