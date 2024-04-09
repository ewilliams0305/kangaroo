using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kangaroo.UI.Services.Database;
public sealed class SqliteDbInitializer : IDbInitializer
{
    static SqliteDbInitializer()
    {
        var types = typeof(SqliteDbInitializer).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IConfigureRepository)));

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            if (instance is IConfigureRepository configurator)
            {
                configurator.Configure();
            }
        }
    }

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqliteDbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        try
        {
            await connection.ExecuteAsync(CreateRecentScansTable);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private const string CreateRecentScansTable = @"CREATE TABLE IF NOT EXISTS RecentScans (
                                                                  Id UUID NOT NULL,
                                                                  ScanMode INTEGER NOT NULL,
                                                                  StartAddress TEXT,
                                                                  EndAddress TEXT,
                                                                  SubnetMask TEXT,
                                                                  SpecifiedAddresses TEXT,
                                                                  Adapter TEXT,
                                                                  CreatedDateTime TEXT NOT NULL
                                                                  )";
}