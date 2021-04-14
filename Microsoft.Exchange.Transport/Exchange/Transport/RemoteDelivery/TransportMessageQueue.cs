using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class TransportMessageQueue : MessageQueue
	{
		public TransportMessageQueue(RoutedQueueBase queueStorage, PriorityBehaviour behaviour) : base(behaviour)
		{
			this.queueStorage = queueStorage;
			this.incomingRateCounter = new SlidingPercentageCounter(TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(5.0));
			this.outgoingRateCounter = new SlidingPercentageCounter(TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(5.0));
		}

		protected TransportMessageQueue(PriorityBehaviour behaviour) : base(behaviour)
		{
		}

		public double IncomingRate
		{
			get
			{
				return this.incomingRate;
			}
		}

		public double OutgoingRate
		{
			get
			{
				return this.outgoingRate;
			}
		}

		public double Velocity
		{
			get
			{
				return this.outgoingRate - this.incomingRate;
			}
		}

		public virtual bool Suspended
		{
			get
			{
				return this.queueStorage.Suspended;
			}
			set
			{
				this.queueStorage.Suspended = value;
				this.queueStorage.Commit();
			}
		}

		public virtual void Delete()
		{
		}

		public virtual bool IsInterestingQueueToLog()
		{
			return base.TotalCount >= Components.TransportAppConfig.QueueConfiguration.QueueLoggingThreshold || (this.Suspended && base.TotalCount > 0);
		}

		protected void UpdateQueueRates()
		{
			DateTime utcNow = DateTime.UtcNow;
			double totalSeconds = (utcNow - this.lastRateUpdateTime).TotalSeconds;
			int num = Interlocked.Exchange(ref this.lastIncomingMessageCount, 0);
			this.incomingRateCounter.Add((long)num, (long)totalSeconds);
			this.incomingRate = this.incomingRateCounter.GetSlidingPercentage() / 100.0;
			int num2 = Interlocked.Exchange(ref this.lastOutgoingMessageCount, 0);
			this.outgoingRateCounter.Add((long)num2, (long)totalSeconds);
			this.outgoingRate = this.outgoingRateCounter.GetSlidingPercentage() / 100.0;
			this.lastRateUpdateTime = utcNow;
		}

		protected RoutedQueueBase queueStorage;

		protected int lastIncomingMessageCount;

		protected int lastOutgoingMessageCount;

		private double incomingRate;

		private SlidingPercentageCounter incomingRateCounter;

		private double outgoingRate;

		private SlidingPercentageCounter outgoingRateCounter;

		private DateTime lastRateUpdateTime;
	}
}
