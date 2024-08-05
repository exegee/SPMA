using SPMA.Dtos.Production;
using System.Collections.Generic;

namespace SPMA.Core.Production
{
    public class ComponentsSorted
    {
        /// <summary>
        /// Helper class to organize components
        /// </summary>
        public List<ComponentDto> standardComponents { get; set; } = new List<ComponentDto>();
        public List<ComponentDto> plazmaCutComponents { get; set; } = new List<ComponentDto>();
        public List<ComponentDto> storeBoughtComponents { get; set; } = new List<ComponentDto>();
    }
}
