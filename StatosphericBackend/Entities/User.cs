using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace StatosphericBackend.Entities
{
    public class User: IdentityUser
    {
        public string Role { get; set; }
    }
}