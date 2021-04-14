using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal sealed class MessageInfoFactory
	{
		internal MessageInfoFactory(bool includeRecipientInfo, bool includeComponentLatencyInfo)
		{
			this.includeRecipientInfo = includeRecipientInfo;
			this.includeComponentLatencyInfo = includeComponentLatencyInfo;
			this.utcTicks = DateTime.UtcNow.Ticks;
			if (this.includeComponentLatencyInfo)
			{
				this.latencyTrackerTicks = Stopwatch.GetTimestamp();
			}
			this.queuesMap = new Dictionary<long, QueueIdentity>();
		}

		internal ExtensibleMessageInfo NewMessageInfo(TransportMailItem mailItem, NextHopSolution nextHopSolution, ExtensibleMessageInfo messageInfoToRecycle)
		{
			if (!mailItem.IsActive)
			{
				return null;
			}
			if (nextHopSolution.IsInactive)
			{
				return null;
			}
			ExtensibleMessageInfo extensibleMessageInfo;
			if (nextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType == DeliveryType.Unreachable)
			{
				extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, mailItem.RecordId, QueueIdentity.UnreachableQueueIdentity);
				if (nextHopSolution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					extensibleMessageInfo.Status = MessageStatus.Suspended;
				}
				else
				{
					extensibleMessageInfo.Status = MessageStatus.Ready;
				}
				extensibleMessageInfo.LastErrorCode = (int)((UnreachableSolution)nextHopSolution).Reasons;
				this.AddRecipientInfoIfNecessary(extensibleMessageInfo, nextHopSolution.Recipients);
			}
			else
			{
				RoutedMessageQueue queue = Components.RemoteDeliveryComponent.GetQueue(nextHopSolution.NextHopSolutionKey);
				if (queue == null)
				{
					return null;
				}
				extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, mailItem.RecordId, this.GetQueueIdentity(QueueType.Delivery, queue.Id, queue.Key.NextHopDomain));
				this.SetSolutionStatus(extensibleMessageInfo, nextHopSolution);
				if (extensibleMessageInfo.Status == MessageStatus.None)
				{
					return null;
				}
			}
			this.SetBasicProperties(extensibleMessageInfo, mailItem);
			this.AddComponentLatencyInfoIfNecessary(extensibleMessageInfo, mailItem);
			return extensibleMessageInfo;
		}

		internal ExtensibleMessageInfo NewShadowMessageInfo(TransportMailItem mailItem, NextHopSolution nextHopSolution, ExtensibleMessageInfo messageInfoToRecycle)
		{
			if (nextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType != DeliveryType.ShadowRedundancy)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "nextHopSolution can only have a delivery type of shadow redundancy, this one has delivery type '{0}'", new object[]
				{
					nextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType
				}));
			}
			if (mailItem.IsRowDeleted)
			{
				return null;
			}
			ShadowMessageQueue queue = Components.ShadowRedundancyComponent.ShadowRedundancyManager.GetQueue(nextHopSolution.NextHopSolutionKey);
			if (queue == null)
			{
				return null;
			}
			ExtensibleMessageInfo extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, mailItem.RecordId, this.GetQueueIdentity(QueueType.Shadow, queue.Id, queue.Key.NextHopDomain));
			this.SetBasicProperties(extensibleMessageInfo, mailItem);
			switch (nextHopSolution.AdminActionStatus)
			{
			case AdminActionStatus.Suspended:
				extensibleMessageInfo.Status = MessageStatus.Suspended;
				goto IL_F5;
			case AdminActionStatus.PendingDeleteWithNDR:
				throw new InvalidOperationException("AdminActionStatus for Shadow NextHopSolutions shouldn't be set to PendingDeleteWithNDR.");
			case AdminActionStatus.PendingDeleteWithOutNDR:
				extensibleMessageInfo.Status = MessageStatus.PendingRemove;
				goto IL_F5;
			}
			extensibleMessageInfo.Status = MessageStatus.Ready;
			IL_F5:
			this.AddRecipientInfoIfNecessary(extensibleMessageInfo, nextHopSolution.Recipients);
			this.AddComponentLatencyInfoIfNecessary(extensibleMessageInfo, mailItem);
			return extensibleMessageInfo;
		}

		internal ExtensibleMessageInfo NewPoisonMessageInfo(TransportMailItem mailItem, ExtensibleMessageInfo messageInfoToRecycle)
		{
			ExtensibleMessageInfo extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, mailItem.RecordId, QueueIdentity.PoisonQueueIdentity);
			this.SetBasicProperties(extensibleMessageInfo, mailItem);
			extensibleMessageInfo.Status = MessageStatus.Suspended;
			this.UnsafeAddRecipientInfoIfNecessary(extensibleMessageInfo, mailItem.Recipients);
			this.AddComponentLatencyInfoIfNecessary(extensibleMessageInfo, mailItem);
			return extensibleMessageInfo;
		}

		internal ExtensibleMessageInfo NewMessageDepotItemMessageInfo(IMessageDepotItemWrapper itemWrapper, ExtensibleMessageInfo messageInfoToRecycle)
		{
			TransportMailItem transportMailItem = (TransportMailItem)itemWrapper.Item.MessageObject;
			QueueIdentity queueIdentity = QueueIdentity.SubmissionQueueIdentity;
			if (transportMailItem.IsPoison)
			{
				queueIdentity = QueueIdentity.PoisonQueueIdentity;
			}
			ExtensibleMessageInfo extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, transportMailItem.RecordId, queueIdentity);
			this.SetBasicProperties(extensibleMessageInfo, transportMailItem);
			extensibleMessageInfo.LockReason = transportMailItem.LockReason;
			switch (itemWrapper.State)
			{
			case MessageDepotItemState.Ready:
				extensibleMessageInfo.Status = MessageStatus.Ready;
				goto IL_9B;
			case MessageDepotItemState.Suspended:
				extensibleMessageInfo.Status = MessageStatus.Suspended;
				goto IL_9B;
			case MessageDepotItemState.Processing:
				extensibleMessageInfo.Status = MessageStatus.Active;
				goto IL_9B;
			case MessageDepotItemState.Expiring:
				extensibleMessageInfo.Status = MessageStatus.PendingRemove;
				goto IL_9B;
			}
			extensibleMessageInfo.Status = MessageStatus.None;
			IL_9B:
			ICollection<MailRecipient> recipients = transportMailItem.Recipients;
			if (recipients != null)
			{
				this.AddRecipientInfoIfNecessary(extensibleMessageInfo, recipients);
			}
			this.AddComponentLatencyInfoIfNecessary(extensibleMessageInfo, transportMailItem);
			return extensibleMessageInfo;
		}

		internal ExtensibleMessageInfo NewCategorizerMessageInfo(CategorizerItem categorizerItem, ExtensibleMessageInfo messageInfoToRecycle)
		{
			TransportMailItem transportMailItem = categorizerItem.TransportMailItem;
			ExtensibleMessageInfo extensibleMessageInfo = MessageInfoFactory.CreateOrRecycleMessageInfo(messageInfoToRecycle, transportMailItem.RecordId, QueueIdentity.SubmissionQueueIdentity);
			this.SetBasicProperties(extensibleMessageInfo, transportMailItem);
			extensibleMessageInfo.LockReason = transportMailItem.LockReason;
			MailRecipient mailRecipient = null;
			ICollection<MailRecipient> collection;
			if (categorizerItem.Stage != -1)
			{
				IList<MailRecipient> list = MessageInfoFactory.CloneUnsafeRecipientCollection(transportMailItem.Recipients);
				if (list != null && list.Count > 0)
				{
					mailRecipient = list[0];
				}
				collection = list;
			}
			else
			{
				collection = transportMailItem.Recipients;
				if (transportMailItem.Recipients.Count > 0)
				{
					mailRecipient = transportMailItem.Recipients[0];
				}
			}
			if (categorizerItem.Stage == -1 || categorizerItem.Stage == -2)
			{
				if (mailRecipient != null && mailRecipient.AdminActionStatus == AdminActionStatus.SuspendedInSubmissionQueue)
				{
					extensibleMessageInfo.Status = MessageStatus.Suspended;
				}
				else if (transportMailItem.DeferUntil != DateTime.MinValue)
				{
					extensibleMessageInfo.Status = MessageStatus.Retry;
					extensibleMessageInfo.LastErrorCode = (int)transportMailItem.DeferReason;
					if (collection != null)
					{
						extensibleMessageInfo.LastError = MessageInfoFactory.GetFirstRecipientResponse(collection);
					}
				}
				else if (transportMailItem.Status == Status.Locked)
				{
					extensibleMessageInfo.Status = MessageStatus.Locked;
				}
				else
				{
					extensibleMessageInfo.Status = MessageStatus.Ready;
				}
			}
			else
			{
				extensibleMessageInfo.Status = MessageStatus.Active;
			}
			if (collection != null)
			{
				this.AddRecipientInfoIfNecessary(extensibleMessageInfo, collection);
			}
			this.AddComponentLatencyInfoIfNecessary(extensibleMessageInfo, transportMailItem);
			return extensibleMessageInfo;
		}

		private static string GetFirstRecipientResponse(IEnumerable<MailRecipient> recipients)
		{
			foreach (MailRecipient mailRecipient in recipients)
			{
				if (!mailRecipient.SmtpResponse.Equals(SmtpResponse.Empty))
				{
					return mailRecipient.GetLastErrorDetails();
				}
			}
			return null;
		}

		private static RecipientInfo NewRecipientInfo(MailRecipient recip)
		{
			RecipientInfo recipientInfo = new RecipientInfo();
			recipientInfo.Address = recip.Email.ToString();
			recipientInfo.Type = recip.Type;
			recipientInfo.FinalDestination = recip.FinalDestination;
			recipientInfo.LastErrorCode = (int)recip.UnreachableReason;
			recipientInfo.OutboundIPPool = recip.OutboundIPPool;
			if (recipientInfo.LastErrorCode == 0)
			{
				recipientInfo.LastError = recip.GetLastErrorDetails();
			}
			switch (recip.Status)
			{
			case Status.Ready:
				recipientInfo.Status = RecipientStatus.Ready;
				break;
			case Status.Retry:
				recipientInfo.Status = RecipientStatus.Retry;
				break;
			case Status.Handled:
			case Status.Complete:
				recipientInfo.Status = RecipientStatus.Complete;
				break;
			case Status.Locked:
				recipientInfo.Status = RecipientStatus.Locked;
				break;
			default:
				throw new InvalidOperationException("Unexpected status");
			}
			return recipientInfo;
		}

		private static ExtensibleMessageInfo CreateOrRecycleMessageInfo(ExtensibleMessageInfo messageInfoToRecycle, long mailItemId, QueueIdentity queueIdentity)
		{
			ExtensibleMessageInfo result;
			if (messageInfoToRecycle == null)
			{
				result = new PropertyBagBasedMessageInfo(mailItemId, queueIdentity);
			}
			else
			{
				messageInfoToRecycle.Reset(mailItemId, queueIdentity);
				result = messageInfoToRecycle;
			}
			return result;
		}

		private static IList<MailRecipient> CloneUnsafeRecipientCollection(ICollection<MailRecipient> recipients)
		{
			List<MailRecipient> list = new List<MailRecipient>(recipients.Count);
			try
			{
				foreach (MailRecipient item in recipients)
				{
					list.Add(item);
				}
			}
			catch (InvalidOperationException arg)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<InvalidOperationException>(0L, "Recipient collection being iterated by get-message while message is being changed by cat - not an error ({0})", arg);
				return null;
			}
			return list;
		}

		private QueueIdentity GetQueueIdentity(QueueType queueType, long queueId, string nextHopDomain)
		{
			QueueIdentity queueIdentity;
			if (!this.queuesMap.TryGetValue(queueId, out queueIdentity))
			{
				queueIdentity = new QueueIdentity(queueType, queueId, nextHopDomain);
				this.queuesMap.Add(queueId, queueIdentity);
			}
			return queueIdentity;
		}

		private void SetSolutionStatus(ExtensibleMessageInfo messageInfo, NextHopSolution solution)
		{
			if (this.includeRecipientInfo)
			{
				messageInfo.Recipients = new RecipientInfo[solution.Recipients.Count];
			}
			bool flag = false;
			int num = 0;
			foreach (MailRecipient mailRecipient in solution.Recipients)
			{
				if (this.includeRecipientInfo)
				{
					messageInfo.Recipients[num++] = MessageInfoFactory.NewRecipientInfo(mailRecipient);
				}
				if (!flag && (mailRecipient.Status == Status.Ready || mailRecipient.Status == Status.Retry || mailRecipient.Status == Status.Locked))
				{
					messageInfo.RetryCount = mailRecipient.RetryCount;
					messageInfo.LastError = mailRecipient.SmtpResponse.ToString();
					flag = true;
					switch (mailRecipient.Status)
					{
					case Status.Ready:
						messageInfo.Status = MessageStatus.Ready;
						break;
					case Status.Retry:
						messageInfo.Status = MessageStatus.Retry;
						break;
					case Status.Locked:
						messageInfo.Status = MessageStatus.Locked;
						messageInfo.LockReason = solution.LockReason;
						break;
					}
					if (!this.includeRecipientInfo)
					{
						break;
					}
				}
			}
			switch (solution.AdminActionStatus)
			{
			case AdminActionStatus.None:
				if (solution.DeliveryStatus == DeliveryStatus.InDelivery)
				{
					messageInfo.Status = MessageStatus.Active;
					messageInfo.LastError = null;
					return;
				}
				return;
			case AdminActionStatus.Suspended:
				if (solution.DeliveryStatus == DeliveryStatus.InDelivery)
				{
					messageInfo.Status = MessageStatus.PendingSuspend;
					return;
				}
				messageInfo.Status = MessageStatus.Suspended;
				return;
			case AdminActionStatus.PendingDeleteWithNDR:
			case AdminActionStatus.PendingDeleteWithOutNDR:
				if (solution.DeliveryStatus == DeliveryStatus.InDelivery)
				{
					messageInfo.Status = MessageStatus.PendingRemove;
					return;
				}
				messageInfo.Status = MessageStatus.None;
				return;
			default:
				throw new ArgumentException("Unexpected AdminActionStatus value in solution", "solution.AdminActionStatus");
			}
		}

		private void SetBasicProperties(ExtensibleMessageInfo messageInfo, TransportMailItem mailItem)
		{
			messageInfo.Subject = mailItem.Subject;
			messageInfo.InternetMessageId = mailItem.InternetMessageId;
			messageInfo.FromAddress = (string)mailItem.From;
			messageInfo.OriginalFromAddress = mailItem.OriginalFrom.ToString();
			messageInfo.Size = ByteQuantifiedSize.FromBytes((ulong)mailItem.MimeSize);
			messageInfo.MessageSourceName = mailItem.ReceiveConnectorName;
			messageInfo.SourceIP = mailItem.SourceIPAddress;
			messageInfo.DateReceived = mailItem.DateReceived;
			messageInfo.MessageLatency = EnhancedTimeSpan.FromTicks(this.utcTicks - mailItem.DateReceived.Ticks);
			messageInfo.DeferReason = this.GetDeferReasonString(mailItem.DeferReason);
			messageInfo.Priority = this.GetPriorityString(((IQueueItem)mailItem).Priority);
			messageInfo.ExternalDirectoryOrganizationId = mailItem.ExternalOrganizationId;
			messageInfo.Directionality = mailItem.Directionality;
			messageInfo.AccountForest = mailItem.ExoAccountForest;
			int scl = mailItem.Scl;
			messageInfo.SCL = ((scl != -2) ? scl : 0);
			DateTime expiry = ((IQueueItem)mailItem).Expiry;
			if (expiry != DateTime.MaxValue)
			{
				messageInfo.ExpirationTime = new DateTime?(expiry);
			}
			messageInfo.IsProbeMessage = mailItem.IsProbe;
		}

		private string GetDeferReasonString(DeferReason deferReason)
		{
			switch (deferReason)
			{
			case DeferReason.None:
				return "None";
			case DeferReason.ADTransientFailureDuringResolve:
				return "AD Transient Failure During Resolve";
			case DeferReason.ADTransientFailureDuringContentConversion:
				return "AD Transient Failure During Content Conversion";
			case DeferReason.Agent:
				return "Agent";
			case DeferReason.LoopDetected:
				return "Loop Detected";
			case DeferReason.ReroutedByStoreDriver:
				return "Rerouted By Store Driver";
			case DeferReason.StorageTransientFailureDuringContentConversion:
				return "Storage Transient Failure During Content Conversion";
			case DeferReason.MarkedAsRetryDeliveryIfRejected:
				return "Marked As Retry Delivery If Rejected";
			case DeferReason.TransientFailure:
				return "Transient Failure";
			case DeferReason.AmbiguousRecipient:
				return "Ambiguous Recipient";
			case DeferReason.TargetSiteInboundMailDisabled:
				return "Target Site Inbound Mail Disabled";
			case DeferReason.RecipientThreadLimitExceeded:
				return "Recipient Thread Limit Exceeded";
			case DeferReason.TransientAttributionFailure:
				return "Transient Attribution Failure";
			case DeferReason.TransientAcceptedDomainsLoadFailure:
				return "Transient Accepted Domains Load Failure";
			case DeferReason.RecipientHasNoMdb:
				return "Recipient does not have a mailbox database";
			case DeferReason.ConfigUpdate:
				return "Config Update";
			}
			throw new NotImplementedException("Add new enum case to GetDeferReasonString()");
		}

		private string GetPriorityString(DeliveryPriority deliveryPriority)
		{
			switch (deliveryPriority)
			{
			case DeliveryPriority.High:
				return "High";
			case DeliveryPriority.Normal:
				return "Normal";
			case DeliveryPriority.Low:
				return "Low";
			case DeliveryPriority.None:
				return "None";
			default:
				throw new NotImplementedException("Add new enum case to GetPriorityString()");
			}
		}

		private void AddRecipientInfoIfNecessary(ExtensibleMessageInfo messageInfo, ICollection<MailRecipient> recipients)
		{
			if (!this.includeRecipientInfo)
			{
				return;
			}
			int num = 0;
			RecipientInfo[] array = new RecipientInfo[recipients.Count];
			foreach (MailRecipient recip in recipients)
			{
				array[num++] = MessageInfoFactory.NewRecipientInfo(recip);
			}
			messageInfo.Recipients = array;
		}

		private void UnsafeAddRecipientInfoIfNecessary(ExtensibleMessageInfo messageInfo, ICollection<MailRecipient> recipients)
		{
			if (!this.includeRecipientInfo)
			{
				return;
			}
			IList<MailRecipient> recipients2 = MessageInfoFactory.CloneUnsafeRecipientCollection(recipients);
			this.AddRecipientInfoIfNecessary(messageInfo, recipients2);
		}

		private void AddComponentLatencyInfoIfNecessary(ExtensibleMessageInfo messageInfo, TransportMailItem mailItem)
		{
			if (!this.includeComponentLatencyInfo)
			{
				return;
			}
			LatencyTracker latencyTracker = LatencyTracker.Clone(mailItem.LatencyTracker);
			if (latencyTracker == null)
			{
				return;
			}
			List<ComponentLatencyInfo> list = new List<ComponentLatencyInfo>();
			int num = 0;
			foreach (LatencyRecord latencyRecord in latencyTracker.GetCompletedRecords())
			{
				list.Add(new ComponentLatencyInfo(LatencyTracker.GetFullName(latencyRecord.ComponentShortName), latencyRecord.Latency, num, false));
				num++;
			}
			foreach (PendingLatencyRecord pendingLatencyRecord in latencyTracker.GetPendingRecords())
			{
				list.Add(new ComponentLatencyInfo(LatencyTracker.GetFullName(LatencyTracker.GetShortName(pendingLatencyRecord.ComponentId)), pendingLatencyRecord.CalculatePendingLatency(this.latencyTrackerTicks), num, false));
				num++;
			}
			messageInfo.ComponentLatency = list.ToArray();
		}

		private readonly long utcTicks;

		private readonly long latencyTrackerTicks;

		private readonly bool includeRecipientInfo;

		private readonly bool includeComponentLatencyInfo;

		private Dictionary<long, QueueIdentity> queuesMap;
	}
}
