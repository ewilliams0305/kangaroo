using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Kangaroo.UI.Database;

public sealed class SqliteDbConnectionFactory : IDbConnectionFactory
{
    public string ConnectionString { get; }

    public SqliteDbConnectionFactory(string connection)
    {
        ConnectionString = connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token)
    {
        var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}