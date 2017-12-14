using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using HospitalSimulatorService.Models;
using System.Net.Http;

namespace HospitalSimulatorService.Controllers
{
    public class ConsultationsController : ApiController
    {
        public ConsultationsController()
        {               
        }

        public ConsultationsController(List<Consultation> consultations)
        {
            DataRepository.Consultations = consultations;
        }

        // GET: api/Consultations
        public HttpResponseMessage GetAllScheduledConsultations()
        {
            return DataRepository.Consultations != null ? Request.CreateResponse(HttpStatusCode.OK, DataRepository.Consultations) : 
                                                          Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
