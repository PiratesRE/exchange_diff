using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class E2ELatencyBucketsPerfCountersWrapper
	{
		internal E2ELatencyBucketsPerfCountersWrapper()
		{
			this.instanceLe90Sec = E2ELatencyBucketsPerfCounters.GetInstance("<=90sec");
			this.instanceGt90SecLe15Min = E2ELatencyBucketsPerfCounters.GetInstance("90sec_to_15min");
			this.instanceGt15Min = E2ELatencyBucketsPerfCounters.GetInstance(">15min");
			this.latencyBucketsMap = new Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>, Tuple<SlidingTotalCounter, ExPerformanceCounter>>
			{
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.DeliverHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.DeliverHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.DeliverHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.DeliverNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.DeliverNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.DeliverNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.DeliverLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.DeliverLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.DeliverLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.DeliverNonePriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.DeliverNonePriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.DeliverNonePriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.SendToExternalHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.SendToExternalHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.High, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.SendToExternalHighPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.SendToExternalNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.SendToExternalNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Normal, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.SendToExternalNormalPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.SendToExternalLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.SendToExternalLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Low, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.SendToExternalLowPriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceLe90Sec.SendToExternalNonePriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt90SecLe15Min.SendToExternalNonePriority)
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.None, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min),
					new Tuple<SlidingTotalCounter, ExPerformanceCounter>(new SlidingTotalCounter(E2ELatencyBucketsPerfCountersWrapper.SlidingWindowLength, E2ELatencyBucketsPerfCountersWrapper.BucketLength), this.instanceGt15Min.SendToExternalNonePriority)
				}
			};
			this.slaInstanceHigh = E2ELatencySlaPerfCounters.GetInstance("high");
			this.slaInstanceNormal = E2ELatencySlaPerfCounters.GetInstance("normal");
			this.slaInstanceLow = E2ELatencySlaPerfCounters.GetInstance("low");
			this.slaInstanceNone = E2ELatencySlaPerfCounters.GetInstance("none");
			this.latencySlaMap = new Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>, ExPerformanceCounter>
			{
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.High),
					this.slaInstanceHigh.DeliverPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Normal),
					this.slaInstanceNormal.DeliverPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Low),
					this.slaInstanceLow.DeliverPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.None),
					this.slaInstanceNone.DeliverPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.High),
					this.slaInstanceHigh.SendToExternalPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Normal),
					this.slaInstanceNormal.SendToExternalPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Low),
					this.slaInstanceLow.SendToExternalPercentMeetingSla
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.None),
					this.slaInstanceNone.SendToExternalPercentMeetingSla
				}
			};
			this.instanceTotal = E2ELatencyBucketsPerfCounters.TotalInstance;
			this.totalPerfCountersMap = new Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>, ExPerformanceCounter>
			{
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.High),
					this.instanceTotal.DeliverHighPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Normal),
					this.instanceTotal.DeliverNormalPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.Low),
					this.instanceTotal.DeliverLowPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, DeliveryPriority.None),
					this.instanceTotal.DeliverNonePriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.High),
					this.instanceTotal.SendToExternalHighPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Normal),
					this.instanceTotal.SendToExternalNormalPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.Low),
					this.instanceTotal.SendToExternalLowPriority
				},
				{
					new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, DeliveryPriority.None),
					this.instanceTotal.SendToExternalNonePriority
				}
			};
		}

		internal void Flush()
		{
			lock (this)
			{
				foreach (Tuple<SlidingTotalCounter, ExPerformanceCounter> tuple in this.latencyBucketsMap.Values)
				{
					SlidingTotalCounter item = tuple.Item1;
					ExPerformanceCounter item2 = tuple.Item2;
					item2.RawValue = item.Sum;
				}
				foreach (Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority> tuple2 in this.latencySlaMap.Keys)
				{
					this.UpdateLatencySlaCounters(tuple2.Item1, tuple2.Item2);
				}
			}
		}

		internal void Reset()
		{
			foreach (Tuple<SlidingTotalCounter, ExPerformanceCounter> tuple in this.latencyBucketsMap.Values)
			{
				ExPerformanceCounter item = tuple.Item2;
				item.RawValue = 0L;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in this.latencySlaMap.Values)
			{
				exPerformanceCounter.RawValue = 0L;
			}
		}

		internal SlidingTotalCounter GetSlidingTotalCounterForUnitTestsOnly(E2ELatencyBucketsPerfCountersWrapper.EventType eventType, DeliveryPriority priority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket bucket)
		{
			Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket> key = new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(eventType, priority, bucket);
			Tuple<SlidingTotalCounter, ExPerformanceCounter> tuple = this.latencyBucketsMap[key];
			return tuple.Item1;
		}

		internal void RecordMailboxDeliveryLatency(DeliveryPriority priority, TimeSpan latency, int recipientCount)
		{
			this.RecordMessageLatency(E2ELatencyBucketsPerfCountersWrapper.EventType.MailboxDelivery, priority, latency, recipientCount);
		}

		internal void RecordExternalSendLatency(DeliveryPriority priority, TimeSpan latency, int recipientCount)
		{
			this.RecordMessageLatency(E2ELatencyBucketsPerfCountersWrapper.EventType.ExternalSend, priority, latency, recipientCount);
		}

		private void RecordMessageLatency(E2ELatencyBucketsPerfCountersWrapper.EventType eventType, DeliveryPriority priority, TimeSpan latency, int recipientCount)
		{
			if (recipientCount <= 0)
			{
				throw new ArgumentOutOfRangeException("recipientCount: " + recipientCount);
			}
			E2ELatencyBucketsPerfCountersWrapper.LatencyBucket item;
			if (latency.TotalSeconds <= 90.0)
			{
				item = E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec;
			}
			else if (latency.TotalMinutes <= 15.0)
			{
				item = E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt90SecLe15Min;
			}
			else
			{
				item = E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Gt15Min;
			}
			Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket> key = new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(eventType, priority, item);
			Tuple<SlidingTotalCounter, ExPerformanceCounter> tuple = this.latencyBucketsMap[key];
			SlidingTotalCounter item2 = tuple.Item1;
			ExPerformanceCounter item3 = tuple.Item2;
			lock (this)
			{
				item3.RawValue = item2.AddValue((long)recipientCount);
				this.UpdateLatencySlaCounters(eventType, priority);
			}
		}

		private void UpdateLatencySlaCounters(E2ELatencyBucketsPerfCountersWrapper.EventType eventType, DeliveryPriority priority)
		{
			Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket> key = new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>(eventType, priority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket.Le90Sec);
			Tuple<SlidingTotalCounter, ExPerformanceCounter> tuple = this.latencyBucketsMap[key];
			ExPerformanceCounter item = tuple.Item2;
			Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority> key2 = new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(eventType, priority);
			ExPerformanceCounter exPerformanceCounter = this.totalPerfCountersMap[key2];
			double num = (exPerformanceCounter.RawValue > 0L) ? ((double)item.RawValue / (double)exPerformanceCounter.RawValue) : 1.0;
			Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority> key3 = new Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>(eventType, priority);
			ExPerformanceCounter exPerformanceCounter2 = this.latencySlaMap[key3];
			exPerformanceCounter2.RawValue = (long)(num * 100.0);
		}

		private static readonly TimeSpan SlidingWindowLength = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan BucketLength = TimeSpan.FromSeconds(1.0);

		private readonly E2ELatencyBucketsPerfCountersInstance instanceLe90Sec;

		private readonly E2ELatencyBucketsPerfCountersInstance instanceGt90SecLe15Min;

		private readonly E2ELatencyBucketsPerfCountersInstance instanceGt15Min;

		private readonly E2ELatencyBucketsPerfCountersInstance instanceTotal;

		private readonly E2ELatencySlaPerfCountersInstance slaInstanceHigh;

		private readonly E2ELatencySlaPerfCountersInstance slaInstanceNormal;

		private readonly E2ELatencySlaPerfCountersInstance slaInstanceLow;

		private readonly E2ELatencySlaPerfCountersInstance slaInstanceNone;

		private readonly Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority, E2ELatencyBucketsPerfCountersWrapper.LatencyBucket>, Tuple<SlidingTotalCounter, ExPerformanceCounter>> latencyBucketsMap;

		private readonly Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>, ExPerformanceCounter> latencySlaMap;

		private readonly Dictionary<Tuple<E2ELatencyBucketsPerfCountersWrapper.EventType, DeliveryPriority>, ExPerformanceCounter> totalPerfCountersMap;

		internal enum LatencyBucket
		{
			Le90Sec,
			Gt90SecLe15Min,
			Gt15Min
		}

		internal enum EventType
		{
			MailboxDelivery,
			ExternalSend
		}
	}
}
