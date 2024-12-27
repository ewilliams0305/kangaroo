using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Kangaroo.UI.Database;

/// <summary>
/// Creates a new connection to a database
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Provides access to the DB's connection string
    /// </summary>
    string ConnectionString { get; }
    
    /// <summary>
    /// Creates an asynchronous connection to a provided database
    /// </summary>
    /// <returns>Database Connection</returns>
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}