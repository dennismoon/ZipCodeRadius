using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZipCodeRadius.Models
{
    public class ZipCodeInfo
    {
        [Key]
        public Guid Id { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string StateName { get; set; }

        public string StateAbbreviation { get; set; }

        public string CountyName { get; set; }

        public string CountyFipsCode { get; set; }

        public double Longitude { get; set; }

        public double Lattitude { get; set; }
    }
}
