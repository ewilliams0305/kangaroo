﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Kangaroo.UI.Database;

public sealed class RecentScansRepository : IEntityRepository<RecentScan, Guid>
{
    private readonly ILogger<RecentScansRepository> _logger;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly TimeProvider _timeProvider;

    private const string RecentScansQuery = "Select * FROM RecentScans GROUP BY CreatedDateTime";
    
    private const string RecentScanQuery = """
                                           select Id id, 
                                             ScanMode scanMode, 
                                             StartAddress startAddress,
                                             EndAddress endAddress,
                                             SubnetMask subnetMask,
                                             SpecifiedAddresses specifiedAddresses,
                                             Adapter adapter,
                                             CreatedDateTime createdDateTime,
                                             ElapsedTime elapsedTime, 
                                             OnlineDevices onlineDevices
                                             FROM RecentScans WHERE Id = @Id
                                           """;
    public RecentScansRepository(ILogger<RecentScansRepository> logger, IDbConnectionFactory connectionFactory, TimeProvider timeProvider)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecentScan>> GetAsync(CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var scans = await connection.QueryAsync<RecentScan>(new CommandDefinition(
            commandText: RecentScansQuery,
            cancellationToken: token));

        return scans;
    }

    /// <inheritdoc />
    public async Task<RecentScan?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var user = await connection.QuerySingleOrDefaultAsync<RecentScan>(new CommandDefinition(
            commandText: RecentScanQuery,
            parameters: new { Id = id.ToString() },
            cancellationToken: token));

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> CreateAsync(RecentScan entity, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        try
        {
            var parameters = RecentScansConfiguration.CreateRecentScansParameters(entity, _timeProvider);
            
            if (entity.Result is not null)
            {
                var scanResultId = Guid.NewGuid();
                
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText: """
                                     INSERT INTO ScanResults (Id, ScanResult, ScanResultDiscriminator)
                                     VALUES (@Id, @ScanResult, @ScanResultDiscriminator);
                                     """, 
                        parameters: new
                        {
                            Id = scanResultId,
                            ScanResult = JsonSerializer.SerializeToUtf8Bytes(entity.Result.ToResults()),
                            ScanResultDiscriminator = typeof(RecentScanResult).FullName,
                        },
                        cancellationToken: token,
                        transaction: transaction
                    )
                );
                
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText: """
                                     INSERT INTO ScanResultMappings (Id, ScanResultId, OwnerId, OwnerType)
                                     VALUES (@Id, @ScanResultId, @OwnerId, @OwnerType)
                                     """,
                        parameters: new
                        {
                            Id = Guid.NewGuid(),
                            ScanResultId = scanResultId, 
                            OwnerId = entity.Id, 
                            OwnerType = nameof(RecentScan)
                        },
                        cancellationToken: token,
                        transaction: transaction
                    )
                );
            }

            var result = await connection.ExecuteAsync(new CommandDefinition(
                commandText: """
                             INSERT INTO RecentScans ( Id, ScanMode, StartAddress, EndAddress, SubnetMask, SpecifiedAddresses, Adapter, CreatedDateTime, ElapsedTime, OnlineDevices)
                             VALUES (@Id, @ScanMode, @StartAddress, @EndAddress, @SubnetMask, @SpecifiedAddresses, @Adapter, @CreatedDateTime, @ElapsedTime, @OnlineDevices)
                             """,
                parameters: parameters,
                transaction: transaction,
                cancellationToken: token));
            
            transaction.Commit();
            
            return result > 0;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            
            _logger.LogError(e, "Failed to create Recent Scan record");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(RecentScan entity, CancellationToken token = default)
    {
        var parameters = RecentScansConfiguration.CreateRecentScansParameters(entity, _timeProvider);

        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(
            commandText: @"update RecentScans ( Id, ScanMode, StartAddress, EndAddress, SubnetMask, SpecifiedAddresses, Adapter, CreatedDateTime, ElapsedTime, OnlineDevices)
                           values (@Id, @ScanMode, @StartAddress, @EndAddress, @SubnetMask, @SpecifiedAddresses, @Adapter, @CreatedDateTime, @ElapsedTime, @OnlineDevices)",
            parameters: parameters,
            cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(
            @"delete FROM RecentScans WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            @"SELECT count(1) FROM RecentScans WHERE Id = @Id",
            new { Id = id.ToString() },
            cancellationToken: token));
    }
}
