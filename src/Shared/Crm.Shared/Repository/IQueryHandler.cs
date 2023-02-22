using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
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
