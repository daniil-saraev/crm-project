using Crm.Shared.Models;
using MediatR;

namespace Crm.Shared.Repository
{
    public interface ISingleQuery<out TEntity> : IRequest<TEntity?> where TEntity : Entity
    {
    }

    public interface ICollectionQuery<out TEntity> : IRequest<IEnumerable<TEntity>> where TEntity : Entity
    {
    }
}
