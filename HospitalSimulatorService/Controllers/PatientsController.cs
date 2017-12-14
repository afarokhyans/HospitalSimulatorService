using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using HospitalSimulatorService.Models;
using System.Net.Http;

namespace HospitalSimulatorService.Controllers
{
    public class PatientsController : ApiController
    {
        public PatientsController()
        {
        }

        /// <summary>
        /// Used for Unit Test
        /// </summary>
        /// <param name="patients"></param>
        public PatientsController(List<Patient> patients)
        {
            DataRepository.Patients = patients;
        }

        // GET: api/Patients
        public HttpResponseMessage GetAllRegisteredPatients()
        {
            var response = DataRepository.Patients != null ? Request.CreateResponse(HttpStatusCode.OK, DataRepository.Patients) : 
                                                             Request.CreateResponse(HttpStatusCode.NoContent);

            return response;
        }

        // POST: api/Patients
        public HttpResponseMessage RegisterPatient([FromBody]Patient value)
        {
            var isValidRequest = DataRepository.ValidatePatientRegistrationRecord(value);

            return Request.CreateResponse(!isValidRequest ? HttpStatusCode.BadRequest : DataRepository.RegisterPatient(value));
        }
    }
}
