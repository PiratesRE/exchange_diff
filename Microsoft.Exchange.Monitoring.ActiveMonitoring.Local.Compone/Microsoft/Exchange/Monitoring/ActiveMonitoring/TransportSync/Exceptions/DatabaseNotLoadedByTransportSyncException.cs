using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions
{
	[Serializable]
	public class DatabaseNotLoadedByTransportSyncException : TransportSyncException
	{
		public DatabaseNotLoadedByTransportSyncException(LocalizedString message) : base(message)
		{
		}
	}
}
