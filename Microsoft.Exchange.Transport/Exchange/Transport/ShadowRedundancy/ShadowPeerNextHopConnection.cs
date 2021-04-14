using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class ShadowPeerNextHopConnection : NextHopConnection
	{
		public ShadowPeerNextHopConnection(ShadowSession shadowSession, IInboundProxyLayer proxyLayer, IMailRouter mailRouter, TransportMailItem mailItem) : base(null, 0L, DeliveryPriority.Normal, null)
		{
			if (shadowSession == null)
			{
				throw new ArgumentNullException("shadowSession");
			}
			if (proxyLayer == null)
			{
				throw new ArgumentNullException("proxyLayer");
			}
			if (mailRouter == null)
			{
				throw new ArgumentNullException("mailRouter");
			}
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			this.shadowSession = shadowSession;
			this.proxyLayer = proxyLayer;
			this.mailRouter = mailRouter;
			this.mailItem = new ShadowRoutedMailItem(mailItem);
			this.recipientsPending = this.mailItem.Recipients.Count;
			this.recipientEnumerator = this.mailItem.Recipients.GetEnumerator();
			this.key = new NextHopSolutionKey(NextHopType.ShadowRedundancy, "ShadowRedundancy", Guid.Empty);
		}

		public ShadowSession ShadowSession
		{
			get
			{
				return this.shadowSession;
			}
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
				throw new NotSupportedException("This should not be called for shadowing");
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
				throw new NotSupportedException("This should not be called for shadowing");
			}
		}

		public override IEnumerable<MailRecipient> ReadyRecipients
		{
			get
			{
				return this.mailItem.Recipients;
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

		public override void CreateConnectionIfNecessary()
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
			ShadowRedundancyManager.SendTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ShadowPeerNextHopConnection.AckConnection. Ackstatus  = {0}. SmtpResponse = {1}", status.ToString(), smtpResponse.ToString());
			this.shadowSession.DropBreadcrumb(ShadowBreadcrumbs.NextHopAckConnection);
			switch (status)
			{
			case AckStatus.Pending:
			case AckStatus.Resubmit:
			case AckStatus.Skip:
				throw new InvalidOperationException("Invalid status");
			case AckStatus.Success:
			case AckStatus.Retry:
			case AckStatus.Fail:
				this.shadowSession.Close(status, smtpResponse);
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

		public override void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			ShadowRedundancyManager.SendTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ShadowPeerNextHopConnection.AckMailItem. Ackstatus  = {0}. SmtpResponse = {1}", ackStatus.ToString(), smtpResponse.ToString());
			this.shadowSession.DropBreadcrumb(ShadowBreadcrumbs.NextHopConnectionAckMailItem);
			if (this.recipientsPending != 0 && ackStatus == AckStatus.Success)
			{
				throw new InvalidOperationException("Cannot ack message successfully until all pending recipients have been acked");
			}
			if (ackStatus == AckStatus.Pending)
			{
				this.recipientsPending = this.mailItem.Recipients.Count;
				this.recipientEnumerator = this.mailItem.Recipients.GetEnumerator();
				this.shadowSession.NotifyProxyFailover(primaryServer, smtpResponse);
				return;
			}
			this.mailItem = null;
			this.recipientEnumerator = null;
			this.shadowSession.NotifyShadowServerResponse(primaryServer, smtpResponse);
			if (ackStatus == AckStatus.Success)
			{
				ShadowRedundancyManager.PerfCounters.MessageShadowed(primaryServer, !this.mailRouter.IsInLocalSite(primaryServer));
			}
			this.shadowSession.AckMessage(ackStatus, smtpResponse);
		}

		public override void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			ShadowRedundancyManager.SendTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ShadowPeerNextHopConnection.AckRecipient. Ackstatus  = {0}. SmtpResponse = {1}", ackStatus.ToString(), smtpResponse.ToString());
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

		public override void NotifyConnectionFailedOver(string targetHostName, SmtpResponse failoverResponse, SessionSetupFailureReason failoverReason)
		{
			ShadowRedundancyManager.SendTracer.TraceDebug<string, SmtpResponse, SessionSetupFailureReason>((long)this.GetHashCode(), "ShadowPeerNextHopConnection.NotifyConnectionFailedOver. targetHostName={0} response={1} reason={2}", targetHostName, failoverResponse, failoverReason);
			this.shadowSession.DropBreadcrumb(ShadowBreadcrumbs.NextHopConnectionFailedOver);
			this.shadowSession.NotifyShadowServerResponse(targetHostName, failoverResponse);
		}

		private IEnumerator<MailRecipient> recipientEnumerator;

		private int recipientsPending;

		private ShadowRoutedMailItem mailItem;

		private NextHopSolutionKey key;

		private ShadowSession shadowSession;

		private IInboundProxyLayer proxyLayer;

		private IMailRouter mailRouter;
	}
}
