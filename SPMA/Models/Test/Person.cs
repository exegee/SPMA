using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Models.Test
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
