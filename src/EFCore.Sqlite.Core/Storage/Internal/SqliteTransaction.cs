// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal
{
    public class SqliteTransaction : RelationalTransaction
    {
        private readonly DbTransaction _dbTransaction;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SqliteTransaction(
            [NotNull] IRelationalConnection connection,
            [NotNull] DbTransaction transaction,
            Guid transactionId,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
            bool transactionOwned)
            : base(connection, transaction, transactionId, logger, transactionOwned)
            => _dbTransaction = transaction;

        /// <inheritdoc />
        public override void Save(string savepointName)
        {
            using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "SAVEPOINT " + savepointName;
            command.ExecuteNonQuery();
        }

        /// <inheritdoc />
        public override async Task SaveAsync(string savepointName, CancellationToken cancellationToken = default)
        {
            await using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "SAVEPOINT " + savepointName;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void Rollback(string savepointName)
        {
            using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "ROLLBACK TO " + savepointName;
            command.ExecuteNonQuery();
        }

        /// <inheritdoc />
        public override async Task RollbackAsync(string savepointName, CancellationToken cancellationToken = default)
        {
            await using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "ROLLBACK TO " + savepointName;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void Release(string savepointName)
        {
            using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "RELEASE " + savepointName;
            command.ExecuteNonQuery();
        }

        /// <inheritdoc />
        public override async Task ReleaseAsync(string savepointName, CancellationToken cancellationToken = default)
        {
            await using var command = Connection.DbConnection.CreateCommand();
            command.Transaction = _dbTransaction;
            command.CommandText = "RELEASE " + savepointName;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override bool AreSavepointsSupported => true;
    }
}
