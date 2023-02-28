using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    public interface IReadRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        Task<TEntity?> Execute(ISingleQuery<TEntity> query, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> Execute(ICollectionQuery<TEntity> query, CancellationToken cancellationToken);
    }

    public interface ISingleQuery<out TEntity> : IRequest<TEntity?> where TEntity : Entity?
    {
    }

    public interface ICollectionQuery<out TEntity> : IRequest<IEnumerable<TEntity>> where TEntity : Entity
    {
    }
}
