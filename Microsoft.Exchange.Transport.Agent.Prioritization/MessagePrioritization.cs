using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.Prioritization
{
	internal class MessagePrioritization
	{
		public MessagePrioritization() : this(() => DateTime.UtcNow)
		{
		}

		internal MessagePrioritization(Func<DateTime> currentTimeProvider)
		{
			this.anonymousSenderCostBucket = new MessagePrioritization.SenderAccumulatedCostBucket(currentTimeProvider);
			this.internalSenderCostBucket = new MessagePrioritization.SenderAccumulatedCostBucket(currentTimeProvider);
			this.recipientCostBucket = new MessagePrioritization.RecipientAccumulatedCostBucket(currentTimeProvider);
		}

		internal static long MessageSizeThreshold
		{
			get
			{
				return MessagePrioritization.messageSizeThreshold;
			}
		}

		internal static long AnonymousMessageSizeThreshold
		{
			get
			{
				return MessagePrioritization.anonymousMessageSizeThreshold;
			}
		}

		internal static long RecipientCostLevel1Threshold
		{
			get
			{
				return MessagePrioritization.recipientCostLevel1Threshold;
			}
		}

		internal static long RecipientCostLevel2Threshold
		{
			get
			{
				return MessagePrioritization.recipientCostLevel2Threshold;
			}
		}

		internal static long AnonymousRecipientCostLevel1Threshold
		{
			get
			{
				return MessagePrioritization.anonymousRecipientCostLevel1Threshold;
			}
		}

		internal static long AnonymousRecipientCostLevel2Threshold
		{
			get
			{
				return MessagePrioritization.anonymousRecipientCostLevel2Threshold;
			}
		}

		internal static TimeSpan SlidingCounterWindowLength
		{
			get
			{
				return MessagePrioritization.slidingCounterWindowLength;
			}
		}

		internal static TimeSpan SlidingCounterBucketLength
		{
			get
			{
				return MessagePrioritization.slidingCounterBucketLength;
			}
		}

		public void PrioritizeMessage(bool isInternalSender, string fromAddress, long messageSize, int recipientCost, MailItem mailItem, Trace tracer, out DeliveryPriority priority, out string reason)
		{
			this.PrioritizeSenderMessage(isInternalSender, fromAddress, mailItem.CachedMimeStreamLength, recipientCost, out priority, out reason);
			if (DeliveryPriority.Low == priority || DeliveryPriority.None == priority)
			{
				tracer.TraceDebug((long)this.GetHashCode(), "Skip recipient prioritization since message is already low or none priority.");
				return;
			}
			this.PrioritizeRecipientMessage(mailItem, tracer, out priority, out reason);
			if (DeliveryPriority.Low != priority || DeliveryPriority.None != priority)
			{
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					if (!RoutingAddress.NullReversePath.Equals(envelopeRecipient.Address) && !RoutingAddress.Empty.Equals(envelopeRecipient.Address) && !envelopeRecipient.IsPublicFolderRecipient())
					{
						this.UpdateRecipientCounter(envelopeRecipient.Address.ToString());
					}
				}
			}
			this.recipientCostBucket.ClearStaleEntries();
		}

		public void PrioritizeSenderMessage(bool isInternal, string sender, long messageSize, int recipientCost, out DeliveryPriority priority, out string reason)
		{
			if (string.IsNullOrEmpty(sender))
			{
				throw new ArgumentException("Sender is null or empty.");
			}
			long totalMessageSize;
			long totalRecipientCost;
			long num;
			long num2;
			long num3;
			if (isInternal)
			{
				this.internalSenderCostBucket.CalculateCost(sender, messageSize, recipientCost, out totalMessageSize, out totalRecipientCost);
				num = MessagePrioritization.messageSizeThreshold;
				num2 = MessagePrioritization.recipientCostLevel1Threshold;
				num3 = MessagePrioritization.recipientCostLevel2Threshold;
			}
			else
			{
				this.anonymousSenderCostBucket.CalculateCost(sender, messageSize, recipientCost, out totalMessageSize, out totalRecipientCost);
				num = MessagePrioritization.anonymousMessageSizeThreshold;
				num2 = MessagePrioritization.anonymousRecipientCostLevel1Threshold;
				num3 = MessagePrioritization.anonymousRecipientCostLevel2Threshold;
			}
			priority = MessagePrioritization.GetSenderPriority(totalMessageSize, totalRecipientCost, num, num2, num3);
			reason = MessagePrioritization.GetSenderPrioritizationReason(totalMessageSize, totalRecipientCost, num, num2, num3);
		}

		public void PrioritizeRecipientMessage(MailItem mailItem, Trace tracer, out DeliveryPriority priority, out string reason)
		{
			int hashCode = this.GetHashCode();
			int count = mailItem.Recipients.Count;
			priority = DeliveryPriority.Normal;
			reason = null;
			if (count == 0)
			{
				tracer.TraceDebug((long)hashCode, "Skip recipient prioritization since there are no P1 recipients on the message");
				return;
			}
			string inboxRulePropertyValue = MessagePrioritization.TryGetStringProperty(mailItem.Properties, "Microsoft.Exchange.Transport.GeneratedByMailboxRule");
			HeaderList headers = mailItem.MimeDocument.RootPart.Headers;
			Header approvalInitiatorHeader = headers.FindFirst("X-MS-Exchange-Organization-Approval-Initiator");
			string approvalInitiatorTransportRuleName = MessagePrioritization.TryGetStringProperty(mailItem.Properties, "Microsoft.Exchange.Transport.ModeratedByTransportRule");
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				if (RoutingAddress.NullReversePath.Equals(envelopeRecipient.Address) || RoutingAddress.Empty.Equals(envelopeRecipient.Address))
				{
					tracer.TraceDebug<RoutingAddress>((long)hashCode, "Skip recipient since its address is empty or null reverse path. Address={0}", envelopeRecipient.Address);
				}
				else if (envelopeRecipient.IsPublicFolderRecipient())
				{
					tracer.TraceDebug<RoutingAddress>((long)hashCode, "Skip public folder recipient {0}", envelopeRecipient.Address);
				}
				else
				{
					long totalRecipients = this.recipientCostBucket.CalculateCost(envelopeRecipient.Address.ToString());
					string transportRulePropertyValue = MessagePrioritization.TryGetStringProperty(envelopeRecipient.Properties, "Microsoft.Exchange.Transport.AddedByTransportRule");
					priority = MessagePrioritization.GetRecipientPriority(envelopeRecipient.Address, totalRecipients, MessagePrioritization.RecipientCountLevel1Threshold, MessagePrioritization.RecipientCountLevel2Threshold, count, approvalInitiatorHeader, approvalInitiatorTransportRuleName, inboxRulePropertyValue, transportRulePropertyValue, tracer, hashCode, out reason);
					if (DeliveryPriority.Low == priority || DeliveryPriority.None == priority)
					{
						break;
					}
				}
			}
		}

		internal static string GetSenderPrioritizationReason(long totalMessageSize, long totalRecipientCost, long messageSizeThreshold, long recipientCostLevel1Threshold, long recipientCostLevel2Threshold)
		{
			return string.Format(CultureInfo.InvariantCulture, "AMS:{0}/{1}|ARC:{2}/{3};{4}", new object[]
			{
				totalMessageSize,
				messageSizeThreshold,
				totalRecipientCost,
				recipientCostLevel1Threshold,
				recipientCostLevel2Threshold
			});
		}

		internal static DeliveryPriority GetRecipientPriority(RoutingAddress recipientAddress, long totalRecipients, long totalRecipientsLevel1Threshold, long totalRecipientsLevel2Threshold, int recipientCountOnMailItem, Header approvalInitiatorHeader, string approvalInitiatorTransportRuleName, string inboxRulePropertyValue, string transportRulePropertyValue, Trace tracer, int hashCode, out string reason)
		{
			reason = null;
			DeliveryPriority deliveryPriority = DeliveryPriority.Normal;
			bool flag = totalRecipients > totalRecipientsLevel1Threshold;
			if (flag)
			{
				DeliveryPriority deliveryPriority2 = DeliveryPriority.Low;
				string text = null;
				if (1 == recipientCountOnMailItem)
				{
					tracer.TraceDebug((long)hashCode, "Set priority: {0} due to single recipient on message. Recipient={1}, RecipientCount={2}, ThresholdLevel1={3}, ThresholdLevel2={4}.", new object[]
					{
						deliveryPriority2,
						recipientAddress,
						totalRecipients,
						totalRecipientsLevel1Threshold,
						totalRecipientsLevel2Threshold
					});
					text = "|SR";
					deliveryPriority = deliveryPriority2;
				}
				string text2 = null;
				if (approvalInitiatorHeader != null)
				{
					tracer.TraceDebug((long)hashCode, "Set priority: {0} due to arbitration initiator message. Recipient={1}, RecipientCount={2}, ThresholdLevel1={3}, ThresholdLevel2={4}.", new object[]
					{
						deliveryPriority2,
						recipientAddress,
						totalRecipients,
						totalRecipientsLevel1Threshold,
						totalRecipientsLevel2Threshold
					});
					text2 = (string.IsNullOrEmpty(approvalInitiatorTransportRuleName) ? "|AI" : "|AI:");
					deliveryPriority = deliveryPriority2;
				}
				string text3 = null;
				if (!string.IsNullOrEmpty(inboxRulePropertyValue))
				{
					tracer.TraceDebug((long)hashCode, "Set priority: {0} due to inbox rule generated message. Recipient={1}, RecipientCount={2}, ThresholdLevel1={3}, ThresholdLevel2={4}, InboxRule={5}.", new object[]
					{
						deliveryPriority2,
						recipientAddress,
						totalRecipients,
						totalRecipientsLevel1Threshold,
						totalRecipientsLevel2Threshold,
						inboxRulePropertyValue
					});
					text3 = "|IR:";
					deliveryPriority = deliveryPriority2;
				}
				string text4 = null;
				if (!string.IsNullOrEmpty(transportRulePropertyValue))
				{
					tracer.TraceDebug((long)hashCode, "Set priority: {0} due to recipient added by transport rule. Recipient={1}, RecipientCount={2}, ThresholdLevel1={3}, ThresholdLevel2={4}, TransportRule={5}.", new object[]
					{
						deliveryPriority2,
						recipientAddress,
						totalRecipients,
						totalRecipientsLevel1Threshold,
						totalRecipientsLevel2Threshold,
						transportRulePropertyValue
					});
					text4 = "|TR:";
					deliveryPriority = deliveryPriority2;
				}
				if (deliveryPriority2 == deliveryPriority)
				{
					reason = string.Format(CultureInfo.InvariantCulture, "RC:{0}:{1}/{2};{3}{4}{5}{6}{7}{8}{9}{10}", new object[]
					{
						SuppressingPiiData.Redact(recipientAddress),
						totalRecipients,
						totalRecipientsLevel1Threshold,
						totalRecipientsLevel2Threshold,
						text,
						text2,
						approvalInitiatorTransportRuleName,
						text3,
						inboxRulePropertyValue,
						text4,
						transportRulePropertyValue
					});
				}
			}
			return deliveryPriority;
		}

		internal void UpdateRecipientCounter(string recipient)
		{
			this.recipientCostBucket.UpdateCounter(recipient);
		}

		private static string TryGetStringProperty(IDictionary<string, object> propertyMap, string key)
		{
			string result = null;
			object obj;
			if (propertyMap.TryGetValue(key, out obj))
			{
				result = (string)obj;
			}
			return result;
		}

		private static DeliveryPriority GetSenderPriority(long totalMessageSize, long totalRecipientCost, long messageSizeThreshold, long recipientCostLevel1Threshold, long recipientCostLevel2Threshold)
		{
			if (totalRecipientCost > recipientCostLevel2Threshold)
			{
				return DeliveryPriority.Low;
			}
			if (totalMessageSize > messageSizeThreshold || totalRecipientCost > recipientCostLevel1Threshold)
			{
				return DeliveryPriority.Low;
			}
			return DeliveryPriority.Normal;
		}

		internal static readonly long RecipientCountLevel1Threshold = Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AccumulatedRecipientCountLevel1Threshold;

		internal static readonly long RecipientCountLevel2Threshold = Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AccumulatedRecipientCountLevel2Threshold;

		private static long messageSizeThreshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AccumulatedMessageSizeThreshold.ToBytes();

		private static long anonymousMessageSizeThreshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AnonymousAccumulatedMessageSizeThreshold.ToBytes();

		private static long recipientCostLevel1Threshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AccumulatedRecipientCostLevel1Threshold;

		private static long recipientCostLevel2Threshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AccumulatedRecipientCostLevel2Threshold;

		private static long anonymousRecipientCostLevel1Threshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AnonymousAccumulatedRecipientCostLevel1Threshold;

		private static long anonymousRecipientCostLevel2Threshold = (long)Components.TransportAppConfig.DeliveryQueuePrioritizationConfiguration.AnonymousAccumulatedRecipientCostLevel2Threshold;

		private static TimeSpan slidingCounterWindowLength = TimeSpan.FromMinutes(1.0);

		private static TimeSpan slidingCounterBucketLength = TimeSpan.FromSeconds(5.0);

		private MessagePrioritization.SenderAccumulatedCostBucket anonymousSenderCostBucket;

		private MessagePrioritization.SenderAccumulatedCostBucket internalSenderCostBucket;

		private MessagePrioritization.RecipientAccumulatedCostBucket recipientCostBucket;

		private interface IAccumulatedCosts
		{
			bool IsStale();
		}

		private struct SenderAccumulatedCosts : MessagePrioritization.IAccumulatedCosts
		{
			internal SenderAccumulatedCosts(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider)
			{
				this.messageSizeCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, currentTimeProvider);
				this.recipientCostCounter = new SlidingTotalCounter(slidingWindowLength, bucketLength, currentTimeProvider);
			}

			internal SlidingTotalCounter MessageSizeCounter
			{
				get
				{
					return this.messageSizeCounter;
				}
				set
				{
					this.messageSizeCounter = value;
				}
			}

			internal SlidingTotalCounter RecipientCostCounter
			{
				get
				{
					return this.recipientCostCounter;
				}
				set
				{
					this.recipientCostCounter = value;
				}
			}

			public bool IsStale()
			{
				return 0L == this.recipientCostCounter.Sum;
			}

			private SlidingTotalCounter messageSizeCounter;

			private SlidingTotalCounter recipientCostCounter;
		}

		private struct RecipientAccumulatedCosts : MessagePrioritization.IAccumulatedCosts
		{
			internal RecipientAccumulatedCosts(TimeSpan slidingWindowLength, TimeSpan bucketLength, Func<DateTime> currentTimeProvider)
			{
				this.recipientCounter = new SlidingTotalCounter(MessagePrioritization.slidingCounterWindowLength, bucketLength, currentTimeProvider);
			}

			internal SlidingTotalCounter RecipientCounter
			{
				get
				{
					return this.recipientCounter;
				}
				set
				{
					this.recipientCounter = value;
				}
			}

			public bool IsStale()
			{
				return 0L == this.recipientCounter.Sum;
			}

			private SlidingTotalCounter recipientCounter;
		}

		private class AccumulatedCostBucket
		{
			public AccumulatedCostBucket(Func<DateTime> currentTimeProvider)
			{
				this.AccumulatedCosts = new Dictionary<string, MessagePrioritization.IAccumulatedCosts>();
				this.CurrentTimeProvider = currentTimeProvider;
				this.lastClearingStaleEntriesTime = DateTime.MinValue;
				this.SyncObject = new object();
			}

			protected Dictionary<string, MessagePrioritization.IAccumulatedCosts> AccumulatedCosts { get; set; }

			protected object SyncObject { get; set; }

			protected Func<DateTime> CurrentTimeProvider { get; set; }

			protected void ClearStaleEntriesIfNecessary()
			{
				if (this.AccumulatedCosts.Count < 20000 && this.CurrentTimeProvider() - this.lastClearingStaleEntriesTime < TimeSpan.FromMinutes(1.0))
				{
					return;
				}
				List<string> list = new List<string>();
				this.lastClearingStaleEntriesTime = this.CurrentTimeProvider();
				foreach (KeyValuePair<string, MessagePrioritization.IAccumulatedCosts> keyValuePair in this.AccumulatedCosts)
				{
					if (keyValuePair.Value.IsStale())
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (string key in list)
				{
					this.AccumulatedCosts.Remove(key);
				}
			}

			private DateTime lastClearingStaleEntriesTime;
		}

		private class SenderAccumulatedCostBucket : MessagePrioritization.AccumulatedCostBucket
		{
			public SenderAccumulatedCostBucket(Func<DateTime> currentTimeProvider) : base(currentTimeProvider)
			{
			}

			public void CalculateCost(string sender, long messageSize, int recipientCost, out long totalMessageSize, out long totalRecipientCost)
			{
				lock (base.SyncObject)
				{
					if (!base.AccumulatedCosts.ContainsKey(sender))
					{
						base.AccumulatedCosts.Add(sender, new MessagePrioritization.SenderAccumulatedCosts(MessagePrioritization.SlidingCounterWindowLength, MessagePrioritization.SlidingCounterBucketLength, base.CurrentTimeProvider));
					}
					MessagePrioritization.SenderAccumulatedCosts senderAccumulatedCosts = (MessagePrioritization.SenderAccumulatedCosts)base.AccumulatedCosts[sender];
					totalMessageSize = senderAccumulatedCosts.MessageSizeCounter.AddValue(messageSize);
					totalRecipientCost = senderAccumulatedCosts.RecipientCostCounter.AddValue((long)recipientCost);
					base.ClearStaleEntriesIfNecessary();
				}
			}
		}

		private class RecipientAccumulatedCostBucket : MessagePrioritization.AccumulatedCostBucket
		{
			public RecipientAccumulatedCostBucket(Func<DateTime> currentTimeProvider) : base(currentTimeProvider)
			{
			}

			public long CalculateCost(string recipient)
			{
				long result;
				lock (base.SyncObject)
				{
					result = this.GetOrCreateAccumulatedCosts(recipient).RecipientCounter.Sum + 1L;
				}
				return result;
			}

			public void UpdateCounter(string recipient)
			{
				lock (base.SyncObject)
				{
					this.GetOrCreateAccumulatedCosts(recipient).RecipientCounter.AddValue(1L);
				}
			}

			public void ClearStaleEntries()
			{
				lock (base.SyncObject)
				{
					base.ClearStaleEntriesIfNecessary();
				}
			}

			private MessagePrioritization.RecipientAccumulatedCosts GetOrCreateAccumulatedCosts(string recipient)
			{
				if (!base.AccumulatedCosts.ContainsKey(recipient))
				{
					MessagePrioritization.RecipientAccumulatedCosts recipientAccumulatedCosts = new MessagePrioritization.RecipientAccumulatedCosts(MessagePrioritization.SlidingCounterWindowLength, MessagePrioritization.SlidingCounterBucketLength, base.CurrentTimeProvider);
					base.AccumulatedCosts.Add(recipient, recipientAccumulatedCosts);
					return recipientAccumulatedCosts;
				}
				return (MessagePrioritization.RecipientAccumulatedCosts)base.AccumulatedCosts[recipient];
			}
		}
	}
}
