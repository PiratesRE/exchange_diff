using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MsgDepotLegacyPerfCounterWrapper
	{
		public MsgDepotLegacyPerfCounterWrapper(IMessageDepot messageDepot, IMessageDepotQueueViewer messageDepotQueueViewer, TransportAppConfig.ILegacyQueueConfig queueConfig)
		{
			ArgumentValidator.ThrowIfNull("messageDepot", messageDepot);
			ArgumentValidator.ThrowIfNull("messageDepotQueueViewer", messageDepotQueueViewer);
			this.messageDepotQueueViewer = messageDepotQueueViewer;
			this.perfCountersInstance = QueuingPerfCounters.GetInstance("_Total");
			this.messagesSubmittedRecently = new SlidingTotalCounter(queueConfig.RecentPerfCounterTrackingInterval, queueConfig.RecentPerfCounterTrackingBucketSize);
			this.queuedRecipientsByAge = new QueuedRecipientsByAgePerfCountersWrapper(queueConfig.QueuedRecipientsByAgeTrackingEnabled);
			messageDepot.SubscribeToAddEvent(MessageDepotItemStage.Submission, new MessageEventHandler(this.OnMessageAdded));
			messageDepot.SubscribeToActivatedEvent(MessageDepotItemStage.Submission, new MessageActivatedEventHandler(this.OnMessageActivated));
			messageDepot.SubscribeToDeactivatedEvent(MessageDepotItemStage.Submission, new MessageDeactivatedEventHandler(this.OnMessageDeactivated));
			messageDepot.SubscribeToRemovedEvent(MessageDepotItemStage.Submission, new MessageRemovedEventHandler(this.OnMessageRemoved));
		}

		public void TimedUpdate()
		{
			this.perfCountersInstance.MessagesSubmittedRecently.RawValue = this.messagesSubmittedRecently.Sum;
		}

		private void OnMessageAdded(MessageEventArgs args)
		{
			ArgumentValidator.ThrowIfNull("args", args);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper", args.ItemWrapper);
			if (args.ItemWrapper.State == MessageDepotItemState.Poisoned)
			{
				this.perfCountersInstance.PoisonQueueLength.Increment();
			}
		}

		private void OnMessageRemoved(MessageRemovedEventArgs args)
		{
			ArgumentValidator.ThrowIfNull("args", args);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper", args.ItemWrapper);
			if (args.ItemWrapper.State == MessageDepotItemState.Poisoned)
			{
				this.perfCountersInstance.PoisonQueueLength.Decrement();
			}
			if (args.Reason == MessageRemovalReason.Expired)
			{
				this.perfCountersInstance.SubmissionQueueItemsExpiredTotal.Increment();
			}
		}

		private void OnMessageActivated(MessageActivatedEventArgs args)
		{
			ArgumentValidator.ThrowIfNull("args", args);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper", args.ItemWrapper);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper.Item", args.ItemWrapper.Item);
			IMessageDepotItem item = args.ItemWrapper.Item;
			this.perfCountersInstance.MessagesSubmittedTotal.Increment();
			this.perfCountersInstance.SubmissionQueueLength.Increment();
			this.perfCountersInstance.PoisonQueueLength.RawValue = this.messageDepotQueueViewer.GetCount(MessageDepotItemStage.Submission, MessageDepotItemState.Poisoned);
			this.messagesSubmittedRecently.AddValue(1L);
			this.perfCountersInstance.MessagesSubmittedRecently.RawValue = this.messagesSubmittedRecently.Sum;
			this.queuedRecipientsByAge.TrackEnteringSubmissionQueue((TransportMailItem)item.MessageObject);
		}

		private void OnMessageDeactivated(MessageDeactivatedEventArgs args)
		{
			ArgumentValidator.ThrowIfNull("args", args);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper", args.ItemWrapper);
			ArgumentValidator.ThrowIfNull("args.ItemWrapper.Item", args.ItemWrapper.Item);
			IMessageDepotItem item = args.ItemWrapper.Item;
			this.perfCountersInstance.SubmissionQueueLength.Decrement();
			this.perfCountersInstance.PoisonQueueLength.RawValue = this.messageDepotQueueViewer.GetCount(MessageDepotItemStage.Submission, MessageDepotItemState.Poisoned);
			TransportMailItem transportMailItem = (TransportMailItem)item.MessageObject;
			if (transportMailItem.QueuedRecipientsByAgeToken != null)
			{
				this.queuedRecipientsByAge.TrackExitingSubmissionQueue(transportMailItem);
			}
		}

		private const string TotalPerfCounterInstanceName = "_Total";

		private readonly IMessageDepotQueueViewer messageDepotQueueViewer;

		private readonly QueuingPerfCountersInstance perfCountersInstance;

		private readonly SlidingTotalCounter messagesSubmittedRecently;

		private readonly QueuedRecipientsByAgePerfCountersWrapper queuedRecipientsByAge;
	}
}
