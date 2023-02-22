using Crm.Shared.Models;

namespace Crm.Shared.Repository
{
    public interface IReadRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> Execute(ISingleQuery<TEntity> query, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> Execute(ICollectionQuery<TEntity> query, CancellationToken cancellationToken);
    }
}
