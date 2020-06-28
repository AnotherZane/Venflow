using Npgsql;
using System;
using System.Text;
using Venflow.Dynamic;

namespace Venflow.Modeling
{
    internal class Entity<TEntity> : Entity where TEntity : class
    {
        internal EntityColumnCollection<TEntity> Columns { get; }
        internal PrimaryEntityColumn<TEntity> PrimaryColumn { get; }

        internal Action<TEntity, StringBuilder, string, NpgsqlParameterCollection> InsertWriter { get; }
        internal Func<ChangeTracker<TEntity>, TEntity>? ChangeTrackerFactory { get; }
        internal Func<ChangeTracker<TEntity>, TEntity, TEntity>? ChangeTrackerApplier { get; }

        internal MaterializerFactory<TEntity> MaterializerFactory { get; }

        internal Entity(Type entityType, Type? proxyEntityType, string tableName, EntityColumnCollection<TEntity> columns, PrimaryEntityColumn<TEntity> primaryColumn, string columnListString, string explicitColumnListString, string nonPrimaryColumnListString, Action<TEntity, StringBuilder, string, NpgsqlParameterCollection> insertWriter, Func<ChangeTracker<TEntity>, TEntity>? changeTrackerFactory, Func<ChangeTracker<TEntity>, TEntity, TEntity>? changeTrackerApplier) : base(entityType, proxyEntityType, tableName, columnListString, explicitColumnListString, nonPrimaryColumnListString)
        {
            InsertWriter = insertWriter;
            ChangeTrackerFactory = changeTrackerFactory;
            ChangeTrackerApplier = changeTrackerApplier;
            Columns = columns;
            PrimaryColumn = primaryColumn;

            MaterializerFactory = new MaterializerFactory<TEntity>(this);
        }

        internal TEntity GetProxiedEntity(bool trackChanges = false)
        {
            return ChangeTrackerFactory.Invoke(new ChangeTracker<TEntity>(Columns.Count, trackChanges));
        }

        internal TEntity ApplyChangeTracking(TEntity entity)
        {
            return ChangeTrackerApplier.Invoke(new ChangeTracker<TEntity>(Columns.Count, false), entity);
        }

        internal override EntityColumn GetPrimaryColumn()
        {
            return PrimaryColumn;
        }

        internal override EntityColumn GetColumn(string columnName)
        {
            return Columns[columnName];
        }

        internal override bool TryGetColumn(string columnName, out EntityColumn? entityColumn)
        {
            if (Columns.TryGetValue(columnName, out var tempColumn))
            {
                entityColumn = tempColumn;

                return true;
            }
            else
            {
                entityColumn = null;

                return false;
            }
        }
    }

    internal abstract class Entity
    {
        internal string EntityName { get; }
        internal string TableName { get; }
        internal string RawTableName { get; }

        internal Type EntityType { get; }
        internal Type? ProxyEntityType { get; }

        internal DualKeyCollection<string, EntityRelation>? Relations { get; set; }

        internal string ColumnListString { get; }
        internal string ExplicitColumnListString { get; }
        internal string NonPrimaryColumnListString { get; }

        protected Entity(Type entityType, Type? proxyEntityType, string tableName, string columnListString, string explicitColumnListString, string nonPrimaryColumnListString)
        {
            EntityType = entityType;
            ProxyEntityType = proxyEntityType;
            EntityName = entityType.Name;
            TableName = "\"" + tableName + "\"";
            RawTableName = tableName;
            ColumnListString = columnListString;
            ExplicitColumnListString = explicitColumnListString;
            NonPrimaryColumnListString = nonPrimaryColumnListString;
        }

        internal abstract EntityColumn GetPrimaryColumn();
        internal abstract EntityColumn GetColumn(string columnName);
        internal abstract bool TryGetColumn(string columnName, out EntityColumn? entityColumn);
    }
}
