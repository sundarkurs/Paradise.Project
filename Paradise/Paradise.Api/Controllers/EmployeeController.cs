
using System.Web.Http;
using Paradise.Api.Legacy.Service;
using Paradise.Api.Models;

namespace Paradise.Api.Controllers
{
    public class EmployeeController : ApiController
    {
        public Employee Get(int id)
        {
            ContactService service = new ContactService();
            var contact = service.GetContact(id);

            return new Employee()
            {
                Id = contact.ContactId,
                FirstName = contact.ContactFirstName,
                LastName = contact.ContactLastName,
                Address = contact.ContactAddress,
                Mobile = contact.ContactMobile,
                DateOfBirth = contact.ContactDateOfBirth
            };

        }
    }
}
