using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum ConnectionPoolType
	{
		DCPool,
		GCPool,
		ConfigDCPool,
		ConfigDCNotifyPool,
		UserDCPool,
		UserGCPool,
		Count
	}
}
