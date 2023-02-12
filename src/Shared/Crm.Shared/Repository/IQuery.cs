using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    public interface ISingleQuery<out TEntity> : IRequest<TEntity?> where TEntity : Entity
    {
    }

    public interface ICollectionQuery<out TCollection> : IRequest<TCollection> where TCollection : IEnumerable<Entity>
    {
    }
}
