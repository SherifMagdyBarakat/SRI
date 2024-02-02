using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class Experiments
    {
        [Key]
        [Required]
        public int ExperimentID { get; set; }
        [Required]
        public DateTime ExperimentDate { get; set; }
        [Required]
        public DateTime ExperimentTime { get; set; }
        [Required]
        public string ExperimentFolderPath { get; set; }
        [Required(ErrorMessage = "Species is Required")]
        public string Species { get; set; }
        [Required]
        public string Username { get; set; }
        [NotMapped]
        public IFormFile[] Reads{ get; set; }
        [NotMapped]
        public IFormFile Reference { get; set; }
        [NotMapped]
        public List<string>  FASTAFiles { get; set; }
        [NotMapped]
        public string ReferenceFile { get; set; }

        [NotMapped]
        public ReadsType? ReadsType { get; set; }
     
        
        [NotMapped]
        public int PN { get; set; }

        [NotMapped]
        public int Similarity { get; set; }

    }
}
