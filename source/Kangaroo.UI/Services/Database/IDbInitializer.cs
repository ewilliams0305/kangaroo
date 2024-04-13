using System.Threading.Tasks;

namespace Kangaroo.UI.Services.Database;

/// <summary>
/// Initialized the DB scheme and ensures all tables are created when the system starts.
/// </summary>
public interface IDbInitializer
{
    /// <summary>
    /// Adds and removes all tables from the database required for the application to initialize correctly. 
    /// </summary>
    /// <returns>awaitable task</returns>
    Task InitializeAsync();
}