using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Kangaroo.UI.Database;

public sealed class ComplianceRepository : IEntityRepository<ComplianceRun, int>
{
    private readonly ILogger<ComplianceRepository> _logger;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly TimeProvider _timeProvider;
    
    public ComplianceRepository(ILogger<ComplianceRepository> logger, IDbConnectionFactory connectionFactory, TimeProvider timeProvider)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ComplianceRun>> GetAsync(CancellationToken token = default)
    {
        return [];
    }

    /// <inheritdoc />
    public async Task<ComplianceRun?> GetByIdAsync(int id, CancellationToken token = default)
    {
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> CreateAsync(ComplianceRun entity, CancellationToken token = default)
    {
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(ComplianceRun entity, CancellationToken token = default)
    {
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(
            @"delete FROM ComplianceRun WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsById(int id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            @"SELECT count(1) FROM ComplianceRun WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));
    }
}
