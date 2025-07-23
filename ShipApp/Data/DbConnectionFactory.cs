using System.Data;
using Npgsql;

namespace ShipApp.Data
{
    public static class DbConnectionFactory
    {
        public static NpgsqlConnection CreateConnection()
        {
            string connectionString = ConfigurationHelper.GetConnectionString("Postgres");
            return new NpgsqlConnection(connectionString);
        }
    }
}
