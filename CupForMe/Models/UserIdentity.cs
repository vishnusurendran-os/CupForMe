using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models
{
    public class UserIdentity : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public string Otp { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
