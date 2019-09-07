using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.Base;

namespace Domain.Interfaces.Application
{
    public interface IBaseService<TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
    {
        IEnumerable<TBaseEntity> Get(Expression<Func<TBaseEntity, bool>> @where);

        PagedResult<TBaseEntity> Get(int page, int itemsCount, Expression<Func<TBaseEntity, bool>> @where);

        PagedResult<TBaseEntity> Get<TOrderBy>(int page, int itemsCount, Expression<Func<TBaseEntity, bool>> @where, Expression<Func<TBaseEntity, TOrderBy>> @orderBy, bool ascending = true);


        TBaseEntity GetById(int id);

        void Create(TBaseEntity entidad);

        void Update(TBaseEntity entidad);

        void Delete(int id);

        bool Exist(int id);

        bool Exist(Func<TBaseEntity, bool> @where);

        int Count(Expression<Func<TBaseEntity, bool>> @where);
    }
}
