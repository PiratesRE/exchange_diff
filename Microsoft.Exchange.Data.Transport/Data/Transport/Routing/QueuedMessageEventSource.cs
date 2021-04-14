using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public abstract class QueuedMessageEventSource
	{
		internal QueuedMessageEventSource()
		{
		}

		public abstract void Defer(TimeSpan waitTime);

		public abstract void Defer(TimeSpan waitTime, string trackingContext);

		public abstract void Defer(TimeSpan waitTime, SmtpResponse deferReason);

		public abstract void Delete();

		public abstract void Delete(string trackingContext);

		public abstract void Fork(IList<EnvelopeRecipient> recipients);

		public abstract void ExpandRecipients(IList<RecipientExpansionInfo> recipientExpansionInfo);

		internal abstract void TrackAgentInfo(string agentName, string groupName, List<KeyValuePair<string, string>> data);

		internal abstract void SetRiskLevel(RiskLevel level);

		internal abstract RiskLevel GetRiskLevel();

		internal abstract void SetOutboundIPPool(EnvelopeRecipient recipient, int offset);

		internal abstract int GetOutboundIPPool(EnvelopeRecipient recipient);

		internal abstract void SetComponentCost(string componentName, long cost);

		internal abstract long GetComponentCost(string componentName);
	}
}
