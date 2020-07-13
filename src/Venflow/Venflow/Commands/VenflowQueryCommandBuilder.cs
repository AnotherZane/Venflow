﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Venflow.Enums;
using Venflow.Modeling;

namespace Venflow.Commands
{
    internal class VenflowQueryCommandBuilder<TEntity, TReturn> : IQueryCommandBuilder<TEntity, TReturn> where TEntity : class where TReturn : class
    {
        internal JoinBuilderValues? JoinBuilderValues { get; set; }

        private bool _trackChanges;

        private readonly bool _generateSQL;
        private readonly bool _disposeCommand;
        private readonly ulong _count;
        private readonly StringBuilder _commandString;
        private readonly NpgsqlCommand _command;
        private readonly Database _database;
        private readonly Entity<TEntity> _entityConfiguration;

        internal VenflowQueryCommandBuilder(Database database, Entity<TEntity> entityConfiguration, NpgsqlCommand command, bool generateSQL, bool disposeCommand)
        {
            _database = database;
            _entityConfiguration = entityConfiguration;
            _generateSQL = generateSQL;
            _command = command;
            _disposeCommand = disposeCommand;
            _commandString = new StringBuilder();
        }

        internal VenflowQueryCommandBuilder(Database database, Entity<TEntity> entityConfiguration, NpgsqlCommand command, ulong count, bool disposeCommand) : this(database, entityConfiguration, command, true, disposeCommand)
        {
            _count = count;
        }

        internal VenflowQueryCommandBuilder(Database database, Entity<TEntity> entityConfiguration, NpgsqlCommand command, string sql, bool disposeCommand) : this(database, entityConfiguration, command, false, disposeCommand)
        {
            _commandString.Append(sql);
        }

        internal VenflowQueryCommandBuilder(Database database, Entity<TEntity> entityConfiguration, NpgsqlCommand command, string sql, IList<NpgsqlParameter> parameters, bool disposeCommand) : this(database, entityConfiguration, command, sql, disposeCommand)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                _command.Parameters.Add(parameters[i]);
            }
        }

        JoinBuilder<TEntity, TToEntity, TReturn> IQueryCommandBuilder<TEntity, TReturn>.JoinWith<TToEntity>(Expression<Func<TEntity, TToEntity>> propertySelector, JoinBehaviour joinBehaviour)
        {
            var builder = new JoinBuilder<TEntity, TToEntity, TReturn>(_entityConfiguration, this, _generateSQL);

            return builder.JoinWith(propertySelector, joinBehaviour);
        }

        JoinBuilder<TEntity, TToEntity, TReturn> IQueryCommandBuilder<TEntity, TReturn>.JoinWith<TToEntity>(Expression<Func<TEntity, List<TToEntity>>> propertySelector, JoinBehaviour joinBehaviour)
        {
            var builder = new JoinBuilder<TEntity, TToEntity, TReturn>(_entityConfiguration, this, _generateSQL);

            return builder.JoinWith(propertySelector, joinBehaviour);
        }

        public IQueryCommandBuilder<TEntity, TReturn> TrackChanges(bool trackChanges = true)
        {
            _trackChanges = trackChanges;

            return this;
        }

        public IQueryCommand<TEntity, TReturn> Build()
        {
            if (_generateSQL)
            {
                if (JoinBuilderValues is null)
                {
                    AppendBaseQuery(_commandString, _count);
                    _commandString.Append(';');
                }
                else
                {
                    BuildRelationQuery(_count);
                }
            }

            _command.CommandText = _commandString.ToString();

            return new VenflowQueryCommand<TEntity, TReturn>(_database, _entityConfiguration, _command, JoinBuilderValues, _trackChanges, _disposeCommand);
        }

        private void BuildRelationQuery(ulong count)
        {
            _commandString.Append("SELECT ");

            _commandString.Append(_entityConfiguration.ExplicitColumnListString);

            var subQuery = new StringBuilder();

            subQuery.Append(" FROM (");

            AppendBaseQuery(subQuery, count);

            subQuery.Append(") AS ");

            subQuery.Append(_entityConfiguration.RawTableName);

            JoinBuilderValues!.AppendColumnNamesAndJoins(_commandString, subQuery);

            _commandString.Append(subQuery);

            _commandString.Append(';');
        }

        private void AppendBaseQuery(StringBuilder sb, ulong count)
        {
            sb.Append("SELECT ");

            sb.Append(_entityConfiguration.ColumnListString);

            sb.Append(" FROM ");

            sb.Append(_entityConfiguration.TableName);

            if (count > 0)
            {
                sb.Append(" LIMIT ");
                sb.Append(count);
            }
        }
    }
}