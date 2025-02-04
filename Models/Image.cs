using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace let_em_cook.Models;

public class Image
{
    public int ImageId { get; set; }

    public string ContentType { get; set; }

    [NotMapped]
    public Stream? DataStream { get; set; }
    
    [Required]
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}