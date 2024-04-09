using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Kangaroo.UI.Services.Database;

/// <summary>
/// Creates a new connection to a database
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates an asynchronous connection to a provided database
    /// </summary>
    /// <returns>Database Connection</returns>
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}