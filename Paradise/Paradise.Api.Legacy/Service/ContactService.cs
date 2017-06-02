using System.Linq;
using Paradise.Api.Legacy.DataSource;
using Paradise.Api.Legacy.Entities;

namespace Paradise.Api.Legacy.Service
{
    public class ContactService
    {
        public ContactEntity GetContact(int id)
        {
            return DataStore.Contacts().FirstOrDefault(contactEntity => contactEntity.ContactId == id);
        }
    }
}
