using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions
{
	[Serializable]
	public abstract class TransportSyncException : LocalizedException
	{
		public TransportSyncException(LocalizedString message) : base(message)
		{
		}
	}
}
