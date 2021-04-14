using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class AgentSubmittedMessageSource : SubmittedMessageEventSource
	{
		public override void Defer(TimeSpan waitTime)
		{
			this.baseProxy.Defer(waitTime, null);
		}

		public override void Defer(TimeSpan waitTime, string trackingContext)
		{
			this.baseProxy.Defer(waitTime, trackingContext);
		}

		public override void Defer(TimeSpan waitTime, SmtpResponse deferReason)
		{
			this.baseProxy.Defer(waitTime, deferReason, null);
		}

		public override void Delete()
		{
			this.baseProxy.Delete(null);
		}

		public override void Delete(string trackingContext)
		{
			this.baseProxy.Delete(trackingContext);
		}

		public override void Fork(IList<EnvelopeRecipient> recipients)
		{
			this.baseProxy.Fork(recipients);
		}

		public override void ExpandRecipients(IList<RecipientExpansionInfo> recipientExpansionInfo)
		{
			this.baseProxy.ExpandRecipients(recipientExpansionInfo);
		}

		internal override void TrackAgentInfo(string agentName, string groupName, List<KeyValuePair<string, string>> data)
		{
			this.baseProxy.TrackAgentInfo(agentName, groupName, data);
		}

		internal override void SetRiskLevel(RiskLevel level)
		{
			this.baseProxy.SetRiskLevel(level);
		}

		internal override RiskLevel GetRiskLevel()
		{
			return this.baseProxy.GetRiskLevel();
		}

		internal override void SetOutboundIPPool(EnvelopeRecipient recipient, int pool)
		{
			this.baseProxy.SetOutboundIPPool(recipient, pool);
		}

		internal override int GetOutboundIPPool(EnvelopeRecipient recipient)
		{
			return this.baseProxy.GetOutboundIPPool(recipient);
		}

		internal override void SetComponentCost(string componentName, long cost)
		{
			this.baseProxy.SetComponentCost(componentName, cost);
		}

		internal override long GetComponentCost(string componentName)
		{
			return this.baseProxy.GetComponentCost(componentName);
		}

		internal void Initialize(InternalSubmittedMessageSource baseProxy)
		{
			this.baseProxy = baseProxy;
		}

		private InternalSubmittedMessageSource baseProxy;
	}
}
