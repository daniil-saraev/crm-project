using Crm.Data.Context;
using Crm.Shared.Models;
using Crm.Shared.Repository;

namespace Crm.Data.Services
{
    internal class Repository<TEntity> : IWriteRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }

        public async Task Add(TEntity entity, CancellationToken cancellationToken)
        {
            await _context.SingleInsertAsync(entity, cancellationToken);
        }

        public async Task Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkInsertAsync(entities, cancellationToken);
        }

        public async Task Delete(TEntity entity, CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>().SingleDeleteAsync(entity, cancellationToken);
        }

        public async Task Delete(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>().BulkDeleteAsync(entities, cancellationToken);
        }

        public async Task SaveChanges(CancellationToken cancellationToken)
        {
            await _context.BulkSaveChangesAsync(cancellationToken);
        }

        public async Task Update(TEntity entity, CancellationToken cancellationToken)
        {
            await _context.SingleUpdateAsync(entity, cancellationToken);
        }

        public async Task Update(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkUpdateAsync(entities, cancellationToken);
        }
    }
}
