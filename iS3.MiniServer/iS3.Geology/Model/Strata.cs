using iS3.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Geology.Model
{
    [Table("Geology_Strata")]
    public class Strata : iS3AreaHandle
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public string GeologicalAge { get; set; }
        public string FormationType { get; set; }
        public string Compaction { get; set; }
        public string ElevationOfStratumBottom { get; set; }
        public string Thickness { get; set; }
    }
}

