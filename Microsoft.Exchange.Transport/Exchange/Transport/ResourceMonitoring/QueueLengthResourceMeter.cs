using System;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal sealed class QueueLengthResourceMeter : ResourceMeter
	{
		public QueueLengthResourceMeter(IMeterableQueue meterableQueue, PressureTransitions pressureTransitions) : base("QueueLength", QueueLengthResourceMeter.GetQueueName(meterableQueue), pressureTransitions)
		{
			this.meterableQueue = meterableQueue;
		}

		protected override long GetCurrentPressure()
		{
			return this.meterableQueue.Length;
		}

		private static string GetQueueName(IMeterableQueue meterableQueue)
		{
			ArgumentValidator.ThrowIfNull("meterableQueue", meterableQueue);
			return meterableQueue.Name;
		}

		private readonly IMeterableQueue meterableQueue;
	}
}
