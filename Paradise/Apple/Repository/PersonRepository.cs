using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apple.Models;

namespace Apple.Repository
{
    public class PersonRepository
    {
        private List<Person> _persons = new List<Person>
        {
            new Person()
            {
                Id = 1,
                FirstName = "Sundar",
                LastName = "Urs",
                Address = "R. R. Nagar, Bangalore",
                DateOfBirth = Convert.ToDateTime("10/10/1985"),
                Mobile = "9972032425"
            },
            new Person()
            {
                Id = 2,
                FirstName = "Pavan",
                LastName = "Kumar",
                Address = "Vijayanagar, Bangalore",
                DateOfBirth = Convert.ToDateTime("10/10/1988"),
                Mobile = "9988776655"
            }
        };

        public List<Person> GetAll()
        {
            return _persons;
        }

        public List<Person> Add(Person person)
        {
            _persons.Add(person);
            return _persons;
        }

        public Person Get(int id)
        {
            return _persons.FirstOrDefault(p => p.Id == id);
        }

        public List<Person> Delete(int id)
        {
            var person = _persons.FirstOrDefault(p => p.Id == id);
            _persons.Remove(person);
            return _persons;
        }

    }
}