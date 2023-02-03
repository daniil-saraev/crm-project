export default class Order 
{
    public readonly FirstName: string;
    public readonly LastName: string;
    public readonly Company: string|null;
    public readonly PhoneNumber: string;    
    public readonly Email: string;
    public readonly Description: string;

    constructor(firstName: string, 
        lastName: string, 
        company: string|null, 
        phoneNumber: string, 
        email: string, 
        description: string) 
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Company = company;
        this.PhoneNumber = phoneNumber;
        this.Email = email;
        this.Description = description;
    }
}