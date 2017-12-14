using System.Collections.Generic;
using System.Net;
using HospitalSimulatorService;
using HospitalSimulatorService.Controllers;
using HospitalSimulatorService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HospitalSimulatorServiceTest
{
    [TestClass]
    public class ControllersTest
    {
        [TestMethod]
        public void GetAllRegisteredPatients_ShouldReturnAllPatients()
        {
            var testPatients = GetTestPatients();
            var controller = new PatientsController(testPatients);
            foreach (var patient in testPatients)
            {
                controller.RegisterPatient(patient);
            }

            var resultResponse = controller.GetAllRegisteredPatients();
            Assert.AreEqual(resultResponse.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public void GetConsultationsForRegisteredPatients_ShouldReturnAllConsultation()
        {
            var testPatients = GetTestPatients();
            var patientsController = new PatientsController(testPatients);
            foreach (var patient in testPatients)
            {
                 patientsController.RegisterPatient(patient);
            }

            var consulationController = new ConsultationsController();

            var resultResponse = consulationController.GetAllScheduledConsultations();
            Assert.AreEqual(resultResponse.StatusCode, HttpStatusCode.Created);
        }

        private static List<Patient> GetTestPatients()
        {
            //Seed resources
            DataRepository.ReadSeedJson();

            var testPatients = new List<Patient>
            {
                new Patient {Name = "Test1", Condition = new Condition {Type = "Flu"}},
                new Patient {Name = "Test2", Condition = new Condition {Type = "Cancer", Topology = "Head&Neck"}},
                new Patient {Name = "Test3", Condition = new Condition {Type = "Cancer", Topology = "Breast"}},
                new Patient {Name = "Test4", Condition = new Condition {Type = "Flu"}}
            };

            return testPatients;
        }
    }
}
