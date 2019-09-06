using System;
using System.Data;
using System.Data.Entity;
using Domain.Base;

namespace Persistence
{
    public interface IDbContext : IDisposable
    {
        IDbSet<TEntidad> Set<TEntidad>() where TEntidad : BaseEntity;

        void SetInserted<TEntidad>(TEntidad entity) where TEntidad : BaseEntity;

        void SetModified<TEntidad>(TEntidad entity) where TEntidad : BaseEntity;

        void SetDeleted<TEntidad>(TEntidad entity) where TEntidad : BaseEntity;

        void BeginTransaccion(IsolationLevel isolationLevel = IsolationLevel.Serializable);

        int Commit();

        void Rollback();

        Database Database { get; }

    }
}
