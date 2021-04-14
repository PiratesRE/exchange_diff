using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class NdrNextHop : RoutingNextHop
	{
		private NdrNextHop(SmtpResponse ackReason)
		{
			this.ackReason = ackReason;
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return DeliveryType.Undefined;
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
			return string.Empty;
		}

		public override bool Match(RoutingNextHop other)
		{
			return object.ReferenceEquals(this, other);
		}

		public override void UpdateRecipient(MailRecipient recipient, RoutingContext context)
		{
			base.UpdateRecipient(recipient, context);
			recipient.Ack(AckStatus.Fail, this.ackReason);
			context.Core.PerfCounters.IncrementRoutingNdrs();
		}

		public override void TraceRoutingResult(MailRecipient recipient, RoutingContext context)
		{
			RoutingDiag.SystemProbeTracer.TraceFail(context.MailItem, (long)this.GetHashCode(), "[{0}] Failed to route recipient '{1}' of type {2} and final destination '{3}'; NDR response = {4}", new object[]
			{
				context.Timestamp,
				recipient.Email.ToString(),
				recipient.Type,
				recipient.FinalDestination,
				this.ackReason
			});
			RoutingDiag.Tracer.TracePfd((long)this.GetHashCode(), "PFD CAT {0} Routing failed for recipient {1} (msgId={2}); recipient type {3}; final destination '{4}'; NDR response {5}", new object[]
			{
				31650,
				recipient.Email.ToString(),
				context.MailItem.RecordId,
				recipient.Type,
				recipient.FinalDestination,
				this.ackReason
			});
		}

		public static readonly NdrNextHop InvalidAddressForRouting = new NdrNextHop(AckReason.InvalidAddressForRouting);

		public static readonly NdrNextHop InvalidX400AddressForRouting = new NdrNextHop(AckReason.InvalidX400AddressForRouting);

		public static readonly NdrNextHop MessageTooLargeForRoute = new NdrNextHop(AckReason.MessageTooLargeForRoute);

		public static readonly NdrNextHop NoConnectorForAddressType = new NdrNextHop(AckReason.NoConnectorForAddressType);

		private SmtpResponse ackReason;
	}
}
