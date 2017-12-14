using System.Collections.Generic;

namespace HospitalSimulatorService.Models
{
    public class Doctor
    {
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }
}
