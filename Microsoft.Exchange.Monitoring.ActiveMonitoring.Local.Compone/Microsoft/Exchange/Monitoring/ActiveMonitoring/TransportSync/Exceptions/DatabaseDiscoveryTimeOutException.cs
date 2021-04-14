using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions
{
	[Serializable]
	public class DatabaseDiscoveryTimeOutException : TransportSyncException
	{
		public DatabaseDiscoveryTimeOutException(LocalizedString message) : base(message)
		{
		}
	}
}
