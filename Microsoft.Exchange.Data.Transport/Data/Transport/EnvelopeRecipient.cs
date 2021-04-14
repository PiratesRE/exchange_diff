using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class EnvelopeRecipient
	{
		internal EnvelopeRecipient()
		{
		}

		public abstract RoutingAddress Address { get; set; }

		[Obsolete("Use ResolvedMessageEventSource.GetRoutingOverride() instead")]
		public abstract RoutingDomain RoutingOverride { get; }

		public abstract string OriginalRecipient { get; set; }

		public abstract DsnTypeRequested RequestedReports { get; set; }

		public abstract IDictionary<string, object> Properties { get; }

		public abstract DeliveryMethod OutboundDeliveryMethod { get; }

		public abstract RecipientCategory RecipientCategory { get; }

		[Obsolete("Use ResolvedMessageEventSource.SetRoutingOverride() instead")]
		public abstract void SetRoutingOverride(RoutingDomain routingDomain);

		internal virtual bool IsPublicFolderRecipient()
		{
			return false;
		}
	}
}
