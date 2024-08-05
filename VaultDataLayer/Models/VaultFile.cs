using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VaultDataLayer.Models
{
    public class VaultFile
    {
        public long MasterId { get; set; }
        public string FileName { get; set; }
        public double Quantity { get; set; }
        public VaultFile ParentFile { get; set; }
        public int Level { get; set; }
    }
}