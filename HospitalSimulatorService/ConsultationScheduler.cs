using System;
using System.Collections.Generic;
using System.Linq;
using HospitalSimulatorService.Models;

namespace HospitalSimulatorService
{
    /// <summary>
    /// This class handles the scheduling of a consultation for a patient
    /// </summary>
    public class ConsultationScheduler
    {
        /// <summary>
        /// This is the main function where we try to find the next first available date to schedule a new consultation 
        /// For Flu patients, Check if there is a GeneralPractitioner and any type of treatment room is available. If both are available, schedule a consulation
        /// Follow the same logic for patients with Cancer, except there will be extra check done when scheduling the room where we need to check the patient cancer topologyu
        /// A Consultation takes a full
        /// A Consultation may not be scheduled on the same day as the patient is registered
        /// A Consultation may be scheduled on any calendar day
        /// Resources may not be double-booked
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public Consultation ScheduleConsultation(Patient patient)
        {
            // Compare total counts of Treatment Rooms and Doctors, Use the small value for calculation (Consultation requires both a doctor and a room)
            var maxConsultationsPerDay = MaxConsultationsPerDay();
            var currentDate = DateTime.Now.Date;
            //The earliest a consultation can be scheduled is one day after the registration
            var startDay = currentDate.AddDays(1).Day;

            while (true)
            {
                foreach (var date in AllDatesInMonth(currentDate.Year, currentDate.Month, startDay))
                {
                    var dailyConsultations = DataRepository.Consultations.Where(d => d.ConsultationDate.Equals(date)).ToList();
                    // Check if there are still available consultations based on the resrouces for each day
                    if (dailyConsultations.Count >= maxConsultationsPerDay)
                        continue;

                    if (patient.Condition.Type.Equals("Flu", StringComparison.OrdinalIgnoreCase))
                    {
                        IList<Doctor> availableGeneralPractitioner;
                        IList<TreatmentRoom> availableTreatmentRoom;

                        // Find the first available GeneralPractitioner - Flu patients have to see a GeneralPractitioner
                        // Find the first available room - Based on requirements Flu patient can be seen in any treatment room
                        // If no consultation exists, book a room without a treatment machine
                        if (dailyConsultations.Any())
                        {
                            availableGeneralPractitioner = DataRepository.Doctors.Except(dailyConsultations.Select(d => d.Doctor)).Where(dr => dr.Roles.Contains("GeneralPractitioner")).ToList();
                            // First check if there are any rooms without treatment machine since Flu doesn't require treatment machine
                            availableTreatmentRoom = DataRepository.TreatmentRooms.Except(dailyConsultations.Select(r => r.TreatmentRoom)).Where(x => x.TreatmentMachine.Name.Equals("")).ToList();  
                            if(!availableTreatmentRoom.Any())
                                availableTreatmentRoom = DataRepository.TreatmentRooms.Except(dailyConsultations.Select(r => r.TreatmentRoom)).ToList();
                        }
                        else
                        {
                            availableGeneralPractitioner = DataRepository.Doctors.Where(dr => dr.Roles.Contains("GeneralPractitioner")).ToList();
                            availableTreatmentRoom = DataRepository.TreatmentRooms.Where(r => r.TreatmentMachine.Name.Equals("")).ToList();//Only rooms without Treatment Machines
                        }

                        if (availableGeneralPractitioner.Any() && availableTreatmentRoom.Any())
                        {
                            return CreateConsultation(patient, availableGeneralPractitioner.First(), date, DateTime.Now.Date, availableTreatmentRoom.First());
                        }
                    }
                    else if (patient.Condition.Type.Equals("Cancer", StringComparison.OrdinalIgnoreCase))
                    {
                        var availableOncologist = DataRepository.Doctors.Except(dailyConsultations.Select(d => d.Doctor)).Where(dr => dr.Roles.Contains("Oncologist")).ToList();

                        if (availableOncologist.Any() && patient.Condition.Topology.Equals("Head&Neck", StringComparison.OrdinalIgnoreCase))
                        {
                            var advancedTreatmentRoom = GetAvailableOncologyRooms("Advanced", dailyConsultations);

                            if (advancedTreatmentRoom.Any())
                            {
                                return CreateConsultation(patient, availableOncologist.First(), date, DateTime.Now.Date, advancedTreatmentRoom.First());
                            }
                        }
                        if (!availableOncologist.Any() || !patient.Condition.Topology.Equals("Breast", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var simpleTreatmentRoom = GetAvailableOncologyRooms("Simple", dailyConsultations);

                        if (simpleTreatmentRoom.Any())
                        {
                            return CreateConsultation(patient, availableOncologist.First(), date, DateTime.Now.Date, simpleTreatmentRoom.First());
                        }
                    }
                }

                currentDate = currentDate.AddDays(1);
                startDay = 1;
            }
        }

        private static IList<TreatmentRoom> GetAvailableOncologyRooms(string treatmentMachineType, List<Consultation> dailyConsultations)
        {
            var oncologyMachinesByType = new HashSet<string>(DataRepository.TreatmentMachines.Where(c => c.Capability.Equals(treatmentMachineType)).Select(a => a.Name));

            IList<TreatmentRoom> availableRooms = dailyConsultations.Any() ? 
                DataRepository.TreatmentRooms.Except(dailyConsultations.Select(r => r.TreatmentRoom)).Where(x => oncologyMachinesByType.Contains(x.TreatmentMachine.Name)).ToList() : 
                DataRepository.TreatmentRooms.Where(x => oncologyMachinesByType.Contains(x.TreatmentMachine.Name)).ToList();

            return availableRooms;
        }


        /// <summary>
        /// Create a consulation object
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="doctor"></param>
        /// <param name="consulationDate"></param>
        /// <param name="registrationDate"></param>
        /// <param name="treatmentRoom"></param>
        /// <returns></returns>
        private static Consultation CreateConsultation(Patient patient, Doctor doctor, DateTime consulationDate, DateTime registrationDate, TreatmentRoom treatmentRoom)
        {
            var consultation = new Consultation
            {
                RegistrationDate = registrationDate,
                ConsultationDate = consulationDate,
                Patient = patient,
                Doctor = doctor,
                TreatmentRoom = treatmentRoom,
            };
            return consultation;
        }

        /// <summary>
        /// Since a consultation requires a Doctor and a Treatment room, we can compare then count of Doctors and Treatment rooms
        /// which ever count is lower, it can be used as the maximum number of consultations that can be scheduled for that day
        /// For each future day, first get the count of existing scheduled consultation, if the count is less than max, then continue
        /// with the patient's condition.
        /// </summary>
        /// <returns></returns>
        private static int MaxConsultationsPerDay()
        {
            var maxPossibleConsultationPerDay = DataRepository.Doctors.Count <= DataRepository.TreatmentRooms.Count ? DataRepository.Doctors.Count : DataRepository.TreatmentRooms.Count;

            return maxPossibleConsultationPerDay;
        }

        /// <summary>
        /// Get an IEnumbereable of all the dates in a month based on the input for Year, Month and Day
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> AllDatesInMonth(int year, int month, int day)
        {
            var days = DateTime.DaysInMonth(year, month);
            for (var remainingDays = day; remainingDays <= days; remainingDays++)
            {
                yield return new DateTime(year, month, remainingDays);
            }
        }
    }
}
