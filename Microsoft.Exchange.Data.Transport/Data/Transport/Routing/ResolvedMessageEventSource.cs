using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public abstract class ResolvedMessageEventSource : QueuedMessageEventSource
	{
		internal ResolvedMessageEventSource()
		{
		}

		public abstract RoutingOverride GetRoutingOverride(EnvelopeRecipient recipient);

		public abstract void SetRoutingOverride(EnvelopeRecipient recipient, RoutingOverride routingOverride);

		public abstract string GetTlsDomain(EnvelopeRecipient recipient);

		public abstract void SetTlsDomain(EnvelopeRecipient recipient, string domain);

		internal abstract void SetRoutingOverride(EnvelopeRecipient recipient, RoutingOverride routingOverride, string overrideSource);

		internal abstract RequiredTlsAuthLevel? GetTlsAuthLevel(EnvelopeRecipient recipient);

		internal abstract void SetTlsAuthLevel(EnvelopeRecipient recipient, RequiredTlsAuthLevel? tlsAuthLevel);
	}
}
