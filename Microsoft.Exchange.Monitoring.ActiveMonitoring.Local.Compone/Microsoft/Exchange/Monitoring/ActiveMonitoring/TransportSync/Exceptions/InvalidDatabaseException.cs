using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions
{
	[Serializable]
	public class InvalidDatabaseException : TransportSyncException
	{
		public InvalidDatabaseException(LocalizedString message) : base(message)
		{
		}
	}
}
