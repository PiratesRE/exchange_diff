using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalDeliveryAgentEventSource
	{
		public InternalDeliveryAgentEventSource(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, RoutedMailItemWrapper currentMailItem, ulong sessionId, NextHopConnection nextHopConnection, string remoteHost, DeliveryAgentConnection.Stats stats)
		{
			this.mexSession = mexSession;
			this.currentMailItem = currentMailItem;
			this.sessionId = sessionId;
			this.nextHopConnection = nextHopConnection;
			this.remoteHost = remoteHost;
			this.stats = stats;
		}

		public bool ConnectionRegistered
		{
			get
			{
				return this.connectionRegistered;
			}
		}

		public bool MessageAcked
		{
			get
			{
				return this.messageAcked;
			}
		}

		public bool AnyRecipientsAcked
		{
			get
			{
				return this.recipientResponses.Count > 0;
			}
		}

		public bool ConnectionUnregistered
		{
			get
			{
				return this.connectionUnregistered;
			}
		}

		public string RemoteHost
		{
			get
			{
				return this.remoteHost;
			}
		}

		public void RegisterConnection(string remoteHost, SmtpResponse smtpResponse)
		{
			if (string.IsNullOrEmpty(remoteHost))
			{
				throw new ArgumentException("remoteHost");
			}
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfQueueResponseSet();
			this.remoteHost = remoteHost.Substring(0, Math.Min(remoteHost.Length, 64));
			ConnectionLog.DeliveryAgentStart(this.sessionId, this.mexSession.CurrentAgentName, this.nextHopConnection.Key);
			ConnectionLog.DeliveryAgentConnected(this.sessionId, this.remoteHost, smtpResponse);
			this.stats.ConnectionStarted();
			this.connectionRegistered = true;
		}

		public void UnregisterConnection(SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfQueueResponseSet();
			ConnectionLog.DeliveryAgentDisconnected(this.sessionId, this.remoteHost, smtpResponse);
			ConnectionLog.DeliveryAgentStop(this.sessionId, this.remoteHost, this.stats.NumMessagesDelivered, this.stats.NumBytesDelivered);
			if (this.currentMailItem != null)
			{
				this.AckMailItemPending();
			}
			this.AckConnection(AckStatus.Success, smtpResponse, null);
		}

		public void FailQueue(SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfQueueResponseSet();
			ConnectionLog.DeliveryAgentPermanentFailure(this.sessionId, this.remoteHost ?? this.nextHopConnection.Key.NextHopDomain, smtpResponse);
			this.AckConnection(AckStatus.Fail, smtpResponse, null);
		}

		public void DeferQueue(SmtpResponse smtpResponse)
		{
			this.DeferQueueInternal(smtpResponse, null);
		}

		public void DeferQueue(SmtpResponse smtpResponse, TimeSpan interval)
		{
			this.DeferQueueInternal(smtpResponse, new TimeSpan?(interval));
		}

		public void AckMailItemSuccess(SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfMailItemResponseSet();
			this.ThrowIfAnyRecipientResponseSet();
			this.stats.MessageDelivered(this.currentMailItem.Recipients.Count, this.currentMailItem.MimeStreamLength);
			this.AckMailItem(AckStatus.Success, smtpResponse, null);
		}

		public void AckMailItemDefer(SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfMailItemResponseSet();
			this.ThrowIfAnyRecipientResponseSet();
			this.AckMailItemDeferInternal(smtpResponse, new TimeSpan?(InternalDeliveryAgentEventSource.DefaultMailItemRetryInterval));
		}

		public void AckMailItemPending()
		{
			this.ThrowIfMailItemResponseSet();
			this.ThrowIfAnyRecipientResponseSet();
			this.AckMailItem(AckStatus.Pending, SmtpResponse.Empty, null);
		}

		public void AckMailItemFail(SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfMailItemResponseSet();
			this.ThrowIfAnyRecipientResponseSet();
			this.stats.MessageFailed();
			this.AckMailItem(AckStatus.Fail, smtpResponse, null);
		}

		public void AckRecipientSuccess(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.AckRecipient(recipient, AckStatus.Success, smtpResponse);
		}

		public void AckRecipientDefer(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.AckRecipient(recipient, AckStatus.Retry, smtpResponse);
		}

		public void AckRecipientFail(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.AckRecipient(recipient, AckStatus.Fail, smtpResponse);
		}

		public void AckRemainingRecipientsAndFinalizeMailItem(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			foreach (EnvelopeRecipient envelopeRecipient in this.currentMailItem.Recipients)
			{
				if (!this.recipientResponses.ContainsKey(envelopeRecipient.Address))
				{
					this.AckRecipient(envelopeRecipient, ackStatus, smtpResponse);
				}
			}
			this.UpdateRecipientStats();
			this.AckMailItem(AckStatus.Success, SmtpResponse.Empty, null);
		}

		public void AddDsnParameters(string key, object value)
		{
			this.ThrowIfMailItemResponseSet();
			this.currentMailItem.RoutedMailItem.AddDsnParameters(key, value);
		}

		public bool TryGetDsnParameters(string key, out object value)
		{
			this.ThrowIfMailItemResponseSet();
			value = null;
			return this.currentMailItem.RoutedMailItem.DsnParameters != null && this.currentMailItem.RoutedMailItem.DsnParameters.TryGetValue(key, out value);
		}

		public void AddDsnParameters(EnvelopeRecipient recipient, string key, object value)
		{
			this.ThrowIfInvalidRecipient(recipient);
			this.ThrowIfMailItemResponseSet();
			MailRecipientWrapper mailRecipientWrapper = (MailRecipientWrapper)recipient;
			mailRecipientWrapper.MailRecipient.AddDsnParameters(key, value);
		}

		public bool TryGetDsnParameters(EnvelopeRecipient recipient, string key, out object value)
		{
			this.ThrowIfInvalidRecipient(recipient);
			this.ThrowIfMailItemResponseSet();
			value = null;
			MailRecipientWrapper mailRecipientWrapper = (MailRecipientWrapper)recipient;
			return mailRecipientWrapper.MailRecipient.DsnParameters != null && mailRecipientWrapper.MailRecipient.DsnParameters.TryGetValue(key, out value);
		}

		private void AckRecipient(EnvelopeRecipient recipient, AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.ThrowIfInvalidRecipient(recipient);
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfMailItemResponseSet();
			this.ThrowIfRecipientResponseSet(recipient.Address);
			this.recipientResponses.Add(recipient.Address, new AckStatusAndResponse(ackStatus, smtpResponse));
		}

		private void UpdateRecipientStats()
		{
			if (this.recipientResponses.Count != this.currentMailItem.Recipients.Count)
			{
				throw new InvalidOperationException("All recipients need to be acked before updating stats");
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (AckStatusAndResponse ackStatusAndResponse in this.recipientResponses.Values)
			{
				switch (ackStatusAndResponse.AckStatus)
				{
				case AckStatus.Success:
					num++;
					break;
				case AckStatus.Retry:
					num2++;
					break;
				case AckStatus.Fail:
					num3++;
					break;
				default:
					throw new InvalidOperationException("Unexpected ack status");
				}
			}
			if (num > 0)
			{
				this.stats.MessageDelivered(num, this.currentMailItem.MimeStreamLength);
			}
			if (num2 > 0)
			{
				this.stats.MessageDeferred();
			}
			if (num3 > 0)
			{
				this.stats.MessageFailed();
			}
		}

		private void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, TimeSpan? retryInterval)
		{
			MailRecipient nextRecipient;
			while ((nextRecipient = this.nextHopConnection.GetNextRecipient()) != null)
			{
				AckStatusAndResponse ackStatusAndResponse;
				if (this.recipientResponses.TryGetValue(nextRecipient.Email, out ackStatusAndResponse))
				{
					this.nextHopConnection.AckRecipient(ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
				}
				else
				{
					this.nextHopConnection.AckRecipient(ackStatus, smtpResponse);
				}
			}
			this.nextHopConnection.AckMailItem(ackStatus, smtpResponse, null, retryInterval, MessageTrackingSource.AGENT, this.mexSession.CurrentAgentName, LatencyComponent.DeliveryAgent, true);
			if (this.currentMailItem != null)
			{
				this.currentMailItem.Close();
				this.currentMailItem = null;
			}
			this.messageAcked = true;
		}

		private void AckMailItemDeferInternal(SmtpResponse smtpResponse, TimeSpan? interval)
		{
			if (interval == null)
			{
				interval = new TimeSpan?(InternalDeliveryAgentEventSource.DefaultMailItemRetryInterval);
			}
			this.stats.MessageDeferred();
			this.AckMailItem(AckStatus.Retry, smtpResponse, interval);
		}

		private void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, TimeSpan? retryInterval)
		{
			if (this.stats.HasOpenConnection)
			{
				this.stats.ConnectionStopped();
			}
			if (ackStatus != AckStatus.Success)
			{
				this.stats.ConnectionFailed();
			}
			this.nextHopConnection.AckConnection(MessageTrackingSource.AGENT, this.mexSession.CurrentAgentName, ackStatus, smtpResponse, null, retryInterval);
			this.nextHopConnection = null;
			this.connectionUnregistered = true;
			this.messageAcked = true;
		}

		private void DeferQueueInternal(SmtpResponse smtpResponse, TimeSpan? interval)
		{
			this.ThrowIfInvalidResponse(smtpResponse);
			this.ThrowIfQueueResponseSet();
			if (interval != null)
			{
				this.ThrowIfInvalidInterval(interval.Value);
			}
			if (this.currentMailItem != null)
			{
				this.AckMailItemPending();
			}
			ConnectionLog.DeliveryAgentQueueRetry(this.sessionId, this.remoteHost ?? this.nextHopConnection.Key.NextHopDomain, smtpResponse);
			this.AckConnection(AckStatus.Retry, smtpResponse, interval);
		}

		private void ThrowIfInvalidResponse(SmtpResponse smtpResponse)
		{
			if (smtpResponse.Equals(SmtpResponse.Empty))
			{
				throw new ArgumentException("smtpResponse");
			}
		}

		private void ThrowIfInvalidInterval(TimeSpan interval)
		{
			if (interval <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("interval");
			}
		}

		private void ThrowIfQueueResponseSet()
		{
			if (this.connectionRegistered || this.connectionUnregistered)
			{
				throw new InvalidOperationException("Only one of FailQueue(), DeferQueue(), RegisterConnection(), or UnregisterConnection() can be called, and it can only be called once.");
			}
		}

		private void ThrowIfMailItemResponseSet()
		{
			if (this.messageAcked)
			{
				throw new InvalidOperationException("Only one of AckMailItemSuccess(), AckMailItemDefer(), or AckMailItemFail() can be called, and it can only be called once.");
			}
		}

		private void ThrowIfInvalidRecipient(EnvelopeRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (!(recipient is MailRecipientWrapper))
			{
				throw new ArgumentException("Invalid recipient type", "recipient");
			}
			if (!this.currentMailItem.Recipients.Contains(recipient.Address))
			{
				throw new ArgumentException("Recipient does not exist in message", "recipient");
			}
		}

		private void ThrowIfAnyRecipientResponseSet()
		{
			if (this.recipientResponses.Count > 0)
			{
				throw new InvalidOperationException("You cannot ack a mail item if you have already acked the mail item, acked any recipient, deferred or failed the queue, or unregistered the connection.");
			}
		}

		private void ThrowIfRecipientResponseSet(RoutingAddress recipient)
		{
			if (this.recipientResponses.ContainsKey(recipient))
			{
				throw new InvalidOperationException("You cannot ack a recipient if you have already acked the recipient, acked the mail item, deferred or failed the queue, or unregistered the connection.");
			}
		}

		public static readonly TimeSpan DefaultMailItemRetryInterval = TimeSpan.FromMinutes(1.0);

		private DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession;

		private RoutedMailItemWrapper currentMailItem;

		private Dictionary<RoutingAddress, AckStatusAndResponse> recipientResponses = new Dictionary<RoutingAddress, AckStatusAndResponse>();

		private ulong sessionId;

		private NextHopConnection nextHopConnection;

		private DeliveryAgentConnection.Stats stats;

		private bool connectionRegistered;

		private bool messageAcked;

		private bool connectionUnregistered;

		private string remoteHost;
	}
}
