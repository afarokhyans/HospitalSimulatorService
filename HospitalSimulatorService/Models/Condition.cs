
namespace HospitalSimulatorService.Models
{
    /// <summary>
    /// A condition belongs to Patient
    /// A cancer condition has a Topology
    /// </summary>
    public class Condition
    {
        public string Type { get; set; }
        public string Topology { get; set; }
    }
}
