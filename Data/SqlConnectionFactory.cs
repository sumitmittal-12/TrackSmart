using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TrackSmart.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        // Fails fast on startup if the configuration is missing, preventing hidden runtime errors.
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection string is missing from appsettings.json.");
    }

    public IDbConnection CreateConnection()
    {
        // Returns a new, closed connection. Dapper will open and close it automatically.
        return new SqlConnection(_connectionString);
    }
}