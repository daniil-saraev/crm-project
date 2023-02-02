export default class Order 
{
    FirstName: string;
    LastName: string;
    Company: string|null;
    PhoneNumber: string;    
    Email: string;
    Description: string;

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