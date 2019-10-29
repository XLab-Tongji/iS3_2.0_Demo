using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iS3.Core;
namespace iS3.Geology.Model
{
    [Table("Geology_SoilProperty")]
    public class SoilProperty : iS3AreaHandle
    {
        
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public Nullable<int> StratumSectionID { get; set; }
        public Nullable<int> StratumID { get; set; }
        public Nullable<double> w { get; set; }
        public Nullable<double> gama { get; set; }
        public Nullable<double> c { get; set; }
        public Nullable<double> fai { get; set; }
        public Nullable<double> cuu { get; set; }
        public Nullable<double> faiuu { get; set; }
        public Nullable<double> Cs { get; set; }         // swelling index
        public Nullable<double> qu { get; set; }
        public Nullable<double> K0 { get; set; }
        public Nullable<double> Kv { get; set; }
        public Nullable<double> Kh { get; set; }
        public Nullable<double> e { get; set; }
        public Nullable<double> av { get; set; }
        public Nullable<double> Cu { get; set; }         // coefficient of uniformity
        public Nullable<double> G { get; set; }
        public Nullable<double> Sr { get; set; }
        public Nullable<double> ccq { get; set; }
        public Nullable<double> faicq { get; set; }
        public Nullable<double> c_s { get; set; }
        public Nullable<double> fais { get; set; }
        public Nullable<double> a01_02 { get; set; }
        public Nullable<double> Es01_02 { get; set; }
        public Nullable<double> ccu { get; set; }
        public Nullable<double> faicu { get; set; }
        public Nullable<double> cprime { get; set; }
        public Nullable<double> faiprime { get; set; }
        public Nullable<double> E015_0025 { get; set; }
        public Nullable<double> E02_0025 { get; set; }
        public Nullable<double> E04_0025 { get; set; }
    
    }
}
