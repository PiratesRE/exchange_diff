using System;
using System.Text;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class PrioritizedLatencyPerformanceCounter : ILatencyPerformanceCounter
	{
		public LatencyPerformanceCounterType CounterType
		{
			get
			{
				return this.counterType;
			}
		}

		private PrioritizedLatencyPerformanceCounter(LatencyPerformanceCounterType type)
		{
			this.counterType = type;
		}

		public static ILatencyPerformanceCounter CreateInstance(string instanceName, TimeSpan expiryInterval, long infinitySeconds)
		{
			return PrioritizedLatencyPerformanceCounter.CreateInstance(instanceName, expiryInterval, infinitySeconds, LatencyPerformanceCounterType.Local);
		}

		public static ILatencyPerformanceCounter CreateInstance(string instanceName, TimeSpan expiryInterval, long infinitySeconds, LatencyPerformanceCounterType type)
		{
			PrioritizedLatencyPerformanceCounter prioritizedLatencyPerformanceCounter = new PrioritizedLatencyPerformanceCounter(type);
			prioritizedLatencyPerformanceCounter.perfCounters = new LatencyPerformanceCounter[PrioritizedLatencyPerformanceCounter.priorityValues.Length];
			bool flag = true;
			foreach (DeliveryPriority deliveryPriority in PrioritizedLatencyPerformanceCounter.priorityValues)
			{
				prioritizedLatencyPerformanceCounter.perfCounters[(int)deliveryPriority] = LatencyPerformanceCounter.CreateInstance(PrioritizedLatencyPerformanceCounter.GetInstanceName(instanceName, deliveryPriority), expiryInterval, infinitySeconds, false, type);
				if (prioritizedLatencyPerformanceCounter.perfCounters[(int)deliveryPriority] == null)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return null;
			}
			return prioritizedLatencyPerformanceCounter;
		}

		public void AddValue(long latencySeconds)
		{
			this.AddValue(latencySeconds, DeliveryPriority.Normal);
		}

		public void AddValue(long latencySeconds, DeliveryPriority priority)
		{
			if (priority < DeliveryPriority.High || priority > (DeliveryPriority)PrioritizedLatencyPerformanceCounter.priorityValues.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Provided priority '{0}' is outside valid range", priority));
			}
			if (latencySeconds >= 0L)
			{
				this.perfCounters[(int)priority].AddValue(latencySeconds);
				this.perfCounters[(int)priority].Update();
			}
		}

		public void Update()
		{
			foreach (DeliveryPriority deliveryPriority in PrioritizedLatencyPerformanceCounter.priorityValues)
			{
				this.perfCounters[(int)deliveryPriority].Update();
			}
		}

		public void Reset()
		{
			foreach (DeliveryPriority deliveryPriority in PrioritizedLatencyPerformanceCounter.priorityValues)
			{
				this.perfCounters[(int)deliveryPriority].Reset();
			}
			this.Update();
		}

		private static string GetInstanceName(string instanceName, DeliveryPriority priority)
		{
			StringBuilder stringBuilder = new StringBuilder(instanceName);
			stringBuilder.Append(" - ");
			stringBuilder.Append(priority.ToString());
			return stringBuilder.ToString();
		}

		private static readonly DeliveryPriority[] priorityValues = (DeliveryPriority[])Enum.GetValues(typeof(DeliveryPriority));

		private ILatencyPerformanceCounter[] perfCounters;

		private LatencyPerformanceCounterType counterType;
	}
}
