using BookStore.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStore.Repository.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private ApplicationDbContext db = null;
        private DbSet<T> table = null;
        public GenericRepository()
        {
            this.db = new ApplicationDbContext();
            table = db.Set<T>();
        }

        public GenericRepository(ApplicationDbContext db)
        {
            this.db = db;
            table = db.Set<T>();
        }
        public virtual IEnumerable<T> GetAll()
        {
            return table.ToList();
        }

        public IOrderedQueryable<T> GetAll1()
        {
            return (IOrderedQueryable<T>)table.ToList();
        }

        public virtual IEnumerable<T> Gets(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual T Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }

        public virtual IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {
            return table.SqlQuery(query, parameters).ToList();
        }

        public virtual object GetObjectWithRawSql(string query, params object[] parameters)
        {
            return table.SqlQuery(query, parameters).ToList();
        }

        public virtual T GetById(object id)
        {
            return table.Find(id);
        }
        public virtual void Insert(T obj)
        {
            table.Add(obj);
            db.SaveChanges();
        }
        public virtual void Insert(IEnumerable<T> list)
        {
            table.AddRange(list);
            db.SaveChanges();
        }

        public virtual void Update(T obj)
        {
            table.Attach(obj);
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }
        public virtual void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
            db.SaveChanges();
        }
        public virtual void Delete(IEnumerable<T> list)
        {
            table.RemoveRange(list);
            db.SaveChanges();
        }
        public virtual void Delete(T entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                table.Attach(entityToDelete);
            }
            table.Remove(entityToDelete);
        }
        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }
    }
}