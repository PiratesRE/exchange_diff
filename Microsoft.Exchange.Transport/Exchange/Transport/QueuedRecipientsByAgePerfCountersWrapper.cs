using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class QueuedRecipientsByAgePerfCountersWrapper
	{
		internal QueuedRecipientsByAgePerfCountersWrapper(bool enabled)
		{
			this.enabled = enabled;
			this.counterSubmission = new SegmentedSlidingCounter(QueuedRecipientsByAgePerfCountersWrapper.slidingWindowSegments, QueuedRecipientsByAgePerfCountersWrapper.bucketLength);
			this.counterInternalHop = new SegmentedSlidingCounter(QueuedRecipientsByAgePerfCountersWrapper.slidingWindowSegments, QueuedRecipientsByAgePerfCountersWrapper.bucketLength);
			this.counterInternalMailboxDelivery = new SegmentedSlidingCounter(QueuedRecipientsByAgePerfCountersWrapper.slidingWindowSegments, QueuedRecipientsByAgePerfCountersWrapper.bucketLength);
			this.counterExternalDelivery = new SegmentedSlidingCounter(QueuedRecipientsByAgePerfCountersWrapper.slidingWindowSegments, QueuedRecipientsByAgePerfCountersWrapper.bucketLength);
			int num = 0;
			this.instances = new QueuedRecipientsByAgePerfCountersInstance[QueuedRecipientsByAgePerfCountersWrapper.slidingWindowSegments.Length + 1];
			this.instances[num++] = QueuedRecipientsByAgePerfCounters.GetInstance("<=90sec");
			this.instances[num++] = QueuedRecipientsByAgePerfCounters.GetInstance("90sec_to_5min");
			this.instances[num++] = QueuedRecipientsByAgePerfCounters.GetInstance("5min_to_15min");
			this.instances[num++] = QueuedRecipientsByAgePerfCounters.GetInstance("15min_to_1hr");
			this.instances[num++] = QueuedRecipientsByAgePerfCounters.GetInstance(">1hr");
			this.instanceValues = new long[4][];
			for (int i = 0; i < 4; i++)
			{
				this.instanceValues[i] = new long[this.instances.Length];
			}
		}

		internal void TimedUpdate()
		{
			if (!this.enabled)
			{
				return;
			}
			this.counterSubmission.TimedUpdate(this.instanceValues[0]);
			this.counterInternalHop.TimedUpdate(this.instanceValues[1]);
			this.counterInternalMailboxDelivery.TimedUpdate(this.instanceValues[2]);
			this.counterExternalDelivery.TimedUpdate(this.instanceValues[3]);
			for (int i = 0; i < this.instances.Length; i++)
			{
				this.instances[i].SubmissionNormalPriority.RawValue = this.instanceValues[0][i];
				this.instances[i].InternalHopNormalPriority.RawValue = this.instanceValues[1][i];
				this.instances[i].InternalMailboxDeliveryNormalPriority.RawValue = this.instanceValues[2][i];
				this.instances[i].ExternalDeliveryNormalPriority.RawValue = this.instanceValues[3][i];
			}
		}

		internal void Reset()
		{
			if (!this.enabled)
			{
				return;
			}
			foreach (QueuedRecipientsByAgePerfCountersInstance queuedRecipientsByAgePerfCountersInstance in this.instances)
			{
				queuedRecipientsByAgePerfCountersInstance.SubmissionNormalPriority.RawValue = 0L;
				queuedRecipientsByAgePerfCountersInstance.InternalHopNormalPriority.RawValue = 0L;
				queuedRecipientsByAgePerfCountersInstance.InternalMailboxDeliveryNormalPriority.RawValue = 0L;
				queuedRecipientsByAgePerfCountersInstance.ExternalDeliveryNormalPriority.RawValue = 0L;
			}
		}

		internal void TrackEnteringSubmissionQueue(TransportMailItem item)
		{
			if (!this.enabled)
			{
				return;
			}
			if (item.QueuedRecipientsByAgeToken != null)
			{
				throw new InvalidOperationException("item already has a token!");
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = QueuedRecipientsByAgeToken.Generate(item);
			lock (queuedRecipientsByAgeToken)
			{
				item.QueuedRecipientsByAgeToken = queuedRecipientsByAgeToken;
				QueuedRecipientsByAgePerfCountersWrapper.TrackEntering(queuedRecipientsByAgeToken, this.counterSubmission);
			}
		}

		internal void TrackExitingSubmissionQueue(TransportMailItem item)
		{
			if (!this.enabled)
			{
				return;
			}
			if (item.QueuedRecipientsByAgeToken == null)
			{
				throw new InvalidOperationException("item leaving submission queue is not being tracked!");
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = item.QueuedRecipientsByAgeToken;
			lock (queuedRecipientsByAgeToken)
			{
				item.QueuedRecipientsByAgeToken = null;
				QueuedRecipientsByAgePerfCountersWrapper.TrackExiting(queuedRecipientsByAgeToken, this.counterSubmission);
			}
		}

		internal QueuedRecipientsByAgeToken TrackEnteringCategorizer(TransportMailItem item)
		{
			if (!this.enabled)
			{
				return null;
			}
			if (item.QueuedRecipientsByAgeToken != null)
			{
				throw new InvalidOperationException("item already has a token!");
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = QueuedRecipientsByAgeToken.Generate(item);
			lock (queuedRecipientsByAgeToken)
			{
				QueuedRecipientsByAgePerfCountersWrapper.TrackEntering(queuedRecipientsByAgeToken, this.counterSubmission);
			}
			return queuedRecipientsByAgeToken;
		}

		internal void TrackExitingCategorizer(QueuedRecipientsByAgeToken token)
		{
			if (!this.enabled)
			{
				return;
			}
			lock (token)
			{
				QueuedRecipientsByAgePerfCountersWrapper.TrackExiting(token, this.counterSubmission);
			}
		}

		internal void TrackEnteringDeliveryQueue(RoutedMailItem item)
		{
			if (!this.enabled)
			{
				return;
			}
			if (item.DeliveryType == DeliveryType.Unreachable)
			{
				return;
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = QueuedRecipientsByAgeToken.Generate(item);
			lock (queuedRecipientsByAgeToken)
			{
				item.QueuedRecipientsByAgeToken = queuedRecipientsByAgeToken;
				QueuedRecipientsByAgePerfCountersWrapper.TrackEntering(queuedRecipientsByAgeToken, this.GetCounter(queuedRecipientsByAgeToken.DeliveryType));
			}
		}

		internal void TrackExitingDeliveryQueue(RoutedMailItem item)
		{
			if (!this.enabled)
			{
				return;
			}
			if (item.DeliveryType == DeliveryType.Unreachable)
			{
				return;
			}
			if (item.QueuedRecipientsByAgeToken == null)
			{
				throw new InvalidOperationException("item leaving delivery queue is not being tracked!");
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = item.QueuedRecipientsByAgeToken;
			lock (queuedRecipientsByAgeToken)
			{
				item.QueuedRecipientsByAgeToken = null;
				QueuedRecipientsByAgePerfCountersWrapper.TrackExiting(queuedRecipientsByAgeToken, this.GetCounter(queuedRecipientsByAgeToken.DeliveryType));
			}
		}

		internal void TrackEnteringSmtpSend(RoutedMailItem item)
		{
			if (!this.enabled)
			{
				return;
			}
			if (item.QueuedRecipientsByAgeToken != null)
			{
				throw new InvalidOperationException("item already has a token!");
			}
			QueuedRecipientsByAgeToken queuedRecipientsByAgeToken = QueuedRecipientsByAgeToken.Generate(item);
			lock (queuedRecipientsByAgeToken)
			{
				item.QueuedRecipientsByAgeToken = queuedRecipientsByAgeToken;
				QueuedRecipientsByAgePerfCountersWrapper.TrackEntering(queuedRecipientsByAgeToken, this.GetCounter(queuedRecipientsByAgeToken.DeliveryType));
			}
		}

		internal void TrackExitingSmtpSend(QueuedRecipientsByAgeToken token)
		{
			if (!this.enabled)
			{
				return;
			}
			lock (token)
			{
				QueuedRecipientsByAgePerfCountersWrapper.TrackExiting(token, this.GetCounter(token.DeliveryType));
			}
		}

		private SegmentedSlidingCounter GetCounter(DeliveryType deliveryType)
		{
			SegmentedSlidingCounter result;
			if (NextHopType.IsMailboxDeliveryType(deliveryType))
			{
				result = this.counterInternalMailboxDelivery;
			}
			else if (TransportDeliveryTypes.internalDeliveryTypes.Contains(deliveryType))
			{
				result = this.counterInternalHop;
			}
			else
			{
				if (!TransportDeliveryTypes.externalDeliveryTypes.Contains(deliveryType))
				{
					throw new ArgumentException("cannot get the right counter for Delivery Type: " + deliveryType);
				}
				result = this.counterExternalDelivery;
			}
			return result;
		}

		private static void TrackEntering(QueuedRecipientsByAgeToken token, SegmentedSlidingCounter counter)
		{
			if (token.DeliveryPriority == DeliveryPriority.Normal && token.RecipientCount > 0)
			{
				token.OrgArrivalTimeUsed = counter.AddEventsAt(token.OrgArrivalTimeUtc, (long)token.RecipientCount);
			}
		}

		private static void TrackExiting(QueuedRecipientsByAgeToken token, SegmentedSlidingCounter counter)
		{
			if (token.DeliveryPriority == DeliveryPriority.Normal && token.RecipientCount > 0)
			{
				counter.RemoveEventsAt(token.OrgArrivalTimeUsed, (long)token.RecipientCount);
			}
		}

		private static readonly TimeSpan bucketLength = TimeSpan.FromSeconds(10.0);

		private static readonly TimeSpan[] slidingWindowSegments = new TimeSpan[]
		{
			TimeSpan.FromSeconds(90.0),
			TimeSpan.FromMinutes(5.0) - TimeSpan.FromSeconds(90.0),
			TimeSpan.FromMinutes(15.0) - TimeSpan.FromMinutes(5.0),
			TimeSpan.FromHours(1.0) - TimeSpan.FromMinutes(15.0)
		};

		private readonly bool enabled;

		private SegmentedSlidingCounter counterSubmission;

		private SegmentedSlidingCounter counterInternalHop;

		private SegmentedSlidingCounter counterInternalMailboxDelivery;

		private SegmentedSlidingCounter counterExternalDelivery;

		private QueuedRecipientsByAgePerfCountersInstance[] instances;

		private long[][] instanceValues;
	}
}
