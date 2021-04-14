using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IConnectionProvider
	{
		Database Database { get; }

		Connection GetConnection();
	}
}
