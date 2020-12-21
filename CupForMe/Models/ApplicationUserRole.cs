using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public virtual UserIdentity User { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}
