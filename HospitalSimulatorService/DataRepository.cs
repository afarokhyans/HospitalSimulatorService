using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using HospitalSimulatorService.Models;
using System.IO;
using System.Net;

namespace HospitalSimulatorService
{
    public static class DataRepository
    {
        public static List<TreatmentMachine> TreatmentMachines { get; set; }
        public static List<TreatmentRoom> TreatmentRooms { get; set; }
        public static List<Consultation> Consultations { get; set; }
        public static List<Doctor> Doctors { get; set; }
        public static List<Patient> Patients { get; set; }

        /// <summary>
        /// Register a patient and schedule a consulation
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static HttpStatusCode RegisterPatient(Patient patient)
        {
            if (Patients == null)
                Patients = new List<Patient>();

            Patients.Add(patient);

            if (Consultations == null)
                Consultations = new List<Consultation>();

            var scheduler = new ConsultationScheduler();
            var consultation = scheduler.ScheduleConsultation(patient);
            if (consultation == null)
                return HttpStatusCode.BadRequest;

            Consultations.Add(consultation);

            return HttpStatusCode.Created;
        }

        /// <summary>
        /// Validate the data sent by user to register a patient
        /// Validation assumptions:
        /// Patient needs to have a Name
        /// Patient Condition can only be Cancer or Flu
        /// Patient with Cancer condition have to have a Topology of either "Head&Neck" or "Breast"
        /// patient with Flu, cannot have a Topology
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public static bool ValidatePatientRegistrationRecord(Patient patient)
        {
            if (string.IsNullOrEmpty(patient.Name))
                return false;

            if (patient.Condition.Type.Equals("Cancer", StringComparison.CurrentCultureIgnoreCase))
            {
                // Check for invalid topology
                if (!patient.Condition.Topology.Equals("Head&Neck") && !patient.Condition.Topology.Equals("Breast"))
                    return false;
            }
            else if (patient.Condition.Type.Equals("Flu", StringComparison.CurrentCultureIgnoreCase))
            {
                // Flu condition should not have a Topology value
                if(!string.IsNullOrEmpty(patient.Condition.Topology))
                    return false;
            }
            else
            {
                // Invalid Condition.Type
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the seed data from seed.json and populate the resources
        /// </summary>
        public static void ReadSeedJson()
        {
            var dir = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(dir, @"SeedData\seed.json");
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(dir, @"..\..\SeedData\seed.json");
            }

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            var seedData = JObject.Parse(File.ReadAllText(filePath));

            // Read Seed Data
            var doctors = seedData.GetValue("doctors").ToList();
            var treatmentMachines = seedData.GetValue("treatmentMachines").ToList();
            var treatmentRooms = seedData.GetValue("treatmentRooms").ToList();

            // Add doctors
            Doctors = new List<Doctor>();
            foreach (var doctor in doctors)
            {
                var name = ((JObject)doctor).GetValue("name").ToString();
                var roles = ((JObject)doctor).GetValue("roles").ToList().Select(r => r.ToString());
                var newDoctor = new Doctor
                {
                    Name = name,
                    Roles = new List<string>()
                };
                foreach (var role in roles)
                {
                    newDoctor.Roles.Add(role);
                }

                Doctors.Add(newDoctor);
            }

            // Add treatment machines
            TreatmentMachines = new List<TreatmentMachine>();
            foreach (var machine in treatmentMachines)
            {
                var name = ((JObject)machine).GetValue("name").ToString();
                var capability = ((JObject)machine).GetValue("capability").ToString();
                TreatmentMachines.Add(new TreatmentMachine
                {
                    Name = name,
                    Capability = capability
                });
            }

            // Add treatment rooms
            TreatmentRooms = new List<TreatmentRoom>();
            foreach (var room in treatmentRooms)
            {
                var name = ((JObject)room).GetValue("name").ToString();
                var machine = ((JObject)room).GetValue("treatmentMachine");
                var machineName = machine?.ToString() ?? "";
                TreatmentRooms.Add(new TreatmentRoom
                {
                    Name = name,
                    TreatmentMachine = new TreatmentMachine { Name = machineName }
                });
            }
        }
    }
}
