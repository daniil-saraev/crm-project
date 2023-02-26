namespace Tests.Commands.Core.Clients
{
    public class ClientTests
    {
        [Fact]
        public void CreateClientTest()
        {
            Assert.Throws<ArgumentException>(() => new Client("", new ContactInfo("email@mail.com", "+79998887766")));
            Assert.Throws<ArgumentException>(() => new Client("John", new ContactInfo("", "+79998887766")));
            Assert.Throws<ArgumentException>(() => new Client("John", new ContactInfo("email@mail.com", "")));
            Assert.Throws<ArgumentException>(() => new Client("John", new ContactInfo("email@mail.com", "phoneNumber")));
            Assert.Throws<ArgumentException>(() => new Client("John", new ContactInfo("email", "+79998887766")));
        }

        [Fact]
        public void PlaceOrderTest()
        {
            // Arrange
            var client = new Client("Jim", new ContactInfo("email@mail.com", "+79998887766"));
            // Act 
            client.PlaceOrder("NewOrder");
            // Assert
            Assert.True(client.CreatedOrders.Single().Description == "NewOrder");
            Assert.True(client.CreatedOrders.Single().ClientId == client.Id);
            Assert.Throws<ArgumentException>(() => client.PlaceOrder(""));
        }
    }
}
