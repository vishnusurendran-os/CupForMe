using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models.Context
{
    public class CupForMeReadContext : DbContext
    {
        public CupForMeReadContext(DbContextOptions<CupForMeReadContext> options) : base(options)
        {

        }
    }
}
