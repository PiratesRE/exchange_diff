using System;
using System.Data.SqlClient;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlClientFactory
	{
		ISqlCommand CreateSqlCommand();

		ISqlCommand CreateSqlCommand(SqlCommand command);

		ISqlCommand CreateSqlCommand(string commandText, ISqlConnection connection, ISqlTransaction transaction);

		ISqlConnection CreateSqlConnection(SqlConnection connection);

		ISqlConnection CreateSqlConnection(string connectionString);

		ISqlDataReader CreateSqlDataReader(SqlDataReader reader);

		ISqlTransaction CreateSqlTransaction(SqlTransaction transaction);
	}
}
