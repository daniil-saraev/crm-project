using Crm.Shared.Models;
using Crm.Shared.Repository;
using MediatR;

namespace Crm.Commands.Data.Repository
{
    internal class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : Entity
    {
        private readonly IMediator _mediator;

        public ReadRepository(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TEntity?> Execute(ISingleQuery<TEntity> query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> Execute(ICollectionQuery<TEntity> query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }
    }
}
