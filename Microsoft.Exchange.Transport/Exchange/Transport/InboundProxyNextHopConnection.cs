using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class InboundProxyNextHopConnection : NextHopConnection
	{
		public InboundProxyNextHopConnection(IInboundProxyLayer proxyLayer, NextHopSolutionKey key, InboundProxyRoutedMailItem mailItem) : base(null, 0L, DeliveryPriority.Normal, null)
		{
			this.proxyLayer = proxyLayer;
			this.key = key;
			this.mailItem = mailItem;
			this.recipientsPending = this.mailItem.Recipients.Count;
			this.recipientEnumerator = this.mailItem.RecipientList.GetEnumerator();
		}

		public IInboundProxyLayer ProxyLayer
		{
			get
			{
				return this.proxyLayer;
			}
		}

		public override NextHopSolutionKey Key
		{
			get
			{
				return this.key;
			}
		}

		public override IReadOnlyMailItem ReadOnlyMailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public override RoutedMailItem RoutedMailItem
		{
			get
			{
				throw new NotSupportedException("This should not be called on Front End");
			}
		}

		public override int MaxMessageRecipients
		{
			get
			{
				return this.mailItem.Recipients.Count;
			}
		}

		public override int RecipientCount
		{
			get
			{
				return this.recipientsPending;
			}
		}

		public override IList<MailRecipient> ReadyRecipientsList
		{
			get
			{
				throw new NotSupportedException("This should not be called on Front End");
			}
		}

		public override IEnumerable<MailRecipient> ReadyRecipients
		{
			get
			{
				return this.mailItem.RecipientList;
			}
		}

		public override int ActiveQueueLength
		{
			get
			{
				return 0;
			}
		}

		public override int TotalQueueLength
		{
			get
			{
				return 0;
			}
		}

		public override void ConnectionAttemptSucceeded()
		{
		}

		public override RoutedMailItem GetNextRoutedMailItem()
		{
			throw new NotSupportedException();
		}

		public override IReadOnlyMailItem GetNextMailItem()
		{
			return this.mailItem;
		}

		public override MailRecipient GetNextRecipient()
		{
			if (this.recipientEnumerator.MoveNext())
			{
				return this.recipientEnumerator.Current;
			}
			return null;
		}

		public override void AckConnection(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, bool resubmitWithoutHighAvailablityRouting, SessionSetupFailureReason failureReason)
		{
			switch (status)
			{
			case AckStatus.Pending:
			case AckStatus.Skip:
				throw new InvalidOperationException("Invalid status");
			case AckStatus.Success:
			case AckStatus.Retry:
			case AckStatus.Fail:
			case AckStatus.Resubmit:
				this.proxyLayer.AckConnection(status, smtpResponse, failureReason);
				return;
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
			case AckStatus.Quarantine:
				return;
			default:
				return;
			}
		}

		public override void CreateConnectionIfNecessary()
		{
		}

		public override void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			if (this.recipientsPending != 0 && ackStatus == AckStatus.Success)
			{
				throw new InvalidOperationException("Cannot ack message successfully until all pending recipients have been acked");
			}
			if (ackStatus == AckStatus.Pending)
			{
				this.recipientsPending = this.mailItem.Recipients.Count;
				this.recipientEnumerator = this.mailItem.RecipientList.GetEnumerator();
				return;
			}
			this.mailItem.FinalizeDeliveryLatencyTracking(deliveryComponent);
			if (ackStatus == AckStatus.Success)
			{
				LatencyFormatter latencyFormatter = new LatencyFormatter(this.mailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, false);
				latencyFormatter.FormatAndUpdatePerfCounters();
			}
			this.proxyLayer.ReleaseMailItem();
			this.mailItem = null;
			this.recipientEnumerator = null;
		}

		public override void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string, string>((long)this.GetHashCode(), "InboundProxyNextHopConnection.AckRecipient. Ackstatus  = {0}. SmtpResponse = {1}", ackStatus.ToString(), smtpResponse.ToString());
			if (this.recipientsPending <= 0)
			{
				throw new InvalidOperationException("AckRecipient called but no recipients left to ack");
			}
			this.recipientsPending--;
		}

		public override void ResetQueueLastRetryTimeAndError()
		{
			throw new NotSupportedException();
		}

		private IEnumerator<MailRecipient> recipientEnumerator;

		private int recipientsPending;

		private InboundProxyRoutedMailItem mailItem;

		private NextHopSolutionKey key;

		private IInboundProxyLayer proxyLayer;
	}
}
