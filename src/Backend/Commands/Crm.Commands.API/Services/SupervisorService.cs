using Ardalis.Result;
using Crm.Commands.Supervisors.Commands;
using Grpc.Core;
using MediatR;

namespace Crm.Commands.API.Services
{
    public class SupervisorService : Supervisor.SupervisorBase
    {
        private readonly IMediator _mediator;

        public SupervisorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async override Task<AddNewManagerResult> AddNewManager(AddNewManagerRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new AddNewManagerCommand(
                request.SupervisorId,
                request.ManagerAccountId),
                context.CancellationToken);
            return result.MapTo<AddNewManagerResult>();
        }

        public override async Task<AssignClientResult> AssignClient(AssignClientRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new AssignClientCommand(
                request.SupervisorId,
                request.ManagerId,
                request.ClientId),
                context.CancellationToken);
            return result.MapTo<AssignClientResult>();
        }

        public override async Task<TransferClientResult> TransferClient(TransferClientRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new TransferClientCommand(
                request.SupervisorId,
                request.FromManagerId,
                request.ToManagerId,
                request.ClientId),
                context.CancellationToken);
            return result.MapTo<TransferClientResult>();
        }

        public override async Task<TransferManagerResult> TransferManager(TransferManagerRequest request, ServerCallContext context)
        {
            Result result = await _mediator.Send(new TransferManagerCommand(
                request.FromSupervisorId,
                request.ToSupervisorId,
                request.ManagerId),
                context.CancellationToken);
            return result.MapTo<TransferManagerResult>();
        }
    }
}
