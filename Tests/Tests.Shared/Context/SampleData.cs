using Crm.Core.Clients;
using Crm.Core.Managers;
using Crm.Core.Orders;
using Crm.Core.Supervisors;
using Crm.Shared.Models;
using static Crm.Core.Orders.CompletedOrder;

namespace Tests.Shared.Context
{
    internal class SampleData
    {
        public readonly Supervisor JohnAndBenManagerSupervisor, BillAndJakeManagerSupervisor;

        public readonly Manager JohnManager, BenManager, BillManager, JakeManager;

        public readonly Client John, Ben, Bill, Jake;

        public readonly CreatedOrder JohnCreatedOrder, BenCreatedOrder, BillCreatedOrder, JakeCreatedOrder;

        public readonly OrderInWork JohnOrderInWork, BenOrderInWork, BillOrderInWork, JakeOrderInWork;

        public readonly CompletedOrder JohnCompletedOrder, BenCompletedOrder, BillCompletedOrder, JakeCompletedOrder;

        public SampleData()
        {
            JohnAndBenManagerSupervisor = Supervisor.New(Guid.NewGuid());
            BillAndJakeManagerSupervisor = Supervisor.New(Guid.NewGuid());

            JohnManager = Manager.New(Guid.NewGuid(), JohnAndBenManagerSupervisor.Id);
            BenManager = Manager.New(Guid.NewGuid(), JohnAndBenManagerSupervisor.Id);
            BillManager = Manager.New(Guid.NewGuid(), BillAndJakeManagerSupervisor.Id);
            JakeManager = Manager.New(Guid.NewGuid(), BillAndJakeManagerSupervisor.Id);

            John = new("John", new ContactInfo("john@mail.com", "+79998887766"));
            Ben = new("Ben", new ContactInfo("ben@mail.com", "+79998887755"));
            Bill = new("Bill", new ContactInfo("john@mail.com", "+79998887744"));
            Jake = new("Jake", new ContactInfo("john@mail.com", "+79998887733"));

            JohnCreatedOrder = new(John.Id, "Mobile app");
            BenCreatedOrder = new(Ben.Id, "Desktop app");
            BillCreatedOrder = new(Bill.Id, "Web app");
            JakeCreatedOrder = new(Jake.Id, "Telegram bot");

            JohnOrderInWork = new(JohnManager.Id, John.Id, new DateTimeOffset(new DateTime(2022, 10, 25)), "Web page");
            BenOrderInWork = new(BenManager.Id, Ben.Id, new DateTimeOffset(new DateTime(2022, 11, 25)), "Bug fixes");
            BillOrderInWork = new(BillManager.Id, Bill.Id, new DateTimeOffset(new DateTime(2022, 12, 11)), "Consulting");
            JakeOrderInWork = new(JakeManager.Id, Jake.Id, new DateTimeOffset(new DateTime(2023, 1, 10)), "Design");

            JohnCompletedOrder = new(
                John.Id, JohnManager.Id, new DateTimeOffset(new DateTime(2021, 5, 10)), new DateTimeOffset(new DateTime(2022, 2, 24)), "Telegram bot", CompletionStatus.Fulfilled, "Fulfilled");
            BenCompletedOrder = new(
                Ben.Id, BenManager.Id, new DateTimeOffset(new DateTime(2020, 4, 15)), new DateTimeOffset(new DateTime(2021, 3, 25)), "Desktop app", CompletionStatus.Canceled, "Found better price");
            BillCompletedOrder = new(
                Bill.Id, BillManager.Id, new DateTimeOffset(new DateTime(2020, 6, 20)), new DateTimeOffset(new DateTime(2020, 6, 21)), "PC repair", CompletionStatus.Rejected, "Not our service");
            JakeCompletedOrder = new(
                Jake.Id, JakeManager.Id, new DateTimeOffset(new DateTime(2019, 9, 9)), new DateTimeOffset(new DateTime(2020, 12, 25)), "Mobile app", CompletionStatus.Fulfilled, "Fulfilled");
        }
    }
}
