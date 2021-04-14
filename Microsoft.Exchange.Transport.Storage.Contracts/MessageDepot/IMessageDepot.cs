using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal interface IMessageDepot
	{
		void SubscribeToAddEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void UnsubscribeFromAddEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void SubscribeToActivatedEvent(MessageDepotItemStage targetStage, MessageActivatedEventHandler eventHandler);

		void UnsubscribeFromActivatedEvent(MessageDepotItemStage targetStage, MessageActivatedEventHandler eventHandler);

		void SubscribeToDeactivatedEvent(MessageDepotItemStage targetStage, MessageDeactivatedEventHandler eventHandler);

		void UnsubscribeFromDeactivatedEvent(MessageDepotItemStage targetStage, MessageDeactivatedEventHandler eventHandler);

		void SubscribeToRemovedEvent(MessageDepotItemStage targetStage, MessageRemovedEventHandler eventHandler);

		void UnsubscribeFromRemovedEvent(MessageDepotItemStage targetStage, MessageRemovedEventHandler eventHandler);

		void SubscribeToExpiredEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void UnsubscribeFromExpiredEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void SubscribeToDelayedEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void UnsubscribeFromDelayedEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler);

		void Add(IMessageDepotItem item);

		void DeferMessage(TransportMessageId messageId, TimeSpan deferTimeSpan, AcquireToken acquireToken);

		AcquireResult Acquire(TransportMessageId messageId);

		bool TryAcquire(TransportMessageId messageId, out AcquireResult result);

		void Release(TransportMessageId messageId, AcquireToken token);

		bool TryGet(TransportMessageId messageId, out IMessageDepotItemWrapper item);

		IMessageDepotItemWrapper Get(TransportMessageId messageId);

		void DehydrateAll();
	}
}
