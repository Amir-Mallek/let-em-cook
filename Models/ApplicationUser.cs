using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using let_em_cook.Models;
using Newtonsoft.Json;

namespace let_em_cook.Models;

public class ApplicationUser : IdentityUser
{
    public int Age { get; set; }
    
    public string Country { get; set; }
    
    public ICollection<ApplicationUser>? Subscribers { get; set; }
    
    public ICollection<ApplicationUser>? Subscriptions { get; set; }
    
    public ICollection<Vote> Votes { get; set; }
    
    public ICollection<Review> Reviews { get; set; }

    [JsonIgnore]
    public ICollection<Recipe> Recipes { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}