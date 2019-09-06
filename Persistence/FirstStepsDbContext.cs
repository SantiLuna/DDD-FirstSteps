using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Domain.Base;

namespace Persistence
{
    public sealed class FirstStepsDbContext : DbContext, IDbContext
    {
        private ObjectContext _objectContext;
        private DbTransaction _transaction;
        
        public FirstStepsDbContext() : base("FirstStepsConnectionString")
        {
            Database.SetInitializer<FirstStepsDbContext>(null);
        }

        public FirstStepsDbContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<FirstStepsDbContext>(null);
        }

        public static FirstStepsDbContext Crear()
        {
            return new FirstStepsDbContext("FirstStepsConnectionString");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

     
        public new IDbSet<TEntidad> Set<TEntidad>() where TEntidad : BaseEntity
        {
            return base.Set<TEntidad>();
        }

        public void SetInserted<TEntidad>(TEntidad entity) where TEntidad : BaseEntity
        {
            UpdateEntityState(entity, EntityState.Added);
        }

        public void SetModified<TEntidad>(TEntidad entity) where TEntidad : BaseEntity
        {
            UpdateEntityState(entity, EntityState.Modified);
        }

        public void SetDeleted<TEntidad>(TEntidad entity) where TEntidad : BaseEntity
        {
            UpdateEntityState(entity, EntityState.Deleted);
        }

        public void BeginTransaccion(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _objectContext = ((IObjectContextAdapter)this).ObjectContext;
            if (_objectContext.Connection.State == ConnectionState.Open)
            {
                return;
            }

            _objectContext.Connection.Open();
            _transaction = _objectContext.Connection.BeginTransaction(isolationLevel);
        }

        public int Commit()
        {
            try
            {
                BeginTransaccion();
                var saveChanges = SaveChanges();
                _transaction.Commit();

                return saveChanges;
            }
            catch (Exception ex)
            {
                Rollback();
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        private void UpdateEntityState<TEntidad>(TEntidad entidad, EntityState entityState) where TEntidad : BaseEntity
        {
            var dbEntityEntry = GetSafeDbEntityEntryState(entidad);
            dbEntityEntry.State = entityState;
        }

        private DbEntityEntry GetSafeDbEntityEntryState<TEntidad>(TEntidad entidad) where TEntidad : BaseEntity
        {
            var dbEntityEntry = Entry(entidad);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                base.Set<TEntidad>().Attach(entidad);
            }
            return dbEntityEntry;
        }


        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            return result;
        }


        object GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        private long? GetKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            long id = 0;
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);

            return id;
        }

        private string GetTableName(DbEntityEntry entry)
        {
            TableAttribute tableAttr = entry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            string tableName = tableAttr != null ? tableAttr.Name : entry.Entity.GetType().Name;
            return tableName;
        }

     
    }
}
