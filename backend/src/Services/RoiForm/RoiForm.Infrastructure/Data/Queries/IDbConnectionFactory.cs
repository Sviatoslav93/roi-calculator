using System.Data.Common;
using Npgsql;

namespace RoiForm.Infrastructure.Data.Queries;

public interface IDbConnectionFactory
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}

public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory, IDisposable
{
    private readonly NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(connectionString);

    public async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        => await _dataSource.OpenConnectionAsync(cancellationToken);

    public void Dispose() => _dataSource.Dispose();
}
