using System;
using System.Collections.Generic;
using System.Data;
using Domain.Base;
using Domain.Interfaces.Repositories;

namespace Domain.Interfaces.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<TBaseEntity> Repository<TBaseEntity>() where TBaseEntity : BaseEntity;
     
        void BeginTransaccion(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        int Commit();

        void Rollback();

        void Dispose(bool disposing);

        IEnumerable<TEntidad> ExecuteQuery<TEntidad>(string sqlQuery, params object[] parameters);

        TTtype ExecuteFunction<TTtype>(string sqlQuery, params object[] parameters);

        int ExecuteCommand(string sqlCommand, params object[] parameters);

    }
}
