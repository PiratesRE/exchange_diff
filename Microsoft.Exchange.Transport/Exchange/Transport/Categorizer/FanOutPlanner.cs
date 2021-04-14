using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class FanOutPlanner
	{
		public FanOutPlanner(RoutingTables routingTables)
		{
			this.siteRelayMap = routingTables.ServerMap.SiteRelayMap;
			this.timestamp = routingTables.WhenCreated;
			this.recipientMap = new Dictionary<Guid, FanOutPlanner.PathRecipients>();
		}

		public void AddRecipient(MailRecipient recipient)
		{
			Guid nextHopConnector = recipient.NextHop.NextHopConnector;
			ADSiteRelayMap.ADTopologyPath adtopologyPath;
			if (!this.siteRelayMap.TryGetPath(nextHopConnector, out adtopologyPath))
			{
				ExTraceGlobals.RoutingTracer.TraceError<DateTime, Guid, RoutingAddress>((long)this.GetHashCode(), "[{0}] [DFO] Cannot find route to AD site '{1}' for recipient '{2}'. E-DNS will handle it as a config change.", this.timestamp, nextHopConnector, recipient.Email);
				return;
			}
			Guid guid = adtopologyPath.FirstHopSiteGuid();
			FanOutPlanner.PathRecipients pathRecipients;
			if (this.recipientMap.TryGetValue(guid, out pathRecipients))
			{
				pathRecipients.AddRecipient(ADSiteRelayMap.ADTopologyPath.GetCommonPath(adtopologyPath, pathRecipients.CommonPath), recipient);
				ExTraceGlobals.RoutingTracer.TraceDebug<DateTime, RoutingAddress, Guid>((long)this.GetHashCode(), "[{0}] [DFO] Recipient '{1}' added to existing group with first hop id '{2}'.", this.timestamp, recipient.Email, guid);
				return;
			}
			if (TransportHelpers.AttemptAddToDictionary<Guid, FanOutPlanner.PathRecipients>(this.recipientMap, guid, new FanOutPlanner.PathRecipients(adtopologyPath, recipient), new TransportHelpers.DiagnosticsHandler<Guid, FanOutPlanner.PathRecipients>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, FanOutPlanner.PathRecipients>)))
			{
				ExTraceGlobals.RoutingTracer.TraceDebug<DateTime, RoutingAddress, Guid>((long)this.GetHashCode(), "[{0}] [DFO] Created a new group for recipient '{1}' and with first hop id '{2}'.", this.timestamp, recipient.Email, guid);
			}
		}

		public void UpdateRecipientNextHops()
		{
			foreach (FanOutPlanner.PathRecipients pathRecipients in this.recipientMap.Values)
			{
				pathRecipients.UpdateRecipientNextHops(this.timestamp);
			}
		}

		private DateTime timestamp;

		private ADSiteRelayMap siteRelayMap;

		private Dictionary<Guid, FanOutPlanner.PathRecipients> recipientMap;

		private class PathRecipients
		{
			public PathRecipients(ADSiteRelayMap.ADTopologyPath path, MailRecipient recipient)
			{
				this.recipients = new List<MailRecipient>();
				this.AddRecipient(path, recipient);
			}

			public ADSiteRelayMap.ADTopologyPath CommonPath
			{
				get
				{
					return this.commonPath;
				}
			}

			public void AddRecipient(ADSiteRelayMap.ADTopologyPath newCommonPath, MailRecipient recipient)
			{
				this.commonPath = newCommonPath;
				this.recipients.Add(recipient);
			}

			public void UpdateRecipientNextHops(DateTime timestamp)
			{
				if (this.recipients.Count > 1)
				{
					foreach (MailRecipient mailRecipient in this.recipients)
					{
						ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "[{0}] [DFO] Changing next hop site for recipient '{1}' from '{2}:{3}' to '{4}:{5}'.", new object[]
						{
							timestamp,
							mailRecipient.Email,
							mailRecipient.NextHop.NextHopDomain,
							mailRecipient.NextHop.NextHopConnector,
							this.commonPath.NextHopSite.Name,
							this.commonPath.NextHopSite.Guid
						});
						ExTraceGlobals.RoutingTracer.TracePfd((long)this.GetHashCode(), "PFD CAT {0} Delayed Fan-out: Changing next hop site for recipient '{1}' from '{2}' to '{3}'.", new object[]
						{
							17570,
							mailRecipient.Email,
							mailRecipient.NextHop.NextHopDomain,
							this.commonPath.NextHopSite.Name
						});
						mailRecipient.NextHop = new NextHopSolutionKey(mailRecipient.NextHop.NextHopType, this.commonPath.NextHopSite.Name, this.commonPath.NextHopSite.Guid);
					}
				}
			}

			private ADSiteRelayMap.ADTopologyPath commonPath;

			private List<MailRecipient> recipients;
		}
	}
}
