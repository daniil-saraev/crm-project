using PhoneNumbers;
using System.Net.Mail;
using static EmailValidation.EmailValidator;

namespace Crm.Backend.Core.Models.Shared
{
    public class ContactInfo
    {
        public MailAddress Email { get; private set; } = null!;
        public PhoneNumber PhoneNumber { get; private set; } = null!;

        internal ContactInfo(MailAddress email, PhoneNumber phonenumber)
        {
            SetEmail(email);
            SetPhoneNumber(phonenumber);
        }

        internal void SetEmail(MailAddress email)
        {
            if(email == null || !Validate(email.Address))
                throw new ArgumentException(null, nameof(email));
            Email = email;
        }

        internal void SetPhoneNumber(PhoneNumber phoneNumber)
        {
            if(phoneNumber == null || !phoneNumber.IsInitialized)
                throw new ArgumentException(null, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }
    }
}
