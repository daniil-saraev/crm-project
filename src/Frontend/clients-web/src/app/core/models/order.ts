export default class Order {
    public readonly Name: string;
    public readonly PhoneNumber: string;
    public readonly Email: string;
    public readonly Description: string;

    constructor(
        name: string,
        phoneNumber: string,
        email: string,
        description: string
    ) {
        this.Name = name;
        this.PhoneNumber = phoneNumber;
        this.Email = email;
        this.Description = description;
    }
}
