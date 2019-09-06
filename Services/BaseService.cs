using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;

namespace Services
{
    public class BaseService<TBaseEntity> : IBaseService<TBaseEntity>
         where TBaseEntity : BaseEntity
    {
        public IUnitOfWork UnitOfWork { get; private set; }
        private readonly IBaseRepository<TBaseEntity> _repository;
        private bool _disposed;

        public BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _repository = UnitOfWork.Repository<TBaseEntity>();
        }

        public IEnumerable<TBaseEntity> Get(Expression<Func<TBaseEntity, bool>> @where)
        {
            return _repository.Get(@where);
        }

        public PagedResult<TBaseEntity> Get(int page, int itemsCount, Expression<Func<TBaseEntity, bool>> @where)
        {
            return _repository.Get(page, itemsCount, @where);
        }

        public PagedResult<TBaseEntity> Get<TOrderBy>(int page, int itemsCount, Expression<Func<TBaseEntity, bool>> @where, Expression<Func<TBaseEntity, TOrderBy>> orderBy, bool ascending = true)
        {
            return _repository.Get(page, itemsCount, @where, @orderBy, ascending);
        }

        public TBaseEntity GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Create(TBaseEntity entidad)
        {
            _repository.Insert(entidad);
            UnitOfWork.Commit();
        }

        public void Update(TBaseEntity entidad)
        {
            _repository.Update(entidad);
            UnitOfWork.Commit();
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
            UnitOfWork.Commit();
        }

        public bool Exist(int id)
        {
            return _repository.Exist(id);
        }

        public bool Exist(Func<TBaseEntity, bool> @where)
        {
            return _repository.Exist(where);
        }

        public int Count(Expression<Func<TBaseEntity, bool>> @where)
        {
            return _repository.Count(@where);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _repository.Dispose();
            }
            _disposed = true;
        }


    }
}
