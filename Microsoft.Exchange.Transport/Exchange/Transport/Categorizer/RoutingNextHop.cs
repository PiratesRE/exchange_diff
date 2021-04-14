using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class RoutingNextHop
	{
		public abstract DeliveryType DeliveryType { get; }

		public abstract Guid NextHopGuid { get; }

		public virtual SmtpSendConnectorConfig NextHopConnector
		{
			get
			{
				return null;
			}
		}

		public virtual bool IsMandatoryTopologyHop
		{
			get
			{
				return false;
			}
		}

		public abstract string GetNextHopDomain(RoutingContext context);

		public virtual IEnumerable<INextHopServer> GetLoadBalancedNextHopServers(string nextHopDomain)
		{
			throw new NotSupportedException("GetLoadBalancedNextHopServers is not supported in " + base.GetType());
		}

		public abstract bool Match(RoutingNextHop other);

		public virtual void UpdateRecipient(MailRecipient recipient, RoutingContext context)
		{
			recipient.NextHop = this.GetNextHopSolutionKey(context);
		}

		public virtual void TraceRoutingResult(MailRecipient recipient, RoutingContext context)
		{
			RoutingDiag.SystemProbeTracer.TracePass(context.MailItem, (long)this.GetHashCode(), "[{0}] Recipient '{1}' of type {2} and final destination '{3}' routed to next hop type '{4}', next hop domain '{5}', connector {6}", new object[]
			{
				context.Timestamp,
				recipient.Email.ToString(),
				recipient.Type,
				recipient.FinalDestination,
				recipient.NextHop.NextHopType,
				recipient.NextHop.NextHopDomain,
				recipient.NextHop.NextHopConnector
			});
			RoutingDiag.Tracer.TracePfd((long)this.GetHashCode(), "PFD CAT {0} Routed recipient {1} (msgId={2}) to next hop type '{3}', next hop domain '{4}', connector {5}", new object[]
			{
				23458,
				recipient.Email.ToString(),
				context.MailItem.RecordId,
				recipient.NextHop.NextHopType,
				recipient.NextHop.NextHopDomain,
				recipient.NextHop.NextHopConnector
			});
		}

		protected virtual NextHopSolutionKey GetNextHopSolutionKey(RoutingContext context)
		{
			return new NextHopSolutionKey(this.DeliveryType, this.GetNextHopDomain(context), this.NextHopGuid);
		}
	}
}
