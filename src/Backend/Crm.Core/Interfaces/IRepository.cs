using Ardalis.Specification;
using Crm.Core.Models;

namespace Crm.Core.Interfaces
{
    public interface IRepository<T> : IRepositoryBase<T> where T : Entity, IAggregateRoot
    {

    }
}
