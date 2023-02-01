using Crm.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Crm.Identity.Models
{
    internal class User : IdentityUser
    {
        private readonly ContactInfo _contactInfo;

        public FullName Name { get; }

        public override string PhoneNumber
        {
            get { return _contactInfo.PhoneNumber; }
            set { _contactInfo.SetPhoneNumber(value); }
        }

        public override string Email
        {
            get { return _contactInfo.Email; }
            set { _contactInfo.SetEmail(value); }
        }

        public User(FullName fullName, string email, string phoneNumber) : base()
        {
            Name = fullName ?? throw new ArgumentNullException(nameof(fullName));
            _contactInfo = new ContactInfo(email, phoneNumber);
        }
    }
}
