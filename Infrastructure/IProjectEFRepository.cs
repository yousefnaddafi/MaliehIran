using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MaliehIran.Infrastructure
{
    public interface IProjectEFRepository<T>
    {
        T Insert(T item);
        Task InsertAsync(T item);
        Task<List<T>> GetAll();
        Task<T> Update(T item);
        void VoidUpdate(T item);
        Task Delete(T entity);
        void Erase(T entity);
        IQueryable<T> GetQuery();
        Task Save();
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        IQueryable<T> SqlExecute(string executable);
        Task<object> DapperSqlQuery(string cmd);
    }
}
