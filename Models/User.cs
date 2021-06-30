using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FilmsCatalog.Models
{
    public class User : IdentityUser
    {
        public static ClaimsIdentity Identity { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }
}