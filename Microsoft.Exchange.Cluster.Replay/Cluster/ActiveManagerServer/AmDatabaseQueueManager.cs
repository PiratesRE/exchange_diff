using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDatabaseQueueManager
	{
		internal AmDatabaseQueueManager()
		{
			this.IsEnabled = true;
			this.m_dbMap = new Dictionary<Guid, AmDatabaseOperationQueue>();
		}

		internal bool IsEnabled { get; set; }

		internal void Stop()
		{
			lock (this.m_locker)
			{
				this.IsEnabled = false;
				foreach (Guid key in this.m_dbMap.Keys)
				{
					AmDatabaseOperationQueue amDatabaseOperationQueue = this.m_dbMap[key];
					amDatabaseOperationQueue.IsEnabled = false;
					if (amDatabaseOperationQueue.OperationServiced != null)
					{
						amDatabaseOperationQueue.OperationServiced.Cancel();
					}
				}
			}
			for (;;)
			{
				bool flag2 = false;
				lock (this.m_locker)
				{
					foreach (Guid key2 in this.m_dbMap.Keys)
					{
						AmDatabaseOperationQueue amDatabaseOperationQueue2 = this.m_dbMap[key2];
						if (!amDatabaseOperationQueue2.IsIdle)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					break;
				}
				Thread.Sleep(50);
			}
		}

		internal AmDatabaseOperationQueue GetOperationQueueByGuid(Guid guid, bool isCreateIfNotExist)
		{
			AmDatabaseOperationQueue amDatabaseOperationQueue = null;
			lock (this.m_locker)
			{
				if ((!this.m_dbMap.TryGetValue(guid, out amDatabaseOperationQueue) || amDatabaseOperationQueue == null) && isCreateIfNotExist)
				{
					amDatabaseOperationQueue = new AmDatabaseOperationQueue(guid);
					this.m_dbMap[guid] = amDatabaseOperationQueue;
				}
			}
			return amDatabaseOperationQueue;
		}

		internal bool Enqueue(AmDbOperation opr)
		{
			bool result = false;
			lock (this.m_locker)
			{
				if (this.IsEnabled)
				{
					AmDatabaseOperationQueue operationQueueByGuid = this.GetOperationQueueByGuid(opr.Database.Guid, true);
					result = operationQueueByGuid.Add(opr, false);
				}
				else
				{
					opr.Cancel();
				}
			}
			return result;
		}

		internal bool Enqueue(Guid dbGuid, List<AmDbOperation> oprList, bool checkIfQueueIsIdle)
		{
			bool result = false;
			lock (this.m_locker)
			{
				if (this.IsEnabled)
				{
					AmDatabaseOperationQueue operationQueueByGuid = this.GetOperationQueueByGuid(dbGuid, true);
					result = operationQueueByGuid.Add(oprList, checkIfQueueIsIdle);
				}
				else
				{
					foreach (AmDbOperation amDbOperation in oprList)
					{
						amDbOperation.Cancel();
					}
				}
			}
			return result;
		}

		private object m_locker = new object();

		private Dictionary<Guid, AmDatabaseOperationQueue> m_dbMap;
	}
}
