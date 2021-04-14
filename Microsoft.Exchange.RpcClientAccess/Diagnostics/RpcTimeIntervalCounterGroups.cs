using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal sealed class RpcTimeIntervalCounterGroups<T> where T : IRpcCounters, new()
	{
		public RpcTimeIntervalCounterGroups() : this(RpcTimeIntervalCounterGroups<T>.DefaultIntervalSize)
		{
		}

		public RpcTimeIntervalCounterGroups(TimeSpan timeIntervalDuration)
		{
			if (timeIntervalDuration == TimeSpan.Zero)
			{
				throw new ArgumentException("timeIntervalDuration must indicate a non-zero duration for the interval.");
			}
			this.timeIntervalDuration = timeIntervalDuration;
			this.timeIntervalCounterGroups = new Dictionary<ExDateTime, T>();
		}

		public void IncrementCounter(ExDateTime timeStamp, IRpcCounterData counterData)
		{
			ExDateTime intervalEndForTimestamp = this.GetIntervalEndForTimestamp(timeStamp);
			T value;
			if (!this.timeIntervalCounterGroups.TryGetValue(intervalEndForTimestamp, out value))
			{
				value = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
				this.timeIntervalCounterGroups.Add(intervalEndForTimestamp, value);
			}
			value.IncrementCounter(counterData);
		}

		public ICollection<ExDateTime> GetTimeIntervals()
		{
			return this.timeIntervalCounterGroups.Keys;
		}

		public string GetFormattedCounterDataForInterval(ExDateTime intervalEndTime)
		{
			T t;
			if (!this.timeIntervalCounterGroups.TryGetValue(intervalEndTime, out t))
			{
				throw new ArgumentException("The specified interval end time does not contain any associated counter data.");
			}
			return t.ToString();
		}

		public void ResetCounters()
		{
			this.timeIntervalCounterGroups.Clear();
		}

		private ExDateTime GetIntervalEndForTimestamp(ExDateTime timeStamp)
		{
			long num = (timeStamp.UtcTicks + this.timeIntervalDuration.Ticks) / this.timeIntervalDuration.Ticks;
			return new ExDateTime(ExTimeZone.UtcTimeZone, new DateTime(num * this.timeIntervalDuration.Ticks));
		}

		public TimeSpan TimeIntervalDuration
		{
			get
			{
				return this.timeIntervalDuration;
			}
		}

		private static readonly TimeSpan DefaultIntervalSize = TimeSpan.FromHours(1.0);

		private readonly IDictionary<ExDateTime, T> timeIntervalCounterGroups;

		private readonly TimeSpan timeIntervalDuration;
	}
}
