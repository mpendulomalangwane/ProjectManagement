using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IRepository<T> where T: class
    {

        IQueryable<T> GetAll();
        T GetById(Guid Id);
        IEnumerable<U> GetSpecific<U>(Expression<Func<T, U>> columns = null, Expression<Func<T, bool>> where = null);
        void Add(T entity);
        void Update(T entity);
        void DeleteById(Guid Id);
        void Delete(T entity);
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);

    }
}
