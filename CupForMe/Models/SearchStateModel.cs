using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models
{
    public class SearchStateModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
        public string SortField { get; set; }
        public SearchStateFilterModel Filter { get; set; }
    }
}
