using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Models.Test
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }

        public Person Person { get; set; }
    }
}
