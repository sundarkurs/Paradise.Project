
using System;
using System.Web.Http;
using AutoMapper;
using Paradise.Api.Legacy.Entities;
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


            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<ContactEntity, Employee>();
            //});

            //IMapper mapper = config.CreateMapper();
            //var source = new ContactEntity();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ContactEntity, Employee>()
                .ForMember(vm => vm.Id, map => map.MapFrom(s => s.ContactId))
               .ForMember(vm => vm.FirstName, map => map.MapFrom(s => s.ContactFirstName))
               .ForMember(vm => vm.LastName, map => map.MapFrom(s => s.ContactLastName))
               .ForMember(vm => vm.Address, map => map.MapFrom(s => s.ContactAddress))
               .ForMember(vm => vm.Mobile, map => map.MapFrom(s => s.ContactMobile))
               .ForMember(vm => vm.DateOfBirth, map => map.MapFrom(s => s.ContactDateOfBirth));
            });

            return AutoMapper.Mapper.Map<Employee>(contact);

            //return new Employee()
            //{
            //    Id = contact.ContactId,
            //    FirstName = contact.ContactFirstName,
            //    LastName = contact.ContactLastName,
            //    Address = contact.ContactAddress,
            //    Mobile = contact.ContactMobile,
            //    DateOfBirth = contact.ContactDateOfBirth
            //};

        }
    }
    

}
