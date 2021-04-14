using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal class NextHopConnection
	{
		public NextHopConnection(RoutedMessageQueue routedMessageQueue, long connectionId, DeliveryPriority priority, ConnectionManager parent)
		{
			this.routedMessageQueue = routedMessageQueue;
			this.connectionId = connectionId;
			this.parent = parent;
			this.priority = priority;
			if (this.routedMessageQueue != null && this.routedMessageQueue.Key.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				this.generateSuccessDSNs = DsnFlags.Delivery;
			}
		}

		public virtual DsnFlags GenerateSuccessDSNs
		{
			get
			{
				return this.generateSuccessDSNs;
			}
			set
			{
				this.generateSuccessDSNs = value;
			}
		}

		public virtual NextHopSolutionKey Key
		{
			get
			{
				return this.routedMessageQueue.Key;
			}
		}

		public virtual int ActiveQueueLength
		{
			get
			{
				return this.routedMessageQueue.ActiveQueueLength;
			}
		}

		public virtual int TotalQueueLength
		{
			get
			{
				return this.routedMessageQueue.TotalQueueLength;
			}
		}

		public virtual IReadOnlyMailItem ReadOnlyMailItem
		{
			get
			{
				return this.RoutedMailItem;
			}
		}

		public virtual RoutedMailItem RoutedMailItem
		{
			get
			{
				return this.routedMailItem;
			}
		}

		public virtual int MaxMessageRecipients
		{
			get
			{
				return this.maxMessageRecipients;
			}
			set
			{
				this.maxMessageRecipients = value;
			}
		}

		public virtual int RecipientCount
		{
			get
			{
				if (this.readyRecipients == null)
				{
					return 0;
				}
				int num = 0;
				foreach (MailRecipient mailRecipient in this.readyRecipients)
				{
					if (mailRecipient.Status == Status.Ready)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual IEnumerable<MailRecipient> ReadyRecipients
		{
			get
			{
				return this.readyRecipients;
			}
		}

		public virtual IList<MailRecipient> ReadyRecipientsList
		{
			get
			{
				return this.readyRecipients;
			}
		}

		public bool RetryQueueRequested
		{
			get
			{
				return this.retryQueueRequested;
			}
		}

		public SmtpResponse RetryQueueSmtpResponse
		{
			get
			{
				return this.retryQueueSmtpResponse;
			}
		}

		public virtual void ConnectionAttemptSucceeded()
		{
			if (!this.isConnectionAttemptSucceeded)
			{
				this.routedMessageQueue.ConnectionAttemptSucceeded(this.priority, this.connectionId);
				this.isConnectionAttemptSucceeded = true;
			}
		}

		public virtual RoutedMailItem GetNextRoutedMailItem()
		{
			if (this.routedMailItem == null)
			{
				if (Components.RemoteDeliveryComponent.IsPaused)
				{
					return null;
				}
				this.routedMailItem = this.routedMessageQueue.GetNextMailItem(this.priority);
				if (this.routedMailItem != null)
				{
					this.readyRecipients = this.GetReadyRecipients();
					this.recipientResponses = new Queue<AckStatusAndResponse>();
					this.recipientsSent = new Queue<MailRecipient>();
					this.recipientsPending = 0;
					this.recipientEnumerator = this.readyRecipients.GetEnumerator();
					Components.QueueManager.GetQueuedRecipientsByAge().TrackEnteringSmtpSend(this.routedMailItem);
				}
			}
			return this.routedMailItem;
		}

		public virtual IReadOnlyMailItem GetNextMailItem()
		{
			return this.GetNextRoutedMailItem();
		}

		public virtual MailRecipient GetNextRecipient()
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2883988797U);
			while (this.recipientEnumerator.MoveNext())
			{
				MailRecipient mailRecipient = this.recipientEnumerator.Current;
				if (mailRecipient.Status == Status.Ready)
				{
					this.recipientsSent.Enqueue(mailRecipient);
					this.recipientsPending++;
					return mailRecipient;
				}
			}
			return null;
		}

		public virtual void NotifyConnectionFailedOver(string targetHostName, SmtpResponse failoverResponse, SessionSetupFailureReason failoverReason)
		{
		}

		public void AckConnection(AckStatus status, SmtpResponse smtpResponse, AckDetails details)
		{
			this.AckConnection(status, smtpResponse, details, null);
		}

		public void AckConnection(AckStatus status, SmtpResponse smtpResponse, AckDetails details, SessionSetupFailureReason failureReason)
		{
			this.AckConnection(MessageTrackingSource.DNS, null, status, smtpResponse, details, null, false, failureReason);
		}

		public void AckConnection(AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval)
		{
			this.AckConnection(MessageTrackingSource.DNS, status, smtpResponse, details, retryInterval);
		}

		public void AckConnection(MessageTrackingSource messageTrackingSource, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval)
		{
			this.AckConnection(messageTrackingSource, null, status, smtpResponse, details, retryInterval);
		}

		public virtual void AckConnection(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval)
		{
			this.AckConnection(messageTrackingSource, messageTrackingSourceContext, status, smtpResponse, details, retryInterval, false);
		}

		public void AckConnection(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, bool resubmitWithoutHighAvailablityRouting)
		{
			this.AckConnection(messageTrackingSource, messageTrackingSourceContext, status, smtpResponse, details, retryInterval, resubmitWithoutHighAvailablityRouting, SessionSetupFailureReason.None);
		}

		public virtual void AckConnection(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, bool resubmitWithoutHighAvailablityRouting, SessionSetupFailureReason failureReason)
		{
			this.routedMessageQueue.CloseConnection(this.priority, this.connectionId);
			this.parent.DecrementActiveConnections(this.routedMessageQueue.Key.NextHopType.DeliveryType);
			bool flag = NextHopConnection.IsSuccessNoNewConnectionResponse(smtpResponse);
			if (this.routedMailItem != null)
			{
				if (status == AckStatus.Skip)
				{
					throw new InvalidOperationException("NextHopConnection should not hold a message when the ack status is Skip");
				}
				this.AckMailItem(AckStatus.Pending, SmtpResponse.Empty, details, MessageTrackingSource.SMTP, LatencyComponent.SmtpSendConnect, false);
			}
			switch (status)
			{
			case AckStatus.Pending:
			case AckStatus.Success:
				this.routedMessageQueue.ResetConnectionRetryCount();
				if (!flag)
				{
					this.CreateConnectionIfNecessary();
					return;
				}
				break;
			case AckStatus.Retry:
				this.routedMessageQueue.Retry(this.parent.CallBackDelegate, retryInterval, smtpResponse, details);
				return;
			case AckStatus.Fail:
				if (!this.routedMessageQueue.Suspended && this.routedMessageQueue.Key.NextHopType != NextHopType.Heartbeat)
				{
					this.routedMessageQueue.NDRAllMessages(messageTrackingSource, messageTrackingSourceContext, smtpResponse, details);
					this.routedMessageQueue.LastError = smtpResponse;
				}
				break;
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
			case AckStatus.Quarantine:
				break;
			case AckStatus.Resubmit:
				this.routedMessageQueue.LastError = smtpResponse;
				if (resubmitWithoutHighAvailablityRouting)
				{
					if (!this.Key.IsLocalDeliveryGroupRelay)
					{
						throw new InvalidOperationException("resubmitWithoutHighAvailablityRouting should not be true if the next hop solution is not for high availability");
					}
					this.routedMessageQueue.Resubmit(ResubmitReason.UnreachableSameVersionHubs, null);
					return;
				}
				else
				{
					bool flag2 = this.routedMessageQueue.EvaluateResubmitDueToConfigUpdate(this.parent.CallBackDelegate);
					if (flag2)
					{
						this.routedMessageQueue.Resubmit(ResubmitReason.ConfigUpdate, null);
						return;
					}
				}
				break;
			case AckStatus.Skip:
				this.routedMessageQueue.ResetConnectionRetryCount();
				return;
			default:
				return;
			}
		}

		public virtual void CreateConnectionIfNecessary()
		{
			this.parent.CreateConnectionIfNecessary(this.routedMessageQueue, this.priority);
		}

		public virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			this.AckMailItem(ackStatus, smtpResponse, details, null, source, messageTrackingSourceContext, deliveryComponent, remoteMta, shadowed, primaryServer, reportEndToEndLatencies);
		}

		public virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			if (this.recipientsPending != 0 && ackStatus == AckStatus.Success)
			{
				throw new InvalidOperationException("Cannot ack message until all pending recipients have been acked");
			}
			DeferReason resubmitDeferReason = DeferReason.None;
			TimeSpan? resubmitDeferInterval = null;
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub && this.routedMessageQueue.Key.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox && smtpResponse.StatusText != null)
			{
				if (smtpResponse.StatusText.Length == 1 && AckReason.IsMailboxTransportDeliveryPoisonMessageResponse(smtpResponse))
				{
					this.routedMailItem.Poison();
				}
				else if (smtpResponse.StatusText.Length > 1)
				{
					SmtpResponse smtpResponse2;
					AckStatus? ackStatus2;
					TimeSpan? timeSpan;
					this.ProcessMailboxTransportDeliveryResult(smtpResponse, out smtpResponse2, out ackStatus2, out timeSpan);
					smtpResponse = smtpResponse2;
					retryInterval = (timeSpan ?? retryInterval);
					ackStatus = (ackStatus2 ?? ackStatus);
					resubmitDeferReason = DeferReason.ReroutedByStoreDriver;
					resubmitDeferInterval = new TimeSpan?(TimeSpan.FromMinutes(Components.TransportAppConfig.Resolver.DeliverMoveMailboxRetryInterval));
				}
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = this.routedMailItem.QueuedRecipientsByAgeToken;
			this.routedMessageQueue.AckMessage(this.routedMailItem, this.recipientResponses, ackStatus, smtpResponse, details, resubmitDeferReason, resubmitDeferInterval, retryInterval, source, messageTrackingSourceContext, deliveryComponent, remoteMta, this.readyRecipients, shadowed, primaryServer, reportEndToEndLatencies);
			Components.QueueManager.GetQueuedRecipientsByAge().TrackExitingSmtpSend(queuedRecipientsByAgeToken);
			this.routedMailItem = null;
			this.readyRecipients = null;
			this.recipientResponses = null;
			this.recipientsSent = null;
			this.recipientEnumerator = null;
		}

		public virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, MessageTrackingSource source, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			this.AckMailItem(ackStatus, smtpResponse, details, source, null, deliveryComponent, remoteMta, shadowed, primaryServer, reportEndToEndLatencies);
		}

		public virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, MessageTrackingSource source, LatencyComponent deliveryComponent, bool reportEndToEndLatencies)
		{
			this.AckMailItem(ackStatus, smtpResponse, details, null, source, null, deliveryComponent, reportEndToEndLatencies);
		}

		public virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, bool reportEndToEndLatencies)
		{
			this.AckMailItem(ackStatus, smtpResponse, details, retryInterval, source, messageTrackingSourceContext, deliveryComponent, null, false, string.Empty, reportEndToEndLatencies);
		}

		public virtual void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			if (this.recipientsPending <= 0)
			{
				throw new InvalidOperationException("AckRecipient called but no recipients left to ack");
			}
			this.recipientsPending--;
			if (ackStatus == AckStatus.Success)
			{
				DsnFlags dsnFlags = this.GenerateSuccessDSNs;
				if (dsnFlags != DsnFlags.None)
				{
					if (dsnFlags == DsnFlags.Relay)
					{
						ackStatus = AckStatus.Relay;
					}
				}
				else
				{
					ackStatus = AckStatus.SuccessNoDsn;
				}
			}
			this.recipientResponses.Enqueue(new AckStatusAndResponse(ackStatus, smtpResponse));
		}

		public virtual void ResetQueueLastRetryTimeAndError()
		{
			this.routedMessageQueue.LastRetryTime = DateTime.UtcNow;
			this.routedMessageQueue.LastError = SmtpResponse.Empty;
		}

		internal void GetQueueCountsOnlyForIndividualPriorities(out int[] activeCount, out int[] retryCount)
		{
			if (this.routedMessageQueue == null)
			{
				activeCount = null;
				retryCount = null;
				return;
			}
			this.routedMessageQueue.GetQueueCountsOnlyForIndividualPriorities(out activeCount, out retryCount);
		}

		private static bool IsSuccessNoNewConnectionResponse(SmtpResponse smtpResponse)
		{
			return string.Equals(smtpResponse.StatusCode, SmtpResponse.SuccessNoNewConnectionResponse.StatusCode, StringComparison.OrdinalIgnoreCase) && string.Equals(smtpResponse.EnhancedStatusCode, SmtpResponse.SuccessNoNewConnectionResponse.EnhancedStatusCode, StringComparison.OrdinalIgnoreCase) && smtpResponse.StatusText != null && smtpResponse.StatusText.Length != 0 && string.Equals(smtpResponse.StatusText[0], SmtpResponse.SuccessNoNewConnectionResponse.StatusText[0], StringComparison.OrdinalIgnoreCase);
		}

		private void ProcessMailboxTransportDeliveryResult(SmtpResponse smtpResponse, out SmtpResponse messageLevelSmtpResponse, out AckStatus? messageLevelAckStatus, out TimeSpan? messageLevelRetryInterval)
		{
			messageLevelSmtpResponse = SmtpResponse.Empty;
			messageLevelAckStatus = null;
			messageLevelRetryInterval = null;
			MailboxTransportDeliveryResult mailboxTransportDeliveryResult;
			string str;
			bool flag = MailboxTransportDeliveryResult.TryParse(smtpResponse, out mailboxTransportDeliveryResult, out str);
			if (flag)
			{
				messageLevelSmtpResponse = mailboxTransportDeliveryResult.MessageLevelSmtpResponse;
				messageLevelRetryInterval = mailboxTransportDeliveryResult.MessageLevelRetryInterval;
				if (mailboxTransportDeliveryResult.RetryQueue)
				{
					messageLevelAckStatus = new AckStatus?(AckStatus.Retry);
					this.OverrideRecipientResponses(AckStatus.Retry, messageLevelSmtpResponse);
					this.retryQueueRequested = true;
					this.retryQueueSmtpResponse = messageLevelSmtpResponse;
					return;
				}
				if (mailboxTransportDeliveryResult.MessageLevelResubmit)
				{
					messageLevelAckStatus = new AckStatus?(AckStatus.Resubmit);
					this.OverrideRecipientResponses(AckStatus.Resubmit, messageLevelSmtpResponse);
					return;
				}
				if (mailboxTransportDeliveryResult.RecipientResponseCount > 0)
				{
					int num = this.recipientResponses.Count((AckStatusAndResponse r) => r.AckStatus == AckStatus.Success);
					if (mailboxTransportDeliveryResult.RecipientResponseCount != num)
					{
						messageLevelSmtpResponse = new SmtpResponse("421", "4.4.0", new string[]
						{
							string.Format(CultureInfo.InvariantCulture, "smtp response from MailboxTransportDelivery has {0} recipients which is different from the number of successful RCPT TO's: {1}", new object[]
							{
								mailboxTransportDeliveryResult.RecipientResponseCount,
								num
							})
						});
						messageLevelAckStatus = new AckStatus?(AckStatus.Retry);
						this.OverrideRecipientResponses(AckStatus.Retry, messageLevelSmtpResponse);
						return;
					}
					this.OverrideRecipientResponses(mailboxTransportDeliveryResult.RecipientResponses);
					return;
				}
			}
			else
			{
				messageLevelSmtpResponse = new SmtpResponse("421", "4.4.0", new string[]
				{
					"failed to parse smtp response from Mailbox Transport Delivery: " + str
				});
				messageLevelAckStatus = new AckStatus?(AckStatus.Retry);
				this.OverrideRecipientResponses(AckStatus.Retry, messageLevelSmtpResponse);
			}
		}

		private void OverrideRecipientResponses(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			int count = this.recipientResponses.Count;
			this.recipientResponses = new Queue<AckStatusAndResponse>(count);
			for (int i = 0; i < count; i++)
			{
				this.recipientResponses.Enqueue(new AckStatusAndResponse(ackStatus, smtpResponse));
			}
		}

		private void OverrideRecipientResponses(IEnumerable<MailboxTransportDeliveryResult.RecipientResponse> mbxTrResponses)
		{
			if (this.recipientResponses.Count != this.recipientsSent.Count)
			{
				throw new InvalidOperationException(string.Format("recipient response count {0} is different from recipients sent count {1}", this.recipientResponses.Count, this.recipientsSent.Count));
			}
			IEnumerator<MailboxTransportDeliveryResult.RecipientResponse> enumerator = mbxTrResponses.GetEnumerator();
			IEnumerator<MailRecipient> enumerator2 = this.recipientsSent.GetEnumerator();
			IEnumerator<AckStatusAndResponse> enumerator3 = this.recipientResponses.GetEnumerator();
			Queue<AckStatusAndResponse> queue = new Queue<AckStatusAndResponse>();
			while (enumerator3.MoveNext() && enumerator2.MoveNext())
			{
				AckStatusAndResponse item = enumerator3.Current;
				MailRecipient mailRecipient = enumerator2.Current;
				if (item.AckStatus == AckStatus.Success)
				{
					enumerator.MoveNext();
					MailboxTransportDeliveryResult.RecipientResponse recipientResponse = enumerator.Current;
					queue.Enqueue(new AckStatusAndResponse(recipientResponse.AckStatus, recipientResponse.SmtpResponse));
					if (recipientResponse.RetryOnDuplicateDelivery)
					{
						mailRecipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", true);
					}
				}
				else
				{
					queue.Enqueue(item);
				}
			}
			this.recipientResponses = queue;
		}

		private IList<MailRecipient> GetReadyRecipients()
		{
			int num;
			if (this.routedMessageQueue.Key.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				num = 47;
			}
			else if (this.maxMessageRecipients > 0)
			{
				num = Math.Max(this.maxMessageRecipients, 50);
			}
			else
			{
				num = this.routedMailItem.Recipients.Count;
			}
			List<MailRecipient> list = new List<MailRecipient>(num);
			foreach (MailRecipient mailRecipient in this.routedMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Ready)
				{
					list.Add(mailRecipient);
					if (list.Count == num)
					{
						break;
					}
				}
			}
			return list;
		}

		private const int MinRecipientsBatchSize = 50;

		private long connectionId;

		private bool isConnectionAttemptSucceeded;

		private bool retryQueueRequested;

		private SmtpResponse retryQueueSmtpResponse = SmtpResponse.Empty;

		private RoutedMessageQueue routedMessageQueue;

		private int maxMessageRecipients;

		private DsnFlags generateSuccessDSNs;

		private IEnumerator<MailRecipient> recipientEnumerator;

		private Queue<AckStatusAndResponse> recipientResponses;

		private Queue<MailRecipient> recipientsSent;

		private int recipientsPending;

		private ConnectionManager parent;

		private RoutedMailItem routedMailItem;

		private IList<MailRecipient> readyRecipients;

		private DeliveryPriority priority;
	}
}
