﻿using Npgsql;
using System;
using System.Reflection;

namespace Venflow.Modeling
{
    internal class PrimaryEntityColumn<TEntity> : EntityColumn<TEntity> where TEntity : class
    {
        internal bool IsServerSideGenerated { get; }

        internal PrimaryEntityColumn(PropertyInfo propertyInfo, string columnName, MethodInfo dbValueRetriever, Action<TEntity, object> valueWriter, Func<TEntity, string, NpgsqlParameter> valueRetriever, bool isServerSideGenerated) : base(propertyInfo, columnName, dbValueRetriever, valueWriter, valueRetriever)
        {
            IsServerSideGenerated = isServerSideGenerated;
        }
    }
}