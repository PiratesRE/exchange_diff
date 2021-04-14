using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.JobQueues
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class JobQueueManager
	{
		private JobQueueManager()
		{
		}

		public static void Initialize(IEnumerable<JobQueue> queues)
		{
			if (!JobQueueManager.instance.initialized)
			{
				lock (JobQueueManager.instance.initializeLockObject)
				{
					if (!JobQueueManager.instance.initialized && !JobQueueManager.instance.shuttingdown)
					{
						JobQueueManager.instance.InternalInitialize(queues);
						JobQueueManager.instance.initialized = true;
					}
				}
			}
		}

		public static void Shutdown()
		{
			lock (JobQueueManager.instance.initializeLockObject)
			{
				JobQueueManager.instance.shuttingdown = true;
				if (!JobQueueManager.instance.initialized)
				{
					return;
				}
			}
			JobQueueManager.instance.InternalShutdown();
		}

		public static EnqueueResult Enqueue(QueueType type, byte[] data)
		{
			if (!JobQueueManager.instance.initialized)
			{
				return new EnqueueResult(EnqueueResultType.QueueServerNotInitialized);
			}
			if (JobQueueManager.instance.shuttingdown)
			{
				return new EnqueueResult(EnqueueResultType.QueueServerShutDown);
			}
			if (data == null || data.Length == 0)
			{
				return new EnqueueResult(EnqueueResultType.InvalidData);
			}
			JobQueue jobQueue = null;
			if (!JobQueueManager.instance.queues.TryGetValue(type, out jobQueue))
			{
				return new EnqueueResult(EnqueueResultType.QueueServerNotInitialized);
			}
			return jobQueue.Enqueue(data);
		}

		private void InternalInitialize(IEnumerable<JobQueue> queues)
		{
			this.queues.Clear();
			foreach (JobQueue jobQueue in queues)
			{
				this.queues[jobQueue.Type] = jobQueue;
			}
		}

		private void InternalShutdown()
		{
			ManualResetEvent[] array = new ManualResetEvent[this.queues.Count];
			int num = 0;
			foreach (JobQueue jobQueue in this.queues.Values)
			{
				array[num] = jobQueue.ShutdownEvent;
				num++;
				jobQueue.SignalShutdown();
			}
			WaitHandle.WaitAll(array, TimeSpan.FromSeconds(15.0));
			foreach (JobQueue jobQueue2 in this.queues.Values)
			{
				jobQueue2.Cleanup();
			}
		}

		private static readonly JobQueueManager instance = new JobQueueManager();

		private readonly object initializeLockObject = new object();

		private bool initialized;

		private bool shuttingdown;

		private Dictionary<QueueType, JobQueue> queues = new Dictionary<QueueType, JobQueue>();
	}
}
