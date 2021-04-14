using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlCommand : IDisposable
	{
		string CommandText { get; set; }

		int CommandTimeout { get; set; }

		CommandType CommandType { get; set; }

		ISqlConnection Connection { get; set; }

		SqlParameterCollection Parameters { get; }

		ISqlTransaction Transaction { get; set; }

		int ExecuteNonQuery();

		ISqlDataReader ExecuteReader();

		ISqlDataReader ExecuteReader(CommandBehavior behavior);

		object ExecuteScalar();
	}
}
