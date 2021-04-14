using System;
using System.Data.SqlClient;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlTransaction : IDisposable
	{
		SqlTransaction WrappedTransaction { get; set; }

		void Commit();

		void Rollback();
	}
}
