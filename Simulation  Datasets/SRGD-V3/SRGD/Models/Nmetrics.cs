using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Nmetrics
    {
        [Key]
        public int ExperimentID { get; set; }
        public string N50C { get; set; }
        public string N90C { get; set; }
        public string N50S { get; set; }
        public string N90S { get; set; }
    }
}
