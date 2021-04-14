using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Smtp
{
	internal class SmtpMailItemNextHopConnection : NextHopConnection, IDisposable
	{
		public SmtpMailItemNextHopConnection(NextHopSolutionKey key, IReadOnlyMailItem mailItem, ISmtpMailItemSenderNotifications notificationHandler) : base(null, 0L, DeliveryPriority.Normal, null)
		{
			this.key = key;
			this.mailItem = mailItem;
			this.readyRecipients = this.GetReadyRecipients();
			this.recipientsPending = this.readyRecipients.Count;
			this.recipientEnumerator = this.mailItem.Recipients.GetEnumerator();
			this.recipientEnumeratorAck = this.mailItem.Recipients.GetEnumerator();
			this.notificationHandler = notificationHandler;
		}

		public WaitHandle AckConnectionEvent
		{
			get
			{
				return this.autoResetEvent;
			}
		}

		public SmtpMailItemResult SmtpMailItemResult
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
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
				throw new NotSupportedException("This should not be called on Mailbox Transport Submission");
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
				throw new NotSupportedException("This should not be called on Mailbox Transport Submission");
			}
		}

		public override IEnumerable<MailRecipient> ReadyRecipients
		{
			get
			{
				return this.readyRecipients;
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

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
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
			if (this.mailItemSentForProcessing)
			{
				return null;
			}
			if (this.getMailItemBeingCalledForTheFirstTime)
			{
				SmtpMailItemNextHopConnection.InitializeSmtpLatencyTracking(this.mailItem.LatencyTracker);
				this.getMailItemBeingCalledForTheFirstTime = false;
			}
			return this.mailItem;
		}

		public override MailRecipient GetNextRecipient()
		{
			while (this.recipientEnumerator.MoveNext())
			{
				MailRecipient mailRecipient = this.recipientEnumerator.Current;
				if (mailRecipient.Status == Status.Ready)
				{
					return mailRecipient;
				}
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
				if (this.result == null)
				{
					this.result = new SmtpMailItemResult();
				}
				this.result.ConnectionResponse = new AckStatusAndResponse(status, smtpResponse);
				if (details != null)
				{
					this.result.RemoteHostName = details.RemoteHostName;
				}
				if (this.notificationHandler != null)
				{
					this.notificationHandler.AckConnection(status, smtpResponse);
				}
				SmtpMailItemNextHopConnection.EndSmtpLatencyTracking(this.mailItem.LatencyTracker);
				if (this.autoResetEvent != null)
				{
					this.autoResetEvent.Set();
					return;
				}
				break;
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
			case AckStatus.Quarantine:
				break;
			default:
				return;
			}
		}

		public override void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			if (this.recipientsPending != 0 && ackStatus == AckStatus.Success)
			{
				throw new InvalidOperationException("Cannot ack message successfully until all pending recipients have been acked");
			}
			if (ackStatus == AckStatus.Pending)
			{
				this.recipientsPending = this.readyRecipients.Count;
				this.recipientEnumerator = this.mailItem.Recipients.GetEnumerator();
				this.recipientEnumeratorAck = this.mailItem.Recipients.GetEnumerator();
				this.result = null;
			}
			else
			{
				if (this.result == null)
				{
					this.result = new SmtpMailItemResult();
				}
				this.result.MessageResponse = new AckStatusAndResponse(ackStatus, smtpResponse);
				this.mailItemSentForProcessing = true;
			}
			if (this.notificationHandler != null)
			{
				this.notificationHandler.AckMessage(ackStatus, smtpResponse);
			}
		}

		public override void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			TraceHelper.SmtpSendTracer.TracePass<string, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "InboundProxyNextHopConnection.AckRecipient. Ackstatus  = {0}. SmtpResponse = {1}", ackStatus.ToString(), smtpResponse.ToString());
			if (!this.recipientEnumeratorAck.MoveNext() || this.recipientsPending <= 0)
			{
				throw new InvalidOperationException("AckRecipient called but no recipients left to ack");
			}
			this.recipientsPending--;
			MailRecipient recipient = this.recipientEnumeratorAck.Current;
			switch (ackStatus)
			{
			case AckStatus.Pending:
			case AckStatus.Success:
			case AckStatus.Retry:
			case AckStatus.Fail:
				if (this.result == null)
				{
					this.result = new SmtpMailItemResult();
				}
				if (this.result.RecipientResponses == null)
				{
					this.result.RecipientResponses = new Dictionary<MailRecipient, AckStatusAndResponse>();
				}
				this.result.RecipientResponses.Add(recipient, new AckStatusAndResponse(ackStatus, smtpResponse));
				if (this.notificationHandler != null)
				{
					this.notificationHandler.AckRecipient(ackStatus, smtpResponse, recipient);
				}
				return;
			default:
				throw new InvalidOperationException(string.Format("AckRecipient with status: {0} is invalid", ackStatus));
			}
		}

		public override void ResetQueueLastRetryTimeAndError()
		{
			throw new NotSupportedException();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.autoResetEvent.Dispose();
				this.autoResetEvent = null;
				this.disposed = true;
			}
		}

		private static void InitializeSmtpLatencyTracking(LatencyTracker latencyTracker)
		{
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission)
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtp, latencyTracker);
			}
		}

		private static void EndSmtpLatencyTracking(LatencyTracker latencyTracker)
		{
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtp, LatencyComponent.SmtpSend, latencyTracker);
			}
		}

		private IList<MailRecipient> GetReadyRecipients()
		{
			List<MailRecipient> list = new List<MailRecipient>(this.mailItem.Recipients.Count);
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Ready)
				{
					list.Add(mailRecipient);
				}
			}
			return list;
		}

		private IEnumerator<MailRecipient> recipientEnumerator;

		private int recipientsPending;

		private IEnumerator<MailRecipient> recipientEnumeratorAck;

		private IReadOnlyMailItem mailItem;

		private NextHopSolutionKey key;

		private bool mailItemSentForProcessing;

		private bool getMailItemBeingCalledForTheFirstTime = true;

		private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

		private SmtpMailItemResult result;

		private ISmtpMailItemSenderNotifications notificationHandler;

		private bool disposed;

		private IList<MailRecipient> readyRecipients;
	}
}
