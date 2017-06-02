using System;

namespace Paradise.Api.Legacy.Entities
{
    public class ContactEntity
    {
        public int ContactId { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public DateTime ContactDateOfBirth { get; set; }

        public string ContactAddress { get; set; }

        public string ContactMobile { get; set; }
    }
}
