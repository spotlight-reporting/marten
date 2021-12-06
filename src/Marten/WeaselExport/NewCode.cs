using System.Threading.Tasks;
using Npgsql;
using Weasel.Core;

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

    public static class ConnectionSourceExtensions
    {
        public static void RunSql(this IConnectionSource tenant, string sql)
        {
            using var conn = tenant.CreateConnection();
            conn.Open();

            try
            {
                conn.CreateCommand(sql).ExecuteNonQuery();
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
                await conn.CreateCommand(sql).ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            finally
            {
                await conn.CloseAsync().ConfigureAwait(false);
                conn.Dispose();
            }
        }
    }
}
