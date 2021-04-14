using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public class AutodiscoverOabUrlNotFound : Exception
	{
		public AutodiscoverOabUrlNotFound(string user) : base(string.Format("Autodiscover Service failed to return the ExternalOABUrl for user {0}", user))
		{
		}
	}
}
