using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.DataAccess
{
    public class DataService<T> : IDataService<T> where T : class
    {
        protected IUnitOfWork _uow;
        protected DbSet<T> TEntity;
        public IQueryable<T> Entities { get; set; }

        protected DataService(IUnitOfWork uow)
        {
            _uow = uow;
            TEntity = _uow.Set<T>();
        }

        public int Update(T entity)
        {
            TEntity.Update(entity);
            return SaveChanges();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            TEntity.Update(entity);
            return await SaveChangesAsync();
        }

        public virtual void Add(T entity)
        {
            TEntity.Add(entity);
        }

        public virtual void Remove(T entity)
        {
            TEntity.Remove(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await TEntity.CountAsync(predicate);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return TEntity.Count(predicate);
        }

        public int Count(List<Expression<Func<T, bool>>> predicates)
        {
            var query = TEntity.Where(predicates[0]);
            foreach (var cond in predicates)
            {
                query = query.Where(cond);
            }
            return query.Count();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await TEntity.Where(predicate).Skip(0).Take(1).FirstOrDefaultAsync();
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return TEntity.Where(predicate).Skip(0).Take(1).FirstOrDefault();
        }

        public virtual IList<T> GetPartOptional(List<Expression<Func<T, bool>>> predicate, int startIndex, int size)
        {
            var query = TEntity.Where(predicate[0]);
            foreach (var cond in predicate)
            {
                query = query.Where(cond);
            }
            return query.Skip(startIndex).Take(size).ToList();
        }

        public virtual IList<T> GetPart(Expression<Func<T, bool>> predicate, int startIndex, int size)
        {
            return TEntity.Where(predicate).Skip(startIndex).Take(size).ToList();
        }

        public virtual IList<T> GetPart(Expression<Func<T, bool>> predicate, int startIndex, int size, Expression<Func<T, int>> orderKey, bool desc)
        {
            if (desc)
            {
                return TEntity.Where(predicate).OrderByDescending(orderKey).Skip(startIndex).Take(size).ToList();
            }
            else
            {
                return TEntity.Where(predicate).OrderBy(orderKey).Skip(startIndex).Take(size).ToList();
            }

        }

        public virtual IList<T> GetPart(List<Expression<Func<T, bool>>> predicates, int startIndex, int size, Expression<Func<T, int>> orderKey, bool desc)
        {
            var query = TEntity.Where(predicates[0]);
            int cc = 0;
            foreach (var cond in predicates)
            {
                cc++;
                if (cc != 1)
                {
                    query = query.Where(cond);
                }
            }


            if (desc)
            {
                return query.OrderByDescending(orderKey).Skip(startIndex).Take(size).ToList();
            }
            else
            {
                return query.OrderBy(orderKey).Skip(startIndex).Take(size).ToList();
            }

        }

        public virtual IList<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return TEntity.Where(predicate).ToList();
        }

        public virtual IList<T> GetAll(List<Expression<Func<T, bool>>> predicates)
        {
            var query = TEntity.Where(predicates[0]);
            foreach (var cond in predicates)
            {
                query = query.Where(cond);
            }
            return query.ToList();
        }

        public virtual IList<TResult> GetAll<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selectItem, int startIndex, int size)
        {
            return TEntity.Where(predicate).Skip(startIndex).Take(size).Select(selectItem).ToList();
        }

        public virtual IList<TResult> GetAll<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selectItem)
        {
            return TEntity.Where(predicate).Select(selectItem).ToList();
        }

        public virtual IList<TResult> GetAll<TResult>(List<Expression<Func<T, bool>>> predicates, Expression<Func<T, TResult>> selectItem)
        {
            var query = TEntity.Where(predicates[0]);
            foreach (var cond in predicates)
            {
                query = query.Where(cond);
            }
            return query.Select(selectItem).ToList();
        }

        public List<Expression<Func<T, bool>>> ExpressionMaker()
        {
            return new List<Expression<Func<T, bool>>>();
        }


        public IQueryable<TResult> QueryMaker<TResult>(Func<IQueryable<T>, IQueryable<TResult>> queryFunction)
        {
            return queryFunction(TEntity);
        }

        public IList<TResult> QueryRun<TResult>(IQueryable<TResult> queryFunction)
        {
            return queryFunction.ToList();
        }



        public int ExecSqlCommand(string command)
        {
            return _uow.DataFace.ExecuteSqlRaw(command);
        }

        public async Task<int> ExecSqlCommandAsync(string command)
        {
            return await _uow.DataFace.ExecuteSqlRawAsync(command);
        }

        public int ExecSqlCommand(string command, object[] values)
        {
            return _uow.DataFace.ExecuteSqlRaw(command, values);
        }

        public async Task<int> ExecSqlCommandAsync(string command, object[] values)
        {
            return await _uow.DataFace.ExecuteSqlRawAsync(command, values);
        }

        public List<T> GetDataFromSp(string command, object[] values)
        {
            return TEntity.FromSqlRaw<T>(command, values).ToList();
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _uow.SaveChangeAsync();
        }

        public int SaveChanges()
        {
            return _uow.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}