using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ApplicationPoolHelper : DisposeTrackableBase
	{
		public ApplicationPoolHelper(string appPoolName)
		{
			this.appPoolName = appPoolName;
		}

		internal ApplicationPool ApplicationPool
		{
			get
			{
				this.Initialize();
				return this.appPool;
			}
		}

		internal List<int> WorkerProcessIds
		{
			get
			{
				List<int> list = new List<int>();
				this.Refresh();
				WorkerProcessCollection workerProcesses = this.appPool.WorkerProcesses;
				if (workerProcesses != null && workerProcesses.Count > 0)
				{
					foreach (WorkerProcess workerProcess in workerProcesses)
					{
						list.Add(workerProcess.ProcessId);
					}
				}
				return list;
			}
		}

		internal static ApplicationPool Find(ServerManager serverManager, string appPoolName)
		{
			foreach (ApplicationPool applicationPool in serverManager.ApplicationPools)
			{
				if (string.Equals(appPoolName, applicationPool.Name, StringComparison.OrdinalIgnoreCase))
				{
					return applicationPool;
				}
			}
			return null;
		}

		internal static Process[] GetRunningProcessesForAppPool(ServerManager serverManager, string appPoolName)
		{
			ApplicationPoolCollection applicationPools = serverManager.ApplicationPools;
			foreach (ApplicationPool applicationPool in applicationPools)
			{
				if (appPoolName.Equals(applicationPool.Name, StringComparison.OrdinalIgnoreCase))
				{
					WorkerProcessCollection workerProcesses = applicationPool.WorkerProcesses;
					List<Process> list = new List<Process>();
					foreach (WorkerProcess workerProcess in workerProcesses)
					{
						if (workerProcess.State == 1)
						{
							Process processById = Process.GetProcessById(workerProcess.ProcessId);
							if (processById != null)
							{
								list.Add(processById);
							}
						}
					}
					return list.ToArray();
				}
			}
			return new Process[0];
		}

		internal void Initialize()
		{
			if (this.serverManager == null)
			{
				this.serverManager = new ServerManager();
			}
			if (this.appPool == null)
			{
				this.appPool = ApplicationPoolHelper.Find(this.serverManager, this.appPoolName);
			}
			if (this.appPool == null)
			{
				throw new InvalidOperationException("Application pool can't be located");
			}
		}

		internal void Refresh()
		{
			if (this.serverManager != null)
			{
				this.serverManager.Dispose();
				this.serverManager = null;
				this.appPool = null;
			}
			this.Initialize();
		}

		internal void Recycle()
		{
			this.Initialize();
			bool flag = false;
			ObjectState objectState;
			if (this.appPool.State == 1)
			{
				objectState = this.appPool.Recycle();
			}
			else
			{
				flag = true;
				objectState = this.appPool.Start();
			}
			if (objectState != null && objectState != 1)
			{
				throw new InvalidOperationException(string.Format("Failed to recycle application pool (poolName={0}, state={1}, startAttempted={2})", this.appPoolName, objectState, flag));
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ApplicationPoolHelper>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && this.serverManager != null)
				{
					this.serverManager.Dispose();
				}
			}
		}

		private readonly string appPoolName;

		private ServerManager serverManager;

		private ApplicationPool appPool;
	}
}
