using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Domain.Base;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UoW;
using Persistence.Repositories;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;
        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<TBaseEntity> Repository<TBaseEntity>() where TBaseEntity : BaseEntity
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TBaseEntity).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IBaseRepository<TBaseEntity>)_repositories[type];
            }

            var typeRepository = typeof(BaseRepository<>);

            _repositories.Add(type, Activator.CreateInstance(typeRepository.MakeGenericType(typeof(TBaseEntity)), _context));

            return (IBaseRepository<TBaseEntity>)_repositories[type];
        }

     
        public void BeginTransaccion(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _context.BeginTransaccion(isolationLevel);
        }

        public int Commit()
        {
            return _context.Commit();
        }

        public void Rollback()
        {
            _context.Rollback();
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, params object[] parameters)
        {
            return _context.Database.SqlQuery<TEntity>(sqlQuery, parameters).ToList();
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            try
            {


                return _context.Database.ExecuteSqlCommand(sqlCommand, parameters);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                foreach (IDisposable repositorio in _repositories.Values)
                {
                    repositorio.Dispose();
                }
            }
            _disposed = true;
        }

        #endregion
    }
}
