using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.Base;

namespace Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
    {
        IEnumerable<TBaseEntity> Get(Expression<Func<TBaseEntity, bool>> predicado);

        PagedResult<TBaseEntity> Get(int page, int itemCount, Expression<Func<TBaseEntity, bool>> @where);

        PagedResult<TBaseEntity> Get<TOrderBy>(int page, int itemCount, Expression<Func<TBaseEntity, bool>> @where, Expression<Func<TBaseEntity, TOrderBy>> orderBy, bool ascending = true);
        
        TBaseEntity GetSingle(Func<TBaseEntity, bool> @where);

        TBaseEntity GetFirst(Func<TBaseEntity, bool> @where);
        
        TBaseEntity GetById(int id);

        void Insert(TBaseEntity entity);

        void Update(TBaseEntity entity);

        void Delete(int id);

        void Delete(TBaseEntity entity);

        bool Exist(int id);

        bool Exist(Func<TBaseEntity, bool> predicado);

        int Count(Expression<Func<TBaseEntity, bool>> predicado);
    }
}
