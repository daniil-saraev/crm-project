using Crm.Shared.Models;

namespace Crm.Shared.Repository
{
    public interface IWriteRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        Task Update(TEntity entity, CancellationToken cancellationToken);
        Task Update(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        Task Delete(TEntity entity, CancellationToken cancellationToken);
        Task Delete(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        Task Add(TEntity entity, CancellationToken cancellationToken);
        Task Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
