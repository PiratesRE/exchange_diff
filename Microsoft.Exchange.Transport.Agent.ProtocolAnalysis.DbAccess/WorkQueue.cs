using System;
using System.Collections;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal static class WorkQueue
	{
		public static int Count
		{
			get
			{
				return WorkQueue.workQueue.Count;
			}
		}

		public static void EnqueueWorkItemData(WorkItemData item)
		{
			WorkQueue.workQueue.Enqueue(item);
		}

		public static WorkItemData DequeueWorkItemData()
		{
			if (WorkQueue.workQueue.Count == 0)
			{
				return null;
			}
			return (WorkItemData)WorkQueue.workQueue.Dequeue();
		}

		private static Queue workQueue = Queue.Synchronized(new Queue());
	}
}
