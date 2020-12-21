using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Utils
{
    public class ApplicationSettings
    {
        public int LoginClaimExpirationMinutes { get; set; }
        public string JwtSecret { get; set; }
    }
}
