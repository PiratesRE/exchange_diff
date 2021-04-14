using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutodiscoverEndpoint
	{
		internal AutodiscoverEndpoint()
		{
			this.DiscoveryDateTime = new DateTime?(DateTime.MinValue);
		}

		internal string Url { get; set; }

		internal DateTime? DiscoveryDateTime { get; set; }

		internal DateTime? ExplicitExpiration { get; set; }

		internal bool IsPotentiallyReusable()
		{
			return this.ExplicitExpirationIsUsable() || this.DiscoveryExpirationIsUsable();
		}

		private bool ExplicitExpirationIsUsable()
		{
			return (DateTime.UtcNow - this.ExplicitExpiration.GetValueOrDefault()).TotalHours < 24.0;
		}

		private bool DiscoveryExpirationIsUsable()
		{
			TimeSpan timeSpan = DateTime.UtcNow - this.DiscoveryDateTime.GetValueOrDefault();
			return !string.IsNullOrWhiteSpace(this.Url) && timeSpan.TotalHours < 24.0;
		}

		private const double MaxUsableHours = 24.0;
	}
}
