using System.ComponentModel.DataAnnotations;

namespace SPMA.Models.Core
{
    public class Service
    {
        public int ServiceID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        // Is service running ?
        public bool IsRunning { get; set; }
        // Run interval in seconds
        public int RunInterval { get; set; }
        public string CurrentJob { get; set; }
    }
}
