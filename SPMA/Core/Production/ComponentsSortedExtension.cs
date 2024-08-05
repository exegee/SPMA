using SPMA.Dtos.Production;
using System.Collections.Generic;
using System.Linq;

namespace SPMA.Core.Production
{
    /// <summary>
    /// Extension class for duplicates counting
    /// </summary>
    public static class ComponentsSortedExtension
    {
        private static List<Duplicate> duplicates = new List<Duplicate>();
        public static List<Duplicate> GetDuplicates(this List<ComponentDto> components)
        {
            duplicates = components.GroupBy(x => x.Number).Select(g => new Duplicate() { Number = g.Key, Quantity = g.Sum(s => s.Quantity) }).ToList();
            return duplicates;
        }
    }

    /// <summary>
    /// Duplicate type model
    /// </summary>
    public class Duplicate
    {
        public string Number { get; set; }
        public int Quantity { get; set; }
    }

}
