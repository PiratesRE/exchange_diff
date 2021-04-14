using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDatabaseOperationQueue
	{
		internal AmDatabaseOperationQueue(Guid databaseGuid)
		{
			this.databaseGuid = databaseGuid;
			this.IsEnabled = true;
			this.m_queue = new Queue<AmDbOperation>();
		}

		internal bool IsEnabled { get; set; }

		internal AmDbOperation OperationServiced
		{
			get
			{
				AmDbOperation operationServiced;
				lock (this.m_locker)
				{
					operationServiced = this.m_operationServiced;
				}
				return operationServiced;
			}
		}

		internal bool IsIdle
		{
			get
			{
				bool result;
				lock (this.m_locker)
				{
					result = (this.m_queue.Count == 0 && !this.m_isInUse);
				}
				return result;
			}
		}

		internal bool Add(List<AmDbOperation> oprList, bool checkIfQueueIdle)
		{
			bool flag = false;
			lock (this.m_locker)
			{
				if (this.IsEnabled)
				{
					if (!checkIfQueueIdle || this.IsIdle)
					{
						foreach (AmDbOperation amDbOperation in oprList)
						{
							AmTrace.Debug("AmDatabaseOperationQueue: Add operation {0} into the queue. checkIfQueueIdle:{1}, IsIdle:{2}", new object[]
							{
								amDbOperation.ToString(),
								checkIfQueueIdle,
								this.IsIdle
							});
							this.AddNoLock(amDbOperation);
						}
						flag = true;
					}
					else
					{
						foreach (AmDbOperation amDbOperation2 in oprList)
						{
							AmTrace.Debug("AmDatabaseOperationQueue: Skip operation {0}. checkIfQueueIdle:{1}, IsIdle:{2}", new object[]
							{
								amDbOperation2.ToString(),
								checkIfQueueIdle,
								this.IsIdle
							});
							this.LogQueueInfo(amDbOperation2);
						}
					}
				}
				if (!flag)
				{
					foreach (AmDbOperation amDbOperation3 in oprList)
					{
						amDbOperation3.Cancel();
					}
				}
			}
			return flag;
		}

		internal bool Add(AmDbOperation op, bool checkIfQueueIdle)
		{
			bool flag = false;
			if (!this.IsEnabled)
			{
				op.Cancel();
				return false;
			}
			lock (this.m_locker)
			{
				if (!checkIfQueueIdle || this.IsIdle)
				{
					AmTrace.Debug("AmDatabaseOperationQueue: Add operation {0} into the queue. checkIfQueueIdle:{1}, IsIdle:{2}", new object[]
					{
						op.ToString(),
						checkIfQueueIdle,
						this.IsIdle
					});
					this.AddNoLock(op);
					flag = true;
				}
				else
				{
					AmTrace.Debug("AmDatabaseOperationQueue: Skip operation {0}. checkIfQueueIdle:{1}, IsIdle:{2}", new object[]
					{
						op.ToString(),
						checkIfQueueIdle,
						this.IsIdle
					});
					this.LogQueueInfo(op);
				}
				if (!flag)
				{
					op.Cancel();
				}
			}
			return flag;
		}

		internal void LogQueueInfo(AmDbOperation operation)
		{
			string name = operation.GetType().Name;
			string uniqueId = operation.UniqueId;
			string text = operation.ToString();
			int count = this.m_queue.Count;
			string text2 = "<none>";
			string text3 = "<none>";
			string text4 = "<none>";
			if (this.m_operationServiced != null)
			{
				text2 = this.m_operationServiced.GetType().Name;
				text3 = this.m_operationServiced.UniqueId;
				text4 = this.m_operationServiced.ToString();
			}
			string text5 = "<none>";
			string text6 = "<none>";
			string text7 = "<none>";
			AmDbOperation amDbOperation = this.m_queue.FirstOrDefault<AmDbOperation>();
			if (amDbOperation != null)
			{
				text5 = amDbOperation.GetType().Name;
				text6 = amDbOperation.UniqueId;
				text7 = amDbOperation.ToString();
			}
			ReplayCrimsonEvents.DatabaseOperationSkippedSinceAlreadyActionsQueued.Log<Guid, int, string, string, string, string, string, string, string, string, string>(this.databaseGuid, count, name, uniqueId, text, text2, text3, text4, text5, text6, text7);
		}

		internal void PerformAction(object context)
		{
			AmDbOperation amDbOperation = null;
			for (;;)
			{
				lock (this.m_locker)
				{
					if (this.m_queue.Count <= 0)
					{
						this.m_operationServiced = null;
						this.m_isInUse = false;
						break;
					}
					amDbOperation = this.m_queue.Dequeue();
					this.m_operationServiced = amDbOperation;
				}
				if (this.IsEnabled)
				{
					bool flag2 = true;
					try
					{
						amDbOperation.ReportStatus(amDbOperation.Database, AmDbActionStatus.Started);
						this.RunOperation(amDbOperation);
						amDbOperation.ReportStatus(amDbOperation.Database, AmDbActionStatus.Completed);
						flag2 = false;
						continue;
					}
					finally
					{
						if (flag2)
						{
							amDbOperation.ReportStatus(amDbOperation.Database, AmDbActionStatus.Failed);
						}
					}
				}
				amDbOperation.Cancel();
			}
		}

		private void AddNoLock(AmDbOperation op)
		{
			this.m_queue.Enqueue(op);
			if (!this.m_isInUse)
			{
				this.m_isInUse = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.PerformAction));
			}
		}

		private void RunOperation(AmDbOperation opr)
		{
			AmTrace.Debug("Running database operation: {0}", new object[]
			{
				opr
			});
			AmFaultInject.SleepIfRequired(opr.Database.Guid, AmSleepTag.GenericDbOperationProcessingDelay);
			opr.Run();
			if (opr.LastException != null)
			{
				AmTrace.Error("Error from database operation {0}\n{1}", new object[]
				{
					opr,
					opr.LastException
				});
				return;
			}
			AmTrace.Debug("Database operation finished: {0}", new object[]
			{
				opr
			});
		}

		private readonly Guid databaseGuid;

		private object m_locker = new object();

		private bool m_isInUse;

		private AmDbOperation m_operationServiced;

		private Queue<AmDbOperation> m_queue;
	}
}
