using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class UnreachableNextHop : RoutingNextHop
	{
		private UnreachableNextHop(UnreachableReason reason)
		{
			this.reason = reason;
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return DeliveryType.Unreachable;
			}
		}

		public override Guid NextHopGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public override string GetNextHopDomain(RoutingContext context)
		{
			return NextHopSolutionKey.Unreachable.NextHopDomain;
		}

		public override bool Match(RoutingNextHop other)
		{
			return object.ReferenceEquals(this, other);
		}

		public override void UpdateRecipient(MailRecipient recipient, RoutingContext context)
		{
			base.UpdateRecipient(recipient, context);
			recipient.UnreachableReason = this.reason;
		}

		public override void TraceRoutingResult(MailRecipient recipient, RoutingContext context)
		{
			RoutingDiag.SystemProbeTracer.TraceFail(context.MailItem, (long)this.GetHashCode(), "[{0}] Failed to route recipient '{1}' of type {2} and final destination '{3}'; Unreachable reason = {4}", new object[]
			{
				context.Timestamp,
				recipient.Email.ToString(),
				recipient.Type,
				recipient.FinalDestination,
				this.reason
			});
			RoutingDiag.Tracer.TracePfd((long)this.GetHashCode(), "PFD CAT {0} Routing failed for recipient {1} (msgId={2}); recipient type {3}; final destination '{4}'; Unreachable reason {5}", new object[]
			{
				31650,
				recipient.Email.ToString(),
				context.MailItem.RecordId,
				recipient.Type,
				recipient.FinalDestination,
				this.reason
			});
		}

		protected override NextHopSolutionKey GetNextHopSolutionKey(RoutingContext context)
		{
			return NextHopSolutionKey.Unreachable;
		}

		public static readonly UnreachableNextHop IncompatibleDeliveryDomain = new UnreachableNextHop(UnreachableReason.IncompatibleDeliveryDomain);

		public static readonly UnreachableNextHop NoDatabase = new UnreachableNextHop(UnreachableReason.NoMdb);

		public static readonly UnreachableNextHop NoRouteToDatabase = new UnreachableNextHop(UnreachableReason.NoRouteToMdb);

		public static readonly UnreachableNextHop NoRouteToServer = new UnreachableNextHop(UnreachableReason.NoRouteToMta);

		public static readonly UnreachableNextHop NoMatchingConnector = new UnreachableNextHop(UnreachableReason.NoMatchingConnector);

		public static readonly UnreachableNextHop NonHubExpansionServer = new UnreachableNextHop(UnreachableReason.NonBHExpansionServer);

		private UnreachableReason reason;
	}
}
