using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Kangaroo.UI.Database;

namespace Kangaroo.UI.Services;

public class ComplianceService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly RecentScansRepository _recentScansRepository;

    public ComplianceService(IDbConnectionFactory dbConnectionFactory, RecentScansRepository recentScansRepository)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _recentScansRepository = recentScansRepository;
    }


    public async Task<IEnumerable<RecentScan>> GetRecentScans(CancellationToken cancellationToken =  default)
    {
        return await _recentScansRepository.GetAsync(cancellationToken);
    }

    public async Task<ScanResults?> SelectRecentScanResult(RecentScan recentScan,CancellationToken cancellationToken =  default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var map = await connection.QueryFirstOrDefaultAsync(
            """
            SELECT sr.* FROM ScanResults sr 
            INNER JOIN ScanResultMappings sm ON sr.Id = sm.ScanResultId
            WHERE OwnerId = @RecentScanId
            """, 
            new {RecentScanId = recentScan.Id});

        if (map is null)
        {
            return null;
        }
        
        var results = JsonSerializer.Deserialize<ScanResults>(map.ScanResults as byte[]);
        
        return results;
    }
}