using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Venflow.Enums;

namespace Venflow.Modeling.Definitions
{
    internal class EntityBuilder<TEntity> : EntityBuilder, IEntityBuilder<TEntity> where TEntity : class
    {
        internal override Type Type { get; }

        internal ChangeTrackerFactory<TEntity>? ChangeTrackerFactory { get; private set; }
        internal Action<TEntity, StringBuilder, string, NpgsqlParameterCollection> InsertWriter { get; private set; }
        internal string TableName { get; private set; }
        internal IDictionary<string, ColumnDefinition<TEntity>> ColumnDefinitions { get; }

        private readonly HashSet<string> _ignoredColumns;

        internal EntityBuilder()
        {
            Type = typeof(TEntity);
            ColumnDefinitions = new Dictionary<string, ColumnDefinition<TEntity>>();
            _ignoredColumns = new HashSet<string>();
        }

        public IEntityBuilder<TEntity> MapToTable(string tableName)
        {
            TableName = tableName;

            return this;
        }

        public IEntityBuilder<TEntity> MapColumn<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector, string columnName)
        {
            var property = ValidatePropertySelector<TTarget>(propertySelector);

            if (ColumnDefinitions.TryGetValue(property.Name, out var definition))
            {
                definition.Name = columnName;
            }
            else
            {
                definition = new ColumnDefinition<TEntity>(columnName);

                ColumnDefinitions.Add(property.Name, definition);
            }

            return this;
        }

        public IEntityBuilder<TEntity> Ignore<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector)
        {
            var property = ValidatePropertySelector<TTarget>(propertySelector);

            _ignoredColumns.Add(property.Name);

            return this;
        }

        public IEntityBuilder<TEntity> MapId<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector, DatabaseGeneratedOption option)
        {
            var property = ValidatePropertySelector<TTarget>(propertySelector);

            var isServerSideGenerated = option != DatabaseGeneratedOption.None;

            var columnDefinition = new PrimaryColumnDefinition<TEntity>(property.Name)
            {
                IsServerSideGenerated = isServerSideGenerated
            };

            if (ColumnDefinitions.TryGetValue(property.Name, out var definition))
            {
                columnDefinition.Name = definition.Name;

                ColumnDefinitions.Remove(property.Name);
            }

            ColumnDefinitions.Add(property.Name, columnDefinition);

            return this;
        }

        public IEntityBuilder<TEntity> MapOneToOne<TRelation, TForeignKey>(Expression<Func<TEntity, TRelation>> relationSelector, Expression<Func<TEntity, TForeignKey>> foreignSelector) where TRelation : class?
        {
            var relation = ValidatePropertySelector<TRelation>(relationSelector);
            var foreignKey = ValidatePropertySelector<TForeignKey>(foreignSelector);

            _ignoredColumns.Add(relation.Name);

            Relations.Add(new EntityRelationDefinition(relation, false, foreignKey, foreignKey.Name, relation.PropertyType.Name, RelationType.OneToOne));

            return this;
        }

        public IEntityBuilder<TEntity> MapOneToOne<TRelation, TForeignKey>(Expression<Func<TEntity, TRelation>> relationSelector, Expression<Func<TRelation, TForeignKey>> foreignSelector) where TRelation : class?
        {
            var relation = ValidatePropertySelector<TRelation>(relationSelector);
            var foreignKey = ValidatePropertySelector<TRelation, TForeignKey>(foreignSelector);

            _ignoredColumns.Add(relation.Name);

            Relations.Add(new EntityRelationDefinition(relation, true, foreignKey, foreignKey.Name, relation.PropertyType.Name, RelationType.OneToOne));

            return this;
        }

        public IEntityBuilder<TEntity> MapOneToMany<TRelation, TForeignKey>(Expression<Func<TEntity, IEnumerable<TRelation>>> relationSelector, Expression<Func<TRelation, TForeignKey>> foreignSelector) where TRelation : class?
        {
            var relation = ValidatePropertySelector<IEnumerable<TRelation>>(relationSelector);
            var foreignKey = ValidatePropertySelector(foreignSelector);

            _ignoredColumns.Add(relation.Name);

            Relations.Add(new EntityRelationDefinition(relation, true, foreignKey, foreignKey.Name, relation.PropertyType.GetGenericArguments()[0].Name, RelationType.OneToMany));

            return this;
        }

        //public IEntityBuilder<TEntity> MapManyToOne<TRelation, TForeignKey>(Expression<Func<TRelation, IEnumerable<TEntity>>> relationSelector, Expression<Func<TEntity, TForeignKey>> foreignSelector) where TRelation : class?
        //{
        //    var relation = ValidatePropertySelector(relationSelector, true);
        //    var foreignKey = ValidatePropertySelector(foreignSelector, false);

        //    Relations.Add(new EntityRelationDefinition(relation, false, foreignKey, relation.PropertyType.Name, RelationType.OneToMany));

        //    return this;
        //}

        private PropertyInfo ValidatePropertySelector<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector)
        {
            if (propertySelector is null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var body = propertySelector.Body as MemberExpression;

            if (body is null)
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property.", nameof(propertySelector));
            }

            var property = body.Member as PropertyInfo;

            if (property is null)
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property.", nameof(propertySelector));
            }

            if (!property.CanWrite || !property.SetMethod.IsPublic)
            {
                throw new ArgumentException($"The provided property doesn't contain a setter or it isn't public.", nameof(propertySelector));
            }

            if (Type != property.ReflectedType &&
                !Type.IsSubclassOf(property.ReflectedType!))
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property on the entity itself.", nameof(propertySelector));
            }

            return property;
        }

        private PropertyInfo ValidatePropertySelector<TFrom, TTarget>(Expression<Func<TFrom, TTarget>> propertySelector)
        {
            if (propertySelector is null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var body = propertySelector.Body as MemberExpression;

            if (body is null)
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property.", nameof(propertySelector));
            }

            var property = body.Member as PropertyInfo;

            if (property is null)
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property.", nameof(propertySelector));
            }

            if (!property.CanWrite || !property.SetMethod.IsPublic)
            {
                throw new ArgumentException($"The provided property doesn't contain a setter or it isn't public.", nameof(propertySelector));
            }

            var type = typeof(TFrom);


            if (type != property.ReflectedType &&
                !type.IsSubclassOf(property.ReflectedType))
            {
                throw new ArgumentException($"The provided {nameof(propertySelector)} is not pointing to a property on the entity itself.", nameof(propertySelector));
            }

            return property;
        }

        internal override void IgnoreProperty(string propertyName)
            => _ignoredColumns.Add(propertyName);

        internal EntityColumnCollection<TEntity> Build()
        {
            var properties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties is null || properties.Length == 0)
            {
                throw new TypeArgumentException("The provided generic type argument doesn't contain any public properties with a getter and a setter.");
            }

            // ExpressionVariables

            var columns = new List<EntityColumn<TEntity>>();
            var nameToColumn = new Dictionary<string, EntityColumn<TEntity>>();
            var changeTrackingColumns = new Dictionary<int, EntityColumn<TEntity>>();
            PrimaryEntityColumn<TEntity>? primaryColumn = null;

            var insertWriterVariables = new List<ParameterExpression>();
            var insertWriterStatments = new List<Expression>();

            var constructorTypes = new Type[2];
            constructorTypes[0] = TypeCache.String;

            var indexParameter = Expression.Parameter(TypeCache.String, "index");
            var entityParameter = Expression.Parameter(Type, "entity");
            var valueParameter = Expression.Parameter(TypeCache.Object, "value");
            var stringBuilderParameter = Expression.Parameter(TypeCache.StringBuilder, "commandString");
            var npgsqlParameterCollectionParameter = Expression.Parameter(TypeCache.NpgsqlParameterCollection, "parameters");

            var stringConcatMethod = TypeCache.String.GetMethod("Concat", new[] { TypeCache.String, TypeCache.String });
            var stringBuilderAppend = TypeCache.StringBuilder.GetMethod("Append", new[] { TypeCache.String });
            var npgsqlParameterCollectionAdd = TypeCache.NpgsqlParameterCollection.GetMethod("Add", new Type[] { TypeCache.GenericNpgsqlParameter });

            // Important column specifications

            var columnIndex = 0;
            var regularColumnsOffset = 0;
            var propertyFlagValue = 1uL;

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (!property.CanWrite || !property.SetMethod!.IsPublic || _ignoredColumns.Contains(property.Name) || Attribute.IsDefined(property, TypeCache.NotMappedAttribute))
                {
                    continue;
                }

                var hasCustomDefinition = false;

                // ParameterValueRetriever

                constructorTypes[1] = property.PropertyType;

                var valueProperty = Expression.Property(entityParameter, property);

                var genericNpgsqlParameter = TypeCache.GenericNpgsqlParameter.MakeGenericType(property.PropertyType);

                var constructor = genericNpgsqlParameter.GetConstructor(constructorTypes)!;

                var parameterInstance = Expression.New(constructor, Expression.Add(Expression.Constant("@" + property.Name), indexParameter, stringConcatMethod), valueProperty);

                var parameterValueRetriever = Expression.Lambda<Func<TEntity, string, NpgsqlParameter>>(parameterInstance, entityParameter, indexParameter).Compile();

                Expression valueAssignment;
                var valueRetriever = TypeCache.NpgsqlDataReader!.GetMethod("GetFieldValue", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(property.PropertyType);

                if (property.PropertyType.IsClass || Nullable.GetUnderlyingType(property.PropertyType) is { })
                {
                    valueAssignment = Expression.Assign(valueProperty, Expression.TypeAs(valueParameter, property.PropertyType));
                }
                else
                {
                    valueAssignment = Expression.Assign(valueProperty, Expression.Convert(valueParameter, property.PropertyType));
                }

                var valueWriter = Expression.Lambda<Action<TEntity, object>>(valueAssignment, entityParameter, valueParameter).Compile();

                var isPrimaryColumn = false;

                // Handle custom columns

                if (ColumnDefinitions.TryGetValue(property.Name, out var definition))
                {
                    switch (definition)
                    {
                        case PrimaryColumnDefinition<TEntity> primaryDefintion:
                            primaryColumn = new PrimaryEntityColumn<TEntity>(property, definition.Name, propertyFlagValue, valueRetriever, valueWriter, parameterValueRetriever, primaryDefintion.IsServerSideGenerated);

                            columns.Insert(0, primaryColumn);

                            regularColumnsOffset++;

                            if (property.GetSetMethod().IsVirtual)
                            {
                                changeTrackingColumns.Add(columnIndex, primaryColumn);
                            }

                            nameToColumn.Add(definition.Name, primaryColumn);

                            hasCustomDefinition = true;
                            isPrimaryColumn = true;
                            break;
                    }
                }

                if (!hasCustomDefinition)
                {
                    string columnName;

                    if (definition is not null)
                    {
                        columnName = definition.Name;

                        var relation = Relations.FirstOrDefault(x => x.ForeignKeyProperty.Name == property.Name);

                        if (relation is not null)
                        {
                            relation.ForeignKeyColumnName = columnName;
                        }
                    }
                    else
                    {
                        columnName = property.Name;
                    }

                    var column = new EntityColumn<TEntity>(property, columnName, propertyFlagValue, valueRetriever, valueWriter, parameterValueRetriever);

                    columns.Add(column);

                    if (property.GetSetMethod().IsVirtual)
                    {
                        changeTrackingColumns.Add(columnIndex, column);
                    }

                    nameToColumn.Add(columnName, column);
                }

                if (!isPrimaryColumn)
                {
                    var parameterVariable = Expression.Variable(genericNpgsqlParameter, property.Name.ToLower());

                    insertWriterVariables.Add(parameterVariable);

                    insertWriterStatments.Add(Expression.Assign(parameterVariable, parameterInstance));
                    insertWriterStatments.Add(Expression.Call(stringBuilderParameter, stringBuilderAppend, Expression.Property(parameterVariable, "ParameterName")));
                    insertWriterStatments.Add(Expression.Call(npgsqlParameterCollectionParameter, npgsqlParameterCollectionAdd, parameterVariable));
                    insertWriterStatments.Add(Expression.Call(stringBuilderParameter, stringBuilderAppend, Expression.Constant(", ")));
                }

                columnIndex++;
                propertyFlagValue <<= 1;
            }

            if (primaryColumn is null)
            {
                throw new InvalidOperationException("The EntityBuilder didn't configure the primary key nor is any property named 'Id'.");
            }

            if (TableName is null)
            {
                TableName = Type.Name + "s";
            }

            if (changeTrackingColumns.Count != 0)
            {
                ChangeTrackerFactory = new ChangeTrackerFactory<TEntity>(Type);

                ChangeTrackerFactory.GenerateEntityProxy(changeTrackingColumns);
            }

            insertWriterStatments.RemoveAt(insertWriterStatments.Count - 1);

            InsertWriter = Expression.Lambda<Action<TEntity, StringBuilder, string, NpgsqlParameterCollection>>(Expression.Block(insertWriterVariables, insertWriterStatments), entityParameter, stringBuilderParameter, indexParameter, npgsqlParameterCollectionParameter).Compile();

            return new EntityColumnCollection<TEntity>(columns.ToArray(), nameToColumn, regularColumnsOffset);
        }
    }

    internal abstract class EntityBuilder
    {
        internal List<EntityRelationDefinition> Relations { get; }
        internal abstract Type Type { get; }

        protected EntityBuilder()
        {
            Relations = new List<EntityRelationDefinition>();
        }

        internal abstract void IgnoreProperty(string propertyName);
    }

    public interface IEntityBuilder<TEntity> where TEntity : class
    {
        IEntityBuilder<TEntity> MapToTable(string tableName);

        IEntityBuilder<TEntity> MapColumn<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector, string columnName);

        IEntityBuilder<TEntity> Ignore<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector);

        IEntityBuilder<TEntity> MapId<TTarget>(Expression<Func<TEntity, TTarget>> propertySelector, DatabaseGeneratedOption option);

        IEntityBuilder<TEntity> MapOneToOne<TRelation, TForeignKey>(Expression<Func<TEntity, TRelation>> relationSelector, Expression<Func<TEntity, TForeignKey>> foreignSelector) where TRelation : class?;

        IEntityBuilder<TEntity> MapOneToOne<TRelation, TForeignKey>(Expression<Func<TEntity, TRelation>> relationSelector, Expression<Func<TRelation, TForeignKey>> foreignSelector) where TRelation : class?;

        IEntityBuilder<TEntity> MapOneToMany<TRelation, TForeignKey>(Expression<Func<TEntity, IEnumerable<TRelation>>> relationSelector, Expression<Func<TRelation, TForeignKey>> foreignSelector) where TRelation : class?;

        //IEntityBuilder<TEntity> MapManyToOne<TRelation, TForeignKey>(Expression<Func<TRelation, IList<TEntity>>> relationSelector, Expression<Func<TEntity, TForeignKey>> foreignSelector) where TRelation : class?;
    }
}