#pragma warning disable IDE0290

using System;
using System.Linq;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;

namespace Kangaroo.UI.Database;
public sealed class SqliteDbInitializer : IDbInitializer
{
    /// <summary>
    /// Locates all the configuration mappers in the assembly and executes the configure method.
    /// </summary>
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

    private readonly ILogger<SqliteDbInitializer> _logger;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    /// <summary>
    /// Creates a new instance of the SQLITE initializer
    /// </summary>
    /// <param name="dbConnectionFactory">DB Connection factory</param>
    public SqliteDbInitializer(ILogger<SqliteDbInitializer> logger, IDbConnectionFactory dbConnectionFactory)
    {
        _logger = logger;
        _dbConnectionFactory = dbConnectionFactory;
    }


    /// <summary>
    /// Adds and removes all tables from the database required for the application to initialize correctly. 
    /// </summary>
    /// <returns>awaitable task</returns>
    public Task InitializeAsync()
    {
        _logger.LogInformation("Checking Database for Required Migrations...");
        
        var upgrader = DeployChanges.To
            .SQLiteDatabase(_dbConnectionFactory.ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(SqliteDbInitializer).Assembly, (resource) => resource.EndsWith(".sql"))
            .LogToConsole()
            .Build();

        if (upgrader is not null && upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();
            _logger.LogInformation("Migrated Database {status}", result.Successful ? "Successful" : "Failed");
            return Task.CompletedTask;
        }
        
        _logger.LogInformation("No migrations are required at this time.");
        return Task.CompletedTask;
    }
}