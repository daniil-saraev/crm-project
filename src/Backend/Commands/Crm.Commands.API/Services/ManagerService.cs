using Crm.Commands.Managers.Commands;
using Grpc.Core;
using MediatR;
using Ardalis.Result;
using System.Runtime.InteropServices;

namespace Crm.Commands.API.Services
{
    public class ManagerService : Manager.ManagerBase
    {
        private readonly IMediator _mediator;

        public ManagerService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CompleteOrderResult> CompleteOrder(CompleteOrderRequest request, ServerCallContext context)
        {
            Result<Guid> result = await _mediator.Send(new CompleteOrderCommand(
                request.ManagerId,
                request.ClientId,
                request.OrderInWorkId,
                request.CompletionStatus,
                request.Comment),
                context.CancellationToken);
            return result.MapTo<CompleteOrderResult, Guid>();
        }

        public override async Task<EditClientContactInfoResult> EditClientContactInfo(EditClientContactInfoRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new EditClientContactInfoCommand(
                request.ManagerId,
                request.ClientId,
                request.Email,
                request.PhoneNumber),
                context.CancellationToken);
            return result.MapTo<EditClientContactInfoResult>();
        }

        public override async Task<EditClientNameResult> EditClientName(EditClientNameRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new EditClientNameCommand(
                request.ManagerId,
                request.ClientId,
                request.Name),
                context.CancellationToken);
            return result.MapTo<EditClientNameResult>();
        }

        public override async Task<EditOrderDescriptionResult> EditOrderDescription(EditOrderDescriptionRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new EditOrderDescriptionCommand(
                request.ManagerId,
                request.OrderInWorkId,
                request.Description),
                context.CancellationToken);
            return result.MapTo<EditOrderDescriptionResult>();
        }
    }
}
