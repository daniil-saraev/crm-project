using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    public interface IReadRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> Execute(ISingleQuery<TEntity> query, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> Execute(ICollectionQuery<TEntity> query, CancellationToken cancellationToken);
    }

    public interface ISingleQuery<out TEntity> : IRequest<TEntity?> where TEntity : Entity
    {
    }

    public interface ICollectionQuery<out TEntity> : IRequest<IEnumerable<TEntity>> where TEntity : Entity
    {
    }

    public interface ISingleQueryHandler<in TSingleQuery, TEntity> : IRequestHandler<TSingleQuery, TEntity?>
        where TSingleQuery : ISingleQuery<TEntity>
        where TEntity : Entity
    {
    }

    public interface ICollectionQueryHandler<in TCollectionQuery, TEntity> : IRequestHandler<TCollectionQuery, IEnumerable<TEntity>>
        where TCollectionQuery : ICollectionQuery<TEntity>
        where TEntity : Entity
    {
    }
}
