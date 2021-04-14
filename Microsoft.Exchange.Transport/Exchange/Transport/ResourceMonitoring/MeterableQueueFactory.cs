using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class MeterableQueueFactory
	{
		internal static Func<string, MessageQueue, IMeterableQueue> CreateMeterableQueueFunc
		{
			get
			{
				return MeterableQueueFactory.createMeterableQueueFunc;
			}
			set
			{
				MeterableQueueFactory.createMeterableQueueFunc = value;
			}
		}

		internal static IMeterableQueue CreateMeterableQueue(string name, MessageQueue queue)
		{
			return MeterableQueueFactory.CreateMeterableQueueFunc(name, queue);
		}

		private static IMeterableQueue CreateRealMeterableQueue(string name, MessageQueue queue)
		{
			ArgumentValidator.ThrowIfNull("queue", queue);
			return new MeterableQueue(name, queue);
		}

		private static Func<string, MessageQueue, IMeterableQueue> createMeterableQueueFunc = new Func<string, MessageQueue, IMeterableQueue>(MeterableQueueFactory.CreateRealMeterableQueue);
	}
}
