using Crm.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Crm.Identity.Models
{
    internal class User : IdentityUser<Guid>
    {
        private ContactInfo _contactInfo;

        public FullName Name { get; }

        public override string PhoneNumber
        {
            get { return _contactInfo.PhoneNumber; }
            set { _contactInfo = new ContactInfo(_contactInfo.Email, value); }
        }

        public override string Email
        {
            get { return _contactInfo.Email; }
            set { _contactInfo = new ContactInfo(value, _contactInfo.PhoneNumber); }
        }

        public User(FullName fullName, string email, string phoneNumber)
        {
            Id = Guid.NewGuid();
            SecurityStamp = Guid.NewGuid().ToString();
            Name = fullName ?? throw new ArgumentNullException(nameof(fullName));
            _contactInfo = new ContactInfo(email, phoneNumber);
        }
    }
}
