using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apple.Models;
using Apple.Repository;

namespace Apple.Controllers
{
    [RoutePrefix("api/person")]
    public class PersonController : ApiController
    {
        PersonRepository personRepository = new PersonRepository();

        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetPersons()
        {
            return Json(personRepository.GetAll());
        }

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(int id)
        {
            return Json(personRepository.Get(id));
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(Person person)
        {
            return Json(personRepository.Add(person));
        }

        [HttpPost]
        [Route("Update")]
        public IHttpActionResult Put(Person person)
        {
            return Json(true);
        }

        [HttpDelete]
        [Route("Delete")]
        public IHttpActionResult Delete(int id)
        {
            return Json(personRepository.Delete(id));
        }
    }
}
