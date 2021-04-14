using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal sealed class SubmitMessageQueue : TransportMessageQueue, ILockableQueue, IQueueQuotaObservableComponent
	{
		private SubmitMessageQueue(RoutedQueueBase queueStorage) : base(queueStorage, PriorityBehaviour.RoundRobin)
		{
		}

		public static SubmitMessageQueue Instance
		{
			get
			{
				return SubmitMessageQueue.instance;
			}
		}

		public NextHopSolutionKey Key
		{
			get
			{
				return NextHopSolutionKey.Submission;
			}
		}

		public static void CreateInstance()
		{
			if (SubmitMessageQueue.instance != null)
			{
				throw new InvalidOperationException("Submission queue already created");
			}
			RoutedQueueBase orAddQueue = Components.MessagingDatabase.GetOrAddQueue(NextHopSolutionKey.Submission);
			SubmitMessageQueue.instance = new SubmitMessageQueue(orAddQueue);
		}

		public static void LoadInstance(RoutedQueueBase queueStorage)
		{
			if (SubmitMessageQueue.instance != null)
			{
				throw new InvalidOperationException("Submission queue already created");
			}
			SubmitMessageQueue.instance = new SubmitMessageQueue(queueStorage);
		}

		public event Action<TransportMailItem> OnAcquire;

		public event Action<TransportMailItem> OnRelease;

		public override bool Suspended
		{
			set
			{
				base.Suspended = value;
				if (this.Suspended)
				{
					base.RelockAll("Queue suspended", delegate(IQueueItem item)
					{
						TransportMailItem transportMailItem = (TransportMailItem)item;
						return transportMailItem.AccessToken != null;
					});
					return;
				}
				if (base.ActiveCount > 0 || base.LockedCount > 0)
				{
					this.DataAvailable();
				}
			}
		}

		public new void Enqueue(IQueueItem item)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			if (item.DeferUntil == DateTime.MinValue)
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.SubmissionQueue, transportMailItem.LatencyTracker);
			}
			base.Enqueue(item);
		}

		public new IQueueItem Dequeue()
		{
			IQueueItem queueItem;
			if (this.conditionManager != null)
			{
				queueItem = this.conditionManager.DequeueNext();
				if (queueItem != null)
				{
					TransportMailItem transportMailItem = (TransportMailItem)queueItem;
					transportMailItem.ThrottlingContext.AddMemoryCost(new ByteQuantifiedSize((ulong)transportMailItem.MimeSize));
				}
			}
			else
			{
				queueItem = this.DequeueInternal();
			}
			return queueItem;
		}

		public ILockableItem DequeueInternal()
		{
			TransportMailItem transportMailItem = (TransportMailItem)base.Dequeue();
			if (transportMailItem != null)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.SubmissionQueue, transportMailItem.LatencyTracker);
				this.InternalOnDequeue(transportMailItem, true);
			}
			return transportMailItem;
		}

		public ILockableItem DequeueInternal(DeliveryPriority priority)
		{
			throw new InvalidOperationException("Submission queue does not support dequeuing by specifying priority.");
		}

		public void Lock(ILockableItem item, WaitCondition condition, string lockReason, int dehydrateThreshold)
		{
			item.LockExpirationTime = DateTime.UtcNow + Components.TransportAppConfig.ThrottlingConfig.LockExpirationInterval;
			base.Lock(item, condition, lockReason, dehydrateThreshold);
		}

		public TransportMailItem GetMailItemById(long mailItemId)
		{
			TransportMailItem mailItem = null;
			base.DequeueItem(delegate(IQueueItem item)
			{
				TransportMailItem transportMailItem = (TransportMailItem)item;
				if (transportMailItem != null && transportMailItem.RecordId == mailItemId)
				{
					mailItem = transportMailItem;
					return DequeueMatchResult.Break;
				}
				return DequeueMatchResult.Continue;
			}, false);
			return mailItem;
		}

		public bool SuspendMailItem(long internalMessageId)
		{
			TransportMailItem transportMailItem = this.DequeueTransportMailItem(internalMessageId, false);
			if (transportMailItem != null)
			{
				transportMailItem.Suspend();
				SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
				base.Enqueue(transportMailItem);
				ExTraceGlobals.QueuingTracer.TraceDebug<long>(0L, "Submission queue message {0} has been frozen by the admin.", internalMessageId);
			}
			return transportMailItem != null;
		}

		public bool ResumeMailItem(long internalMessageId)
		{
			TransportMailItem transportMailItem = this.DequeueTransportMailItem(internalMessageId, true);
			if (transportMailItem != null)
			{
				transportMailItem.Resume();
				base.Enqueue(transportMailItem);
				ExTraceGlobals.QueuingTracer.TraceDebug<long>(0L, "Submission queue message {0} has been unfrozen by the admin.", internalMessageId);
			}
			return transportMailItem != null;
		}

		public bool DeleteMailItem(long internalMessageId, bool withNDR)
		{
			TransportMailItem transportMailItem = this.DequeueTransportMailItem(internalMessageId, false);
			if (transportMailItem != null)
			{
				if (withNDR)
				{
					if (transportMailItem.ADRecipientCache == null)
					{
						ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(transportMailItem);
						if (!adoperationResult.Succeeded)
						{
							MultiTenantTransport.TraceAttributionError(string.Format("Error {0} when creating recipient cache for message {1}. Falling back to first org", adoperationResult.Exception, MultiTenantTransport.ToString(transportMailItem)), new object[0]);
							MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(transportMailItem, OrganizationId.ForestWideOrgId);
						}
					}
					MailRecipient mailRecipient = transportMailItem.Recipients[0];
					mailRecipient.DsnNeeded = DsnFlags.Failure;
					Components.DsnGenerator.GenerateDSNs(transportMailItem, transportMailItem.Recipients);
				}
				transportMailItem.Ack(AckStatus.Fail, AckReason.MessageDeletedByAdmin, transportMailItem.Recipients, null);
				MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.ADMIN, transportMailItem, transportMailItem.Recipients, null);
				SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
				transportMailItem.ReleaseFromActive();
				transportMailItem.CommitLazy();
				ExTraceGlobals.QueuingTracer.TraceDebug<long, bool>(0L, "Submission queue message {0} has been deleted by the admin, NDR={1}", internalMessageId, withNDR);
			}
			return transportMailItem != null;
		}

		public bool ReadMessageBody(long internalMessageId, byte[] buffer, int position, int count, out int bytesRead, out bool foundNotSuspended)
		{
			bytesRead = 0;
			TransportMailItem transportMailItem = this.DequeueTransportMailItem(internalMessageId, true, true, out foundNotSuspended);
			if (foundNotSuspended)
			{
				return false;
			}
			if (transportMailItem != null)
			{
				Stream stream;
				if (ExportStream.TryCreate(transportMailItem, transportMailItem.Recipients, false, out stream))
				{
					using (stream)
					{
						stream.Position = (long)position;
						bytesRead = stream.Read(buffer, 0, count);
					}
				}
				base.Enqueue(transportMailItem);
				if (bytesRead > 0)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<int, long>(0L, "Exported {0} bytes of message {1} read by the admin.", bytesRead, internalMessageId);
				}
			}
			return transportMailItem != null;
		}

		public bool UpdateMailItem(long internalMessageId, ExtensibleMessageInfo properties, out bool errorNotSuspended)
		{
			errorNotSuspended = false;
			TransportMailItem transportMailItem = this.DequeueTransportMailItem(internalMessageId, false);
			if (transportMailItem == null)
			{
				return false;
			}
			bool result;
			try
			{
				if (transportMailItem.Recipients[0].AdminActionStatus != AdminActionStatus.SuspendedInSubmissionQueue && !this.Suspended)
				{
					errorNotSuspended = true;
					result = false;
				}
				else
				{
					if (properties.OutboundIPPool > 0)
					{
						foreach (MailRecipient mailRecipient in transportMailItem.Recipients.AllUnprocessed)
						{
							if (properties.OutboundIPPool != mailRecipient.OutboundIPPool)
							{
								mailRecipient.OutboundIPPool = properties.OutboundIPPool;
							}
						}
						ExTraceGlobals.QueuingTracer.TraceDebug<long>(0L, "Submission queue: properties of message {0} have been updated by the admin.", internalMessageId);
					}
					result = true;
				}
			}
			finally
			{
				base.Enqueue(transportMailItem);
			}
			return result;
		}

		public bool VisitMailItems(Func<TransportMailItem, bool> visitor)
		{
			bool visitedAll = true;
			base.DequeueItem(delegate(IQueueItem item)
			{
				if (!visitor((TransportMailItem)item))
				{
					visitedAll = false;
					return DequeueMatchResult.Break;
				}
				return DequeueMatchResult.Continue;
			}, false);
			return visitedAll;
		}

		public new void TimedUpdate()
		{
			base.UpdateQueueRates();
			base.TimedUpdate();
		}

		public override bool IsInterestingQueueToLog()
		{
			return base.IsInterestingQueueToLog() || base.TotalCount > 0;
		}

		internal void SetConditionManager(SingleQueueWaitConditionManager conditionManager)
		{
			if (this.conditionManager != null)
			{
				throw new InvalidOperationException("Overwriting existing condition map");
			}
			this.conditionManager = conditionManager;
		}

		protected override void DataAvailable()
		{
			if (Components.IsActive)
			{
				Components.CategorizerComponent.DataAvail();
			}
		}

		protected override void ItemEnqueued(IQueueItem item)
		{
			this.InternalOnEnqueue((TransportMailItem)item);
		}

		protected override void ItemExpired(IQueueItem item, bool wasEnqueued)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Message with ID {0} has expired in the submission queue.", transportMailItem.RecordId);
			if (transportMailItem.ADRecipientCache == null)
			{
				ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(transportMailItem);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError(string.Format("Error {0} when creating recipient cache for message {1}. Falling back to first org", adoperationResult.Exception, MultiTenantTransport.ToString(transportMailItem)), new object[0]);
					MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(transportMailItem, OrganizationId.ForestWideOrgId);
				}
			}
			this.ItemRemoved(item, wasEnqueued);
			CategorizerComponent.AckAllRecipients(transportMailItem, AckStatus.Fail, AckReason.MessageExpired);
			Components.OrarGenerator.GenerateOrarMessage(transportMailItem, true);
			Components.DsnGenerator.GenerateDSNs(transportMailItem);
			LatencyFormatter latencyFormatter = new LatencyFormatter(transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.QUEUE, "Queue=Submission", transportMailItem, transportMailItem.Recipients, null, SmtpResponse.Empty, latencyFormatter);
			transportMailItem.ReleaseFromActiveMaterializedLazy();
			Components.QueueManager.UpdatePerfCountersOnExpireFromSubmissionQueue(transportMailItem);
		}

		protected override void ItemLockExpired(IQueueItem item)
		{
			this.ItemRemoved(item);
			Components.QueueManager.UpdatePerfCountersOnLockExpiredInSubmissionQueue();
		}

		protected override bool ItemDeferred(IQueueItem item)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			Components.ShadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemDeferred(transportMailItem, this, item.DeferUntil);
			SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
			this.InternalOnEnqueue(transportMailItem);
			item.Update();
			return true;
		}

		protected override bool ItemActivated(IQueueItem item)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "Message with ID {0} has been activated in the submission queue.", transportMailItem.RecordId);
			TransportMailItem transportMailItem2 = item as TransportMailItem;
			if (transportMailItem2 != null && transportMailItem2.DeferReason != DeferReason.None)
			{
				LatencyTracker.EndTrackLatency(TransportMailItem.GetDeferLatencyComponent(transportMailItem2.DeferReason), transportMailItem2.LatencyTracker);
			}
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Retry)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
			LatencyTracker.BeginTrackLatency(LatencyComponent.SubmissionQueue, transportMailItem.LatencyTracker);
			this.InternalOnDequeue(item, true);
			return true;
		}

		protected override bool ItemLocked(IQueueItem item, WaitCondition condition, string lockReason)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			LatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerLocking, transportMailItem.LatencyTracker);
			transportMailItem.LockReason = lockReason;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Ready)
				{
					mailRecipient.Status = Status.Locked;
				}
			}
			SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
			this.InternalOnEnqueue(transportMailItem);
			return true;
		}

		protected override bool ItemUnlocked(IQueueItem item, AccessToken token)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			transportMailItem.AccessToken = token;
			transportMailItem.LockReason = null;
			transportMailItem.LockExpirationTime = DateTimeOffset.MinValue;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Locked)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
			LatencyTracker.EndTrackLatency(LatencyComponent.CategorizerLocking, transportMailItem.LatencyTracker);
			return true;
		}

		protected override void ItemRelocked(IQueueItem item, string lockReason, out WaitCondition condition)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			LatencyTracker.BeginTrackLatency(LatencyComponent.CategorizerLocking, transportMailItem.LatencyTracker);
			SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
			transportMailItem.LockReason = lockReason;
			transportMailItem.LockExpirationTime = DateTime.UtcNow + Components.TransportAppConfig.ThrottlingConfig.LockExpirationInterval;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Ready)
				{
					mailRecipient.Status = Status.Locked;
				}
			}
			this.conditionManager.AddToWaitlist(transportMailItem.CurrentCondition);
			condition = transportMailItem.CurrentCondition;
		}

		protected override void ItemRemoved(IQueueItem item)
		{
			this.ItemRemoved(item, true);
		}

		protected override void ItemDehydrated(IQueueItem item)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			transportMailItem.CommitLazyAndDehydrateMessageIfPossible(Breadcrumb.DehydrateOnMailItemLocked);
		}

		private void ItemRemoved(IQueueItem item, bool wasEnqueued)
		{
			TransportMailItem transportMailItem = (TransportMailItem)item;
			if (transportMailItem.AccessToken != null)
			{
				SubmitMessageQueue.ReturnTokenIfPresent(transportMailItem);
			}
			else if (transportMailItem.CurrentCondition != null)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.CategorizerLocking, transportMailItem.LatencyTracker);
				this.conditionManager.CleanupItem(transportMailItem.CurrentCondition);
			}
			else
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.SubmissionQueue, transportMailItem.LatencyTracker);
			}
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Locked)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
			if (wasEnqueued)
			{
				this.InternalOnDequeue(transportMailItem, false);
			}
			transportMailItem.CurrentCondition = null;
		}

		private void InternalOnEnqueue(TransportMailItem item)
		{
			Interlocked.Increment(ref this.lastIncomingMessageCount);
			item.ThrottlingContext = null;
			if (this.OnAcquire != null)
			{
				this.OnAcquire(item);
			}
		}

		private void InternalOnDequeue(IQueueItem item, bool clearDeferReason = true)
		{
			Interlocked.Increment(ref this.lastOutgoingMessageCount);
			TransportMailItem transportMailItem = (TransportMailItem)item;
			if (clearDeferReason)
			{
				transportMailItem.DeferReason = DeferReason.None;
			}
			if (this.OnRelease != null)
			{
				this.OnRelease(transportMailItem);
			}
		}

		private static void ReturnTokenIfPresent(TransportMailItem mailItem)
		{
			if (mailItem.AccessToken != null)
			{
				mailItem.AccessToken.Return(true);
				mailItem.AccessToken = null;
			}
		}

		private TransportMailItem DequeueTransportMailItem(long internalMessageId, bool deferredQueueFirst)
		{
			bool flag;
			return this.DequeueTransportMailItem(internalMessageId, false, deferredQueueFirst, out flag);
		}

		private TransportMailItem DequeueTransportMailItem(long internalMessageId, bool dequeueSuspendedOnly, bool deferredQueueFirst, out bool foundNotSuspended)
		{
			bool found = false;
			IQueueItem queueItem = base.DequeueItem(delegate(IQueueItem item)
			{
				TransportMailItem transportMailItem = (TransportMailItem)item;
				if (transportMailItem == null || transportMailItem.RecordId != internalMessageId)
				{
					return DequeueMatchResult.Continue;
				}
				if (dequeueSuspendedOnly && transportMailItem.Recipients[0].AdminActionStatus != AdminActionStatus.SuspendedInSubmissionQueue)
				{
					found = true;
					return DequeueMatchResult.Break;
				}
				return DequeueMatchResult.DequeueAndBreak;
			}, deferredQueueFirst);
			foundNotSuspended = found;
			return (TransportMailItem)queueItem;
		}

		private static SubmitMessageQueue instance;

		private SingleQueueWaitConditionManager conditionManager;
	}
}
