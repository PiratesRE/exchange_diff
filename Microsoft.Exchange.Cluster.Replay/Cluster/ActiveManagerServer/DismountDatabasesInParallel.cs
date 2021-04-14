using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DismountDatabasesInParallel
	{
		public DismountDatabasesInParallel(MdbStatus[] mdbStatuses)
		{
			this.m_mdbStatuses = mdbStatuses;
			this.m_totalRequests = mdbStatuses.Length;
		}

		public bool Execute(int waitTimeoutMs, string hint)
		{
			bool flag = false;
			Stopwatch stopwatch = new Stopwatch();
			AmStoreHelper.DismountDelegate dismountDelegate = new AmStoreHelper.DismountDelegate(AmStoreHelper.RemoteDismount);
			this.m_completedEvent = new ManualResetEvent(false);
			stopwatch.Start();
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				ReplayCrimsonEvents.ForceDismountingDatabases.Log<AmServerName, string>(AmServerName.LocalComputerName, hint);
				if (this.m_mdbStatuses != null && this.m_mdbStatuses.Length > 0)
				{
					AmTrace.Debug("DismountDatabasesInParallel.Execute() now starting with timeout of {0} ms...", new object[]
					{
						waitTimeoutMs
					});
					foreach (MdbStatus mdbStatus in this.m_mdbStatuses)
					{
						DismountDatabasesInParallel.AsyncDismountState @object = new DismountDatabasesInParallel.AsyncDismountState(mdbStatus.MdbGuid, dismountDelegate);
						dismountDelegate.BeginInvoke(null, mdbStatus.MdbGuid, UnmountFlags.SkipCacheFlush, false, new AsyncCallback(this.DismountCompletionCallback), @object);
					}
					if (this.m_completedEvent.WaitOne(waitTimeoutMs))
					{
						AmTrace.Debug("DismountDatabasesInParallel.Execute() finished dismounting DBs in {0} ms.", new object[]
						{
							stopwatch.ElapsedMilliseconds
						});
						flag = true;
					}
					else
					{
						AmTrace.Error("DismountDatabasesInParallel.Execute() timed out waiting for DBs to finish dismounting.", new object[0]);
						AmStoreServiceMonitor.KillStoreIfRunningBefore(utcNow, "DismountDatabasesInParallel");
					}
				}
			}
			finally
			{
				ReplayCrimsonEvents.ForceDismountAllDatabasesComplete.Log<bool>(flag);
				lock (this.m_locker)
				{
					this.m_completedEvent.Close();
					this.m_completedEvent = null;
				}
			}
			return flag;
		}

		private void DismountCompletionCallback(IAsyncResult ar)
		{
			DismountDatabasesInParallel.AsyncDismountState state = (DismountDatabasesInParallel.AsyncDismountState)ar.AsyncState;
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				state.DismountFunction.EndInvoke(ar);
			});
			if (ex == null)
			{
				AmTrace.Debug("DismountDatabasesInParallel: Finished dismount for DB {0}.", new object[]
				{
					state.MdbGuid
				});
			}
			else
			{
				AmTrace.Debug("DismountDatabasesInParallel: Dismount for DB {0} failed with {1}.", new object[]
				{
					state.MdbGuid,
					ex.Message
				});
			}
			lock (this.m_locker)
			{
				this.m_completedCount++;
				if (this.m_completedCount == this.m_totalRequests && this.m_completedEvent != null)
				{
					this.m_completedEvent.Set();
				}
			}
		}

		private readonly int m_totalRequests;

		private readonly MdbStatus[] m_mdbStatuses;

		private int m_completedCount;

		private ManualResetEvent m_completedEvent;

		private object m_locker = new object();

		private class AsyncDismountState
		{
			public AsyncDismountState(Guid mdbGuid, AmStoreHelper.DismountDelegate dismountFunc)
			{
				this.MdbGuid = mdbGuid;
				this.DismountFunction = dismountFunc;
			}

			public Guid MdbGuid { get; private set; }

			public AmStoreHelper.DismountDelegate DismountFunction { get; private set; }
		}
	}
}
