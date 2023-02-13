using EmailValidation;

namespace Crm.Shared.Models
{
    public class ContactInfo : ValueObject
    { 
        public string Email { get; } = null!;
        public string PhoneNumber { get; } = null!;

        private ContactInfo() { }

        public ContactInfo(string email, string phoneNumber)
        {
            if (email == null || !EmailValidator.Validate(email))
                throw new ArgumentException(null, nameof(email));
            Email = email;

            if (!IsValidNumber(phoneNumber))
                throw new ArgumentException(null, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }

        private bool IsValidNumber(string number)
        {
            if (number == null)
                return false;
            if ((number).Length != 12)
                return false;
            if (number[0] != '+')
                return false;
            if (!(number).Substring(1).All(c => char.IsDigit(c)))
                return false;
            return true;
        }
    }
}
