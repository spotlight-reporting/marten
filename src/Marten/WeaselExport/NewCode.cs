using System.Threading.Tasks;
using Npgsql;
using Weasel.Postgresql;
using CommandExtensions = Weasel.Core.CommandExtensions;

namespace Marten.WeaselExport
{
    public interface IConnectionSource
    {
        /// <summary>
        ///     Fetch a connection to the tenant database
        /// </summary>
        /// <returns></returns>
        NpgsqlConnection CreateConnection();
    }

    public interface ISchemaObjectGroup
    {
        /// <summary>
        /// All the schema objects in this feature
        /// </summary>
        ISchemaObject[] Objects { get; }

        /// <summary>
        /// Really just the filename when the SQL is exported
        /// </summary>
        string Identifier { get; }
    }


    public static class ConnectionSourceExtensions
    {
        public static void RunSql(this IConnectionSource tenant, string sql)
        {
            using var conn = tenant.CreateConnection();
            conn.Open();

            try
            {
                CommandExtensions.CreateCommand(conn, sql).ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public static async Task RunSqlAsync(this IConnectionSource tenant, string sql)
        {
            await using var conn = tenant.CreateConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            try
            {
                await CommandExtensions.CreateCommand(conn, sql).ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            finally
            {
                await conn.CloseAsync().ConfigureAwait(false);
                conn.Dispose();
            }
        }
    }
}
