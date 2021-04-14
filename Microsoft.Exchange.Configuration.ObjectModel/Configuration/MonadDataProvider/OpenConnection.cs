using System;
using System.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenConnection : IDisposable
	{
		public OpenConnection(IDbConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (connection.State == ConnectionState.Closed)
			{
				this.connection = connection;
				this.connection.Open();
			}
		}

		public void Dispose()
		{
			if (this.connection != null)
			{
				this.connection.Close();
			}
		}

		private IDbConnection connection;
	}
}
