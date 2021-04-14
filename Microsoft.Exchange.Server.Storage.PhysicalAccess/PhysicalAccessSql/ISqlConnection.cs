using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlConnection : IDisposable
	{
		event SqlInfoMessageEventHandler InfoMessage;

		ConnectionState State { get; }

		SqlConnection WrappedConnection { get; set; }

		ISqlTransaction BeginTransaction(IsolationLevel iso);

		void ClearPool();

		void Close();

		ISqlCommand CreateCommand();

		void Open();
	}
}
