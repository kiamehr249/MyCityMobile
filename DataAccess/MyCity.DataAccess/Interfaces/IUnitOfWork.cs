using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace MyCity.DataAccess
{
    public interface IUnitOfWork
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DatabaseFacade DataFace { get; }
        Task<int> SaveChangeAsync();
        int SaveChanges();
    }
}
