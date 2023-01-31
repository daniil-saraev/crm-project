using Ardalis.GuardClauses;

namespace Crm.Shared.Models
{
    public class FullName
    {
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }

        public FullName(string firstName, string middleName, string lastName)
        {
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            MiddleName = Guard.Against.NullOrWhiteSpace(middleName, nameof(middleName));
        }

        public override string ToString()
        {
            return $"{LastName} {FirstName} {MiddleName}";
        }
    }
}
