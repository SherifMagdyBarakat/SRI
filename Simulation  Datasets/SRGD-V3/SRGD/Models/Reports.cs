using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    [NotMapped]
    public class Reports
    {
        public int ExperimentID { get; set; }

        public string N50C { get; set; }
        public string N90C { get; set; }
        public string N50S { get; set; }
        public string N90S { get; set; }
     
        public string Species { get; set; }

    
        public string ValidReadCount { get; set; }

        public string RejectedReadCount { get; set; }
      
        public string maxL { get; set; }

        public string minL { get; set; }
      
        public string TotalReadCount { get; set; }
  
        public string ReadCountAfterDuplication { get; set; }
        
    
        public string ReadCountAfterOverlapping { get; set; }


        public string TotalDatasetSize { get; set; }
 
        public string DatasetSizetAfterDuplication { get; set; }
       
        public string DatasetSizeAfterOverlapping { get; set; }

       
        public string PN { get; set; }
    
        public string PL { get; set; }
    
     
        public string ReferenceLength { get; set; }

        public string KmerLength { get; set; }
   
        public string KmerCount { get; set; }
 
        public string Coverage { get; set; }



        public string ContigCount { get; set; }

        public string ScaffoldCount { get; set; }
    
        public string AllowedMismatchCount { get; set; }

   
        public string MisAssemblyCount { get; set; }
        
        public string OverlappingTime { get; set; }
    
        public string TotalAssemblyTime { get; set; }
        
        public string TotalAssemblySize{ get; set; }

       public string RepeatIdentificationTime { get; set; }
        public string RepeatAnnotationTime { get; set; }
        public string AlignmentTime { get; set; }

        public string TotalNonRepeatCount { get; set; }
     
        public string TotalRepeatCount { get; set; }
      
        public string RepeatCountPartitions { get; set; }
       
        public string RepeatCountFR { get; set; }

        public string RepeatCountCR { get; set; }
   
        public DataTable RepeatAnnotation{ get; set; }
      
        public DataTable OverlappingMetrics { get; set; }

   
      
        public string ExperimentFolderPath { get; set; }

        public string CountRepeat { get; set; }
        public string CountUniqueRepeat { get; set; }
        public Int64 RepeatSize { get; set; }
    }
}
