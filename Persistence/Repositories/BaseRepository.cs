using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Domain.Base;
using Domain.Interfaces.Repositories;
using LinqKit;

namespace Persistence.Repositories
{
    public class BaseRepository<TBaseEntity> : IBaseRepository<TBaseEntity> where TBaseEntity : BaseEntity
    {
        private IDbContext _context { get; set; }
        private IDbSet<TBaseEntity> _dbSet { get; set; }
        private bool _disposed;

        public BaseRepository(IDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TBaseEntity>();
        }

        public IEnumerable<TBaseEntity> Get(Expression<Func<TBaseEntity, bool>> predicado)
        {
            return _dbSet.AsExpandable().Where(predicado).ToList();
        }

        public PagedResult<TBaseEntity> Get(int page, int itemCount, Expression<Func<TBaseEntity, bool>> @where)
        {
            if (page <= 0) page = 1;

            var totalItems = Count(@where);

            var totalPages = (int)Math.Ceiling((double)totalItems / itemCount);

            var results = _dbSet.AsExpandable().Where(@where).OrderBy(b => b.Id).Skip((page - 1) * itemCount)
                .Take(itemCount).ToList();

            var resultadoPaginado = new PagedResult<TBaseEntity>() 
            {
                PageItems = itemCount,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                Items = results
            };

            return resultadoPaginado;
        }

        public PagedResult<TBaseEntity> Get<TOrderKey>(int page, int itemCount, Expression<Func<TBaseEntity, bool>> @where, Expression<Func<TBaseEntity, TOrderKey>> ordenarPor, bool ascending = true)
        {
            if (page <= 0) page = 1;

            var totalItems = Count(@where);

            var totalPages = (int)Math.Ceiling((double)totalItems / itemCount);
            
            List<TBaseEntity> results;

            if (ascending)
            {
                results = _dbSet.AsExpandable().Where(@where)
                   .OrderBy(ordenarPor).Skip((page - 1) * itemCount)
                   .Take(itemCount).ToList();
            }
            else
            {
                results = _dbSet.AsExpandable().Where(@where)
                   .OrderByDescending(ordenarPor).Skip((page - 1) * itemCount)
                   .Take(itemCount).ToList();
            }

            var pagedResult = new PagedResult<TBaseEntity>()
            {
                PageItems = itemCount,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                Items = results
            };

            return pagedResult;
        }

        public TBaseEntity GetSingle(Func<TBaseEntity, bool> predicado)
        {
            return _dbSet.AsExpandable().Single(predicado);
        }

        public TBaseEntity GetFirst(Func<TBaseEntity, bool> predicado)
        {
            return _dbSet.AsExpandable().First(predicado);
        }

        public TBaseEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        
        public void Insert(TBaseEntity entidad)
        {
            _context.SetInserted(entidad);
        }

        public virtual void Update(TBaseEntity entidadAActualizar)
        {
            _context.SetModified(entidadAActualizar);
        }



        public virtual void Delete(int id)
        {
            var entidadAEliminar = _dbSet.Find(id);
            Delete(entidadAEliminar);
        }
        public virtual void Delete(TBaseEntity entidadAEliminar)
        {
            _context.SetDeleted(entidadAEliminar);
        }


        public bool Exist(int id)
        {
            return _dbSet.Any(t => t.Id == id);
        }

        public bool Exist(Func<TBaseEntity, bool> predicado)
        {
            return _dbSet.AsNoTracking().Any(predicado);
        }

        public int Count(Expression<Func<TBaseEntity, bool>> predicado)
        {
            return _dbSet.AsNoTracking().AsExpandable().Count(predicado);
        }
        
        public TTtype ExecuteFunction<TTtype>(string sqlQuery, params object[] parameters)
        {

            return _context.Database.SqlQuery<TTtype>(sqlQuery, parameters).Single();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }


        #endregion


    }
}
