using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchProbeFailureException : LocalizedException
	{
		public SearchProbeFailureException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public SearchProbeFailureException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}
	}
}
