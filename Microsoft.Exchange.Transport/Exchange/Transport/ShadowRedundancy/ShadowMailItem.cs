using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowMailItem : IQueueItem
	{
		public ShadowMailItem(TransportMailItem mailItem, NextHopSolution nextHopSolution, DateTime creationTime, IShadowRedundancyConfigurationSource configuration)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.creationTime = creationTime;
			this.mailItem = mailItem;
			this.nextHopSolution = nextHopSolution;
			this.configuration = configuration;
		}

		public TransportMailItem TransportMailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public NextHopSolution NextHopSolution
		{
			get
			{
				return this.nextHopSolution;
			}
		}

		public DiscardReason? DiscardReason
		{
			get
			{
				return this.discardReason;
			}
		}

		DateTime IQueueItem.DeferUntil
		{
			get
			{
				return this.NextHopSolution.DeferUntil;
			}
			set
			{
				throw new NotSupportedException("DeferUntil not supported on ShadowItems.");
			}
		}

		DateTime IQueueItem.Expiry
		{
			get
			{
				AdminActionStatus adminActionStatus = this.nextHopSolution.AdminActionStatus;
				if (adminActionStatus == AdminActionStatus.PendingDeleteWithNDR || adminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR)
				{
					return DateTime.MinValue;
				}
				return this.creationTime + this.configuration.ShadowMessageAutoDiscardInterval;
			}
		}

		DeliveryPriority IQueueItem.Priority
		{
			get
			{
				return DeliveryPriority.Normal;
			}
			set
			{
				if (value != DeliveryPriority.Normal)
				{
					throw new NotSupportedException("Priorities not supported for Shadow Messages.");
				}
			}
		}

		void IQueueItem.Update()
		{
		}

		MessageContext IQueueItem.GetMessageContext(MessageProcessingSource source)
		{
			return new MessageContext(this.TransportMailItem.RecordId, this.TransportMailItem.InternetMessageId, source);
		}

		public bool Discard(DiscardReason discardReason)
		{
			if (this.discardReason != null)
			{
				return false;
			}
			this.discardReason = new DiscardReason?(discardReason);
			this.creationTime = DateTime.MinValue;
			MessageTrackingLog.TrackHighAvailabilityDiscard(MessageTrackingSource.SMTP, this.TransportMailItem, this.discardReason.ToString());
			return true;
		}

		private TransportMailItem mailItem;

		private NextHopSolution nextHopSolution;

		private DiscardReason? discardReason;

		private DateTime creationTime;

		private IShadowRedundancyConfigurationSource configuration;
	}
}
