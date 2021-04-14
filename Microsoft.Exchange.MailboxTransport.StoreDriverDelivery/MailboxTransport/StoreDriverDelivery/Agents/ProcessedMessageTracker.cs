using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ProcessedMessageTracker
	{
		public ProcessedMessageTracker()
		{
			this.processedMessages = new ConcurrentDictionary<string, DeliveryStage>(StringComparer.OrdinalIgnoreCase);
		}

		public void AddMessageToProcessedList(string messageId, ExDateTime sentTime, Guid mailboxGuid, DeliveryStage stage)
		{
			string key2 = this.GenerateUniqueId(messageId, sentTime, mailboxGuid);
			this.processedMessages.AddOrUpdate(key2, stage, (string key, DeliveryStage oldValue) => stage);
		}

		public DeliveryStage ClearMessageFromProcessedList(string messageId, ExDateTime sentTime, Guid mailboxGuid)
		{
			string key = this.GenerateUniqueId(messageId, sentTime, mailboxGuid);
			DeliveryStage result = DeliveryStage.None;
			this.processedMessages.TryRemove(key, out result);
			return result;
		}

		public bool IsAlreadyProcessedForStage(string messageId, ExDateTime sentTime, Guid mailboxGuid, DeliveryStage checkStage)
		{
			string key = this.GenerateUniqueId(messageId, sentTime, mailboxGuid);
			DeliveryStage deliveryStage = DeliveryStage.None;
			this.processedMessages.TryGetValue(key, out deliveryStage);
			return deliveryStage >= checkStage;
		}

		private string GenerateUniqueId(string messageId, ExDateTime sentTime, Guid mailboxGuid)
		{
			return string.Format("{0}:{1}:{2}", messageId ?? "null", sentTime.ToISOString(), mailboxGuid);
		}

		private ConcurrentDictionary<string, DeliveryStage> processedMessages;
	}
}
