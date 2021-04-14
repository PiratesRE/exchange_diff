using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class QueueFactory
	{
		public static IQueue<T> GetQueue<T>(Queues queue)
		{
			EnumValidator.ThrowIfInvalid<Queues>(queue);
			if (QueueFactory.queueList == null)
			{
				lock (QueueFactory.syncRoot)
				{
					if (QueueFactory.queueList == null)
					{
						QueueFactory.queueList = new IDisposable[Enum.GetValues(typeof(Queues)).Length];
					}
				}
			}
			if (QueueFactory.queueList[(int)queue] == null)
			{
				lock (QueueFactory.syncRoot)
				{
					if (QueueFactory.queueList[(int)queue] == null)
					{
						QueueFactory.queueList[(int)queue] = new MemoryQueue<T>();
					}
				}
			}
			return (IQueue<T>)QueueFactory.queueList[(int)queue];
		}

		internal static void Reset()
		{
			lock (QueueFactory.syncRoot)
			{
				if (QueueFactory.queueList != null)
				{
					foreach (IDisposable disposable in QueueFactory.queueList)
					{
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					QueueFactory.queueList = null;
				}
			}
		}

		private static IDisposable[] queueList;

		private static object syncRoot = new object();
	}
}
