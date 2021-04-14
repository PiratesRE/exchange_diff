using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyResubmitHelper : ResubmitHelper
	{
		public ShadowRedundancyResubmitHelper(ResubmitReason resubmitReason, NextHopSolutionKey solutionKey) : base(resubmitReason, MessageTrackingSource.REDUNDANCY, ShadowRedundancyResubmitHelper.GetShadowResubmitSourceContext(solutionKey, resubmitReason), solutionKey, ExTraceGlobals.ShadowRedundancyTracer)
		{
			if (string.IsNullOrEmpty(solutionKey.NextHopDomain))
			{
				throw new ArgumentException("solutionKey.NextHopDomain cannot be empty", "solutionKey");
			}
			this.solutionKey = solutionKey;
		}

		private string PrimaryServerFqdn
		{
			get
			{
				return this.solutionKey.NextHopDomain;
			}
		}

		protected override string GetComponentNameForReceivedHeader()
		{
			return "ShadowRedundancy";
		}

		protected override TransportMailItem CloneWithoutRecipients(TransportMailItem mailItem)
		{
			TransportMailItem transportMailItem = base.CloneWithoutRecipients(mailItem);
			transportMailItem.BumpExpirationTime();
			return transportMailItem;
		}

		protected override ResubmitHelper.RecipientAction DetermineRecipientAction(MailRecipient recipient)
		{
			if (string.IsNullOrEmpty(recipient.PrimaryServerFqdnGuid))
			{
				return ResubmitHelper.RecipientAction.None;
			}
			ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId;
			if (!ShadowRedundancyManager.QualifiedPrimaryServerId.TryParse(recipient.PrimaryServerFqdnGuid, out qualifiedPrimaryServerId))
			{
				return ResubmitHelper.RecipientAction.None;
			}
			if (!string.Equals(qualifiedPrimaryServerId.Fqdn, this.PrimaryServerFqdn, StringComparison.OrdinalIgnoreCase))
			{
				return ResubmitHelper.RecipientAction.None;
			}
			return ResubmitHelper.RecipientAction.Copy;
		}

		protected override void TrackLatency(TransportMailItem mailItemToResubmit)
		{
			mailItemToResubmit.LatencyTracker = LatencyTracker.CreateInstance(LatencyComponent.ShadowQueue);
			LatencyTracker.TrackPreProcessLatency(LatencyComponent.ShadowQueue, mailItemToResubmit.LatencyTracker, mailItemToResubmit.DateReceived);
		}

		private static string GetShadowResubmitSourceContext(NextHopSolutionKey solutionKey, ResubmitReason reason)
		{
			return string.Format("{0} ({1})", solutionKey.NextHopDomain, reason);
		}

		internal const string ComponentNameForReceivedHeader = "ShadowRedundancy";

		private NextHopSolutionKey solutionKey;
	}
}
