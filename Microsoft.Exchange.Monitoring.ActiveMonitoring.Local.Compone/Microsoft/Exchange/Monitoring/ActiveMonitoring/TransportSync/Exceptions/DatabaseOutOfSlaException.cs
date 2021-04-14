using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions
{
	[Serializable]
	public class DatabaseOutOfSlaException : TransportSyncException
	{
		public DatabaseOutOfSlaException(LocalizedString message) : base(message)
		{
		}
	}
}
