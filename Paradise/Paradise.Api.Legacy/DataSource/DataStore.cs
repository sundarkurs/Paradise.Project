using System;
using System.Collections.Generic;
using Paradise.Api.Legacy.Entities;

namespace Paradise.Api.Legacy.DataSource
{
    public static class DataStore
    {
        public static List<ContactEntity> Contacts()
        {
            return new List<ContactEntity>
            {
                new ContactEntity()
                {
                    ContactId = 1,
                    ContactFirstName = "Sundar",
                    ContactLastName = "Urs",
                    ContactAddress = "R. R. Nagar, Bangalore",
                    ContactDateOfBirth = Convert.ToDateTime("10/10/1985"),
                    ContactMobile = "9972032425"
                },
                new ContactEntity()
                {
                    ContactId = 1,
                    ContactFirstName = "Pavan",
                    ContactLastName = "Kumar",
                    ContactAddress = "Vijayanagar, Bangalore",
                    ContactDateOfBirth = Convert.ToDateTime("10/10/1988"),
                    ContactMobile = "9988776655"
                }
            };
        }
    }
}
