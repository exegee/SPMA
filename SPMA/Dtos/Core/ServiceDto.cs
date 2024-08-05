namespace SPMA.Dtos.Core
{
    public class ServiceDto
    {
        public int ServiceID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Is service running ?
        public bool IsRunning { get; set; }
        // Run interval in seconds
        public int RunInterval { get; set; }
        public string CurrentJob { get; set; }
    }
}
