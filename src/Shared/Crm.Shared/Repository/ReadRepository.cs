using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    internal class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : Entity, IAggregateRoot
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

        public async Task<IEnumerable<TEntity>> Execute(ICollectionQuery<IEnumerable<TEntity>> query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }
    }
}
