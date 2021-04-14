using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessManager<IComInterface> : DisposeTrackableBase
	{
		public ComProcessManager(int maxWorkerProcessNumber, ComWorkerConfiguration workerConfiguration, Trace tracer)
		{
			if (maxWorkerProcessNumber <= 0)
			{
				throw new ArgumentException("Invalid max work process count", "maxWorkerProcessNumber");
			}
			if (workerConfiguration == null)
			{
				throw new ArgumentNullException("Invalid worker configuration", "workerConfiguration");
			}
			this.workerConfiguration = workerConfiguration;
			this.tracer = tracer;
			this.semaphore = new Semaphore(maxWorkerProcessNumber, maxWorkerProcessNumber);
			try
			{
				this.jobObjectManager = new JobObjectManager(this.workerConfiguration.WorkerMemoryLimit);
			}
			catch (Win32Exception inner)
			{
				throw new ComProcessManagerInitializationException("ComProcessManager initialization fails because fail to intialize job object manager.", inner);
			}
			this.jobObjectManager.Tracer = this.tracer;
			JobObjectManager jobObjectManager = this.jobObjectManager;
			jobObjectManager.CallbackMonitorEvent = (CallbackMontiorEvent)Delegate.Combine(jobObjectManager.CallbackMonitorEvent, new CallbackMontiorEvent(this.JobObjectMonitorEventCallback));
			this.currentProcessHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccess.Synchronize, true, NativeMethods.GetCurrentProcessId());
			if (workerConfiguration.RunAsLocalService)
			{
				bool flag = NativeMethods.LogonUser("LocalService", "NT AUTHORITY", null, 5, 0, out this.workerProcessToken);
				if (!flag || this.workerProcessToken.IsInvalid)
				{
					throw new ComProcessManagerInitializationException("ComProcessManager initialization fails because fail to logon LocalService account.", new Win32Exception());
				}
			}
			this.workerProcessMap = new Dictionary<int, ComProcessAgent<IComInterface>>(maxWorkerProcessNumber);
			if (this.workerConfiguration.WorkerAllocationTimeout != 0 || this.workerConfiguration.WorkerIdleTimeout != 0)
			{
				this.processMonitorTimer = new Timer(new TimerCallback(this.ProcessTimerCallback), this, 5000, 5000);
			}
			this.freeWorkers = new List<ComProcessAgent<IComInterface>>();
		}

		public bool ExecuteRequest(object requestParameters)
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException("ComProcessManager");
			}
			ComProcessAgent<IComInterface> comProcessAgent = null;
			bool flag = true;
			if (!this.semaphore.WaitOne(this.workerConfiguration.WorkerAllocationTimeout, false))
			{
				this.TraceError(this, "Unable to obtain a worker to perform a request", new object[0]);
				throw new ComProcessBusyException("No workers available for execution");
			}
			bool result;
			try
			{
				lock (this.LockObject)
				{
					while (this.freeWorkers.Count > 0)
					{
						comProcessAgent = this.freeWorkers[0];
						this.freeWorkers.RemoveAt(0);
						if (comProcessAgent.IsValid)
						{
							break;
						}
						comProcessAgent.Dispose();
						comProcessAgent = null;
					}
				}
				if (comProcessAgent == null)
				{
					comProcessAgent = new ComProcessAgent<IComInterface>(this.workerConfiguration, this.jobObjectManager, this.currentProcessHandle, this.workerProcessToken, this.CreateWorkerCallback, requestParameters, new EventHandler<EventArgs>(this.OnWorkerProcessTerminated), this.tracer);
					lock (this.LockObject)
					{
						this.workerProcessMap[comProcessAgent.ProcessId] = comProcessAgent;
					}
				}
				bool flag4 = comProcessAgent.ExecuteRequest(this.ExecuteRequestCallback, requestParameters);
				flag = false;
				if (flag4)
				{
					if (comProcessAgent.IsExpiredLifetimeOrTransactionCount(ExDateTime.Now.LocalTime))
					{
						if (this.DestroyWorkerCallback != null)
						{
							this.DestroyWorkerCallback(comProcessAgent, requestParameters, false);
						}
						comProcessAgent.TerminateWorkerProcess(false);
					}
					else
					{
						lock (this.LockObject)
						{
							this.freeWorkers.Insert(0, comProcessAgent);
						}
					}
				}
				result = flag4;
			}
			finally
			{
				this.semaphore.Release();
				if (flag && this.DestroyWorkerCallback != null)
				{
					this.DestroyWorkerCallback(null, requestParameters, true);
				}
			}
			return result;
		}

		private void JobObjectMonitorEventCallback(MonitorEvent monitorEvent, params object[] args)
		{
			switch (monitorEvent)
			{
			case MonitorEvent.MonitorStart:
				this.TraceInfo(this, "Memory monitor thread starting.", new object[0]);
				return;
			case MonitorEvent.MonitorStop:
				this.TraceInfo(this, "Memory monitor thread stopping.", new object[0]);
				return;
			case MonitorEvent.ReachMemoryLimitation:
			{
				ComProcessAgent<IComInterface> comProcessAgent = null;
				int num = (int)args[0];
				this.TraceError(this, "Process {0} exceeded the memory limitation, will be killed", new object[]
				{
					num
				});
				lock (this.LockObject)
				{
					if (this.workerProcessMap.TryGetValue(num, out comProcessAgent))
					{
						this.workerProcessMap.Remove(num);
					}
				}
				if (comProcessAgent != null)
				{
					comProcessAgent.IsWorkerBeyondMemoryLimit = true;
					comProcessAgent.TerminateWorkerProcess(true);
					return;
				}
				break;
			}
			default:
				this.TraceError(this, "Invalid Job object event {0}.", new object[]
				{
					monitorEvent
				});
				break;
			}
		}

		private void OnWorkerProcessTerminated(object sender, EventArgs args)
		{
			ComProcessAgent<IComInterface> comProcessAgent = (ComProcessAgent<IComInterface>)sender;
			if (comProcessAgent != null)
			{
				lock (this.LockObject)
				{
					this.workerProcessMap.Remove(comProcessAgent.ProcessId);
					this.freeWorkers.Remove(comProcessAgent);
				}
				comProcessAgent.Dispose();
			}
		}

		private void ProcessTimerCallback(object state)
		{
			List<ComProcessAgent<IComInterface>> list = null;
			DateTime localTime = ExDateTime.Now.LocalTime;
			lock (this.LockObject)
			{
				for (int i = this.freeWorkers.Count - 1; i >= 0; i--)
				{
					ComProcessAgent<IComInterface> comProcessAgent = this.freeWorkers[i];
					if (comProcessAgent.IsExpiredLifetimeOrIdleTime(localTime))
					{
						this.freeWorkers.RemoveAt(i);
						if (list == null)
						{
							list = new List<ComProcessAgent<IComInterface>>();
						}
						list.Add(comProcessAgent);
					}
				}
				if (list != null)
				{
					foreach (ComProcessAgent<IComInterface> comProcessAgent2 in list)
					{
						if (this.DestroyWorkerCallback != null)
						{
							this.DestroyWorkerCallback(comProcessAgent2, null, false);
						}
						comProcessAgent2.TerminateWorkerProcess(false);
					}
				}
			}
		}

		internal ComWorkerConfiguration WorkerConfiguration
		{
			get
			{
				return this.workerConfiguration;
			}
		}

		private object LockObject
		{
			get
			{
				return this.workerProcessMap;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				for (int i = this.freeWorkers.Count - 1; i >= 0; i--)
				{
					this.freeWorkers[i].TerminateWorkerProcess(false);
				}
				if (this.jobObjectManager != null)
				{
					this.jobObjectManager.Dispose();
					this.jobObjectManager = null;
				}
				if (this.currentProcessHandle != null)
				{
					this.currentProcessHandle.Dispose();
					this.currentProcessHandle = null;
				}
				if (this.workerProcessToken != null)
				{
					this.workerProcessToken.Dispose();
					this.workerProcessToken = null;
				}
				if (this.semaphore != null)
				{
					this.semaphore.Close();
					this.semaphore = null;
				}
				if (this.processMonitorTimer != null)
				{
					this.processMonitorTimer.Dispose();
					this.processMonitorTimer = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ComProcessManager<IComInterface>>(this);
		}

		internal void TraceInfo(object target, string formatString, params object[] args)
		{
			if (this.tracer != null)
			{
				this.tracer.Information((long)((target != null) ? target.GetHashCode() : 0), formatString, args);
			}
		}

		private void TraceError(object target, string formatString, params object[] args)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceError((long)((target != null) ? target.GetHashCode() : 0), formatString, args);
			}
		}

		private const int ProcessMonitorTimerFrequency = 5000;

		public ComProcessManager<IComInterface>.OnCreateWorker CreateWorkerCallback;

		public ComProcessManager<IComInterface>.OnDestroyWorker DestroyWorkerCallback;

		public ComProcessManager<IComInterface>.OnExecuteRequest ExecuteRequestCallback;

		private JobObjectManager jobObjectManager;

		private SafeProcessHandle currentProcessHandle;

		private SafeUserTokenHandle workerProcessToken;

		private Semaphore semaphore;

		private List<ComProcessAgent<IComInterface>> freeWorkers;

		private Dictionary<int, ComProcessAgent<IComInterface>> workerProcessMap;

		private Trace tracer;

		private ComWorkerConfiguration workerConfiguration;

		private Timer processMonitorTimer;

		public delegate void OnCreateWorker(IComWorker<IComInterface> worker, object requestParameters);

		public delegate bool OnExecuteRequest(IComWorker<IComInterface> worker, object requestParameters);

		public delegate void OnDestroyWorker(IComWorker<IComInterface> worker, object requestParameters, bool forcedTermination);
	}
}
