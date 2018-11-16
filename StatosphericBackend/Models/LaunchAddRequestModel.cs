using System;

namespace StatosphericBackend.Entities
{
    public class LaunchAddRequestModel
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}