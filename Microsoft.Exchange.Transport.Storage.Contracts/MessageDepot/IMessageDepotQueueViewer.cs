using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal interface IMessageDepotQueueViewer
	{
		void Remove(TransportMessageId messageId, bool withNdr);

		void Suspend(TransportMessageId messageId);

		void Resume(TransportMessageId messageId);

		bool TryGet(TransportMessageId messageId, out IMessageDepotItemWrapper item);

		IMessageDepotItemWrapper Get(TransportMessageId messageId);

		long GetCount(MessageDepotItemStage stage, MessageDepotItemState state);

		void VisitMailItems(Func<IMessageDepotItemWrapper, bool> visitor);
	}
}
