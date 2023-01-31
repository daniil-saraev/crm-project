using EmailValidation;

namespace Crm.Shared.Models
{
    public class ContactInfo
    {
        public string Email { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;

        public ContactInfo(string email, string phoneNumber)
        {
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
        }

        public void SetEmail(string email)
        {
            if(email == null || !EmailValidator.Validate(email))
                throw new ArgumentException(null, nameof(email));
            Email = email;
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            if(!IsValidNumber(phoneNumber))
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
