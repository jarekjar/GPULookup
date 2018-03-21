using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GpuLookup.Models
{
    public class GPU
    {
        public int? Id { get; set; }
        public decimal Price { get; set; }
        public string Card { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int RowCount { get; set; }
    }
}