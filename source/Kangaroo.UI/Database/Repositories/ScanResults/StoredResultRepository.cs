using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Kangaroo.UI.Database;

public sealed class StoredResultsRepository : IEntityRepository<StoredResults, int>
{
    private readonly ILogger<StoredResultsRepository> _logger;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly TimeProvider _timeProvider;
    
    public StoredResultsRepository(ILogger<StoredResultsRepository> logger, IDbConnectionFactory connectionFactory, TimeProvider timeProvider)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StoredResults>> GetAsync(CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var results = await connection.QueryAsync<StoredResults>(new CommandDefinition(
            commandText: "SELECT * FROM ScanResults",
            cancellationToken: token));

        return results;
    }

    /// <inheritdoc />
    public async Task<StoredResults?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var result = await connection.QuerySingleOrDefaultAsync<StoredResults>(new CommandDefinition(
            commandText: "SELECT * FROM ScanResults WHERE Id = @id",
            parameters: new { Id = id.ToString() },
            cancellationToken: token));

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> CreateAsync(StoredResults entity, CancellationToken token = default)
    {
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(StoredResults entity, CancellationToken token = default)
    {
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition(
            @"DELETE FROM ScanResults WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));

        return result > 0;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsById(int id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            @"SELECT count(1) FROM ScanResults WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));
    }
}
