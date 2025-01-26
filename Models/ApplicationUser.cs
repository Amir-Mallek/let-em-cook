using Microsoft.AspNetCore.Identity;
using let_em_cook.Models;
namespace let_em_cook.Models;

public class ApplicationUser : IdentityUser
{
    public int Age { get; set; }
    
    public ICollection<Vote> Votes { get; set; }
    
    public ICollection<Review> Reviews { get; set; }

    public ICollection<Recipe> Recipes { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}