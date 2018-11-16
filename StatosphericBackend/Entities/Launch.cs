using System;

namespace StatosphericBackend.Entities
{
    public class Launch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Time { get; set; }
    }
}