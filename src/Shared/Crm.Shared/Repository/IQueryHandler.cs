using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    public interface ISingleQueryHandler<in TSingleQuery, TEntity> : IRequestHandler<TSingleQuery, TEntity?> 
        where TSingleQuery : ISingleQuery<TEntity> 
        where TEntity : Entity
    {
    }

    public interface ICollectionQueryHandler<in TCollectionQuery, TCollection> : IRequestHandler<TCollectionQuery, TCollection>
        where TCollectionQuery : ICollectionQuery<TCollection>
        where TCollection : IEnumerable<Entity>
    {
    }
}
