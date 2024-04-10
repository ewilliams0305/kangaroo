using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Kangaroo.UI.Services.Database;

public sealed class SqliteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connection;

    public SqliteDbConnectionFactory(string connection)
    {
        _connection = connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token)
    {
        var connection = new SqliteConnection(_connection);
        await connection.OpenAsync(token);
        return connection;
    }
}