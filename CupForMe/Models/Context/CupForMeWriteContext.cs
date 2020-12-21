using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models.Context
{
    public class CupForMeWriteContext : DbContext
    {
        public CupForMeWriteContext(DbContextOptions<CupForMeWriteContext> options) : base(options)
        {

        }
    }
}
