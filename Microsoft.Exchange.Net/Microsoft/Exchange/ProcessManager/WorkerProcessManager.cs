using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.ProcessManager
{
	internal class WorkerProcessManager : DisposeTrackableBase
	{
		public event WorkerProcessManager.DisconnectPerformanceCountersHandler OnDisconnectPerformanceCounters;

		public event WorkerProcessManager.WorkerEventHandler OnWorkerContacted;

		public event WorkerProcessManager.WorkerEventHandler OnWorkerExited;

		internal WorkerProcessManager(string workerPath, WorkerProcessManager.ProcessNeedsSwapCheckDelegate processNeedsSwapCheckDelegate, SafeJobHandle jobObject, Microsoft.Exchange.Diagnostics.Trace tracer, ExEventLog eventLogger, WorkerProcessManager.StopServiceHandler stopServiceHandler)
		{
			this.diag = tracer;
			this.eventLogger = eventLogger;
			this.processNeedsSwapCheckDelegate = processNeedsSwapCheckDelegate;
			this.jobObject = jobObject;
			this.stopServiceHandler = stopServiceHandler;
			if (workerPath == null || !File.Exists(workerPath))
			{
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerImagePathNotExist, null, new object[]
				{
					workerPath
				});
				this.diag.TraceError<string>(0L, "Path to worker process is invalid: {0}", workerPath);
				throw new ArgumentException("Path to worker process is invalid", "workerPath");
			}
			this.workerPath = workerPath;
			this.socketInformationMemoryStream = new MemoryStream();
		}

		internal bool HasWorkerCrashed
		{
			get
			{
				return this.hasWorkerCrashed;
			}
		}

		internal bool HasWorkerEverContacted
		{
			get
			{
				return this.hasWorkerEverContacted;
			}
		}

		private bool SetState(WorkerProcessManager.States newState)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.SetState, (int)newState);
			if (this.state == WorkerProcessManager.States.Stopped && newState != WorkerProcessManager.States.Init)
			{
				return false;
			}
			this.state = newState;
			return true;
		}

		internal bool ReInit()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.ReInit, ExDateTime.UtcNow);
			bool result;
			lock (this)
			{
				if (this.state != WorkerProcessManager.States.Stopped)
				{
					result = false;
				}
				else if (ProcessManagerService.instance.StopListenerAndWorkerCalled == 1)
				{
					result = false;
				}
				else
				{
					result = this.SetState(WorkerProcessManager.States.Init);
				}
			}
			return result;
		}

		internal bool Start()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StartEnter, ExDateTime.UtcNow);
			bool result;
			try
			{
				this.diag.TracePfd<int>(0L, "PFD ETS {0} Starting an instance of Worker Process", 20503);
				lock (this)
				{
					if (this.state != WorkerProcessManager.States.Init)
					{
						this.diag.TraceDebug<int>(0L, "Cannot start process manager because state is not idle (state={0})", (int)this.state);
						result = false;
					}
					else if (!this.SetState(WorkerProcessManager.States.Idle))
					{
						result = false;
					}
					else
					{
						GC.Collect();
						GC.WaitForPendingFinalizers();
						this.activeWorker = this.StartWorkerInstance(0, false);
						if (this.monitorTimer == null)
						{
							this.monitorTimer = new Timer(new TimerCallback(this.MonitorInstancesTimer), null, 60000, 60000);
						}
						result = (this.activeWorker != null);
					}
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StartExit, ExDateTime.UtcNow);
			}
			return result;
		}

		internal void StopInstance(WorkerInstance workerInstance)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StopInstanceEnter, ExDateTime.UtcNow);
			this.diag.TraceDebug(0L, "WorkerProcessManager.StopInstance invoked.");
			try
			{
				workerInstance.Stop();
				int maxWorkerProcessExitTimeout = ProcessManagerService.instance.ServiceConfiguration.MaxWorkerProcessExitTimeout;
				int num = 0;
				int num2 = 1;
				bool flag = false;
				while (!workerInstance.Exited)
				{
					if (maxWorkerProcessExitTimeout > 0 && !flag && num >= maxWorkerProcessExitTimeout)
					{
						workerInstance.SignalHang();
						flag = true;
					}
					num += num2;
					Thread.Sleep(num2 * 1000);
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StopInstanceExit, ExDateTime.UtcNow);
			}
		}

		internal void Stop(int workerProcessExitTimeout, int workerProcessDumpTimeout)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StopEnter, ExDateTime.UtcNow);
			try
			{
				this.diag.TraceDebug<int>(0L, "Stop (state={0})", (int)this.state);
				this.TraceWorkers();
				List<WorkerInstance> list = new List<WorkerInstance>();
				lock (this)
				{
					if (!this.SetState(WorkerProcessManager.States.Stopping))
					{
						return;
					}
					this.WaitForWorkerReady(this.activeWorker);
					this.WaitForWorkerReady(this.passiveWorker);
					if (this.activeWorker != null && this.activeWorker.SignaledReady && this.activeWorker.IsConnected)
					{
						this.activeWorker.Stop();
						list.Add(this.activeWorker);
					}
					if (this.passiveWorker != null && this.passiveWorker.SignaledReady && this.passiveWorker.IsConnected)
					{
						this.passiveWorker.Stop();
						list.Add(this.passiveWorker);
					}
					if (this.retiredWorker != null && this.retiredWorker.SignaledReady && this.retiredWorker.IsConnected)
					{
						this.retiredWorker.Stop();
						list.Add(this.retiredWorker);
					}
				}
				int num = 0;
				int num2 = 1;
				bool flag2 = false;
				while (this.WorkerInstancesExist(list) && num < workerProcessDumpTimeout)
				{
					if (workerProcessExitTimeout > 0 && !flag2 && num >= workerProcessExitTimeout)
					{
						this.WorkerInstancesTriggerDump(list);
						flag2 = true;
					}
					num += num2;
					Thread.Sleep(num2 * 1000);
				}
				if (num >= workerProcessDumpTimeout)
				{
					this.TryKillWorkerInstances(list);
				}
				lock (this)
				{
					this.SetState(WorkerProcessManager.States.Stopped);
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.StopExit, ExDateTime.UtcNow);
			}
		}

		internal void WorkerInstancesTriggerDumpAndWait(int timeout)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.WorkerInstancesTriggerDumpAndWaitEnter, ExDateTime.UtcNow);
			List<WorkerInstance> list = new List<WorkerInstance>();
			WorkerInstance workerInstance = this.activeWorker;
			if (workerInstance != null)
			{
				list.Add(this.activeWorker);
			}
			workerInstance = this.passiveWorker;
			if (workerInstance != null)
			{
				list.Add(this.passiveWorker);
			}
			workerInstance = this.retiredWorker;
			if (workerInstance != null)
			{
				list.Add(this.retiredWorker);
			}
			this.WorkerInstancesTriggerDump(list);
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (this.WorkerInstancesExist(list) && stopwatch.Elapsed.TotalSeconds < (double)timeout)
			{
				Thread.Sleep(1000);
			}
			if (stopwatch.Elapsed.TotalSeconds >= (double)timeout)
			{
				this.TryKillWorkerInstances(list);
			}
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.WorkerInstancesTriggerDumpAndWaitExit, ExDateTime.UtcNow);
		}

		internal void InitiateRefresh()
		{
			WorkerInstance workerInstance = this.activeWorker;
			int arg = (workerInstance != null) ? workerInstance.Pid : 0;
			if (this.state != WorkerProcessManager.States.Idle)
			{
				this.diag.TraceDebug<int, int>(0L, "Ignore Refresh because state is not Idle (state={0}, active pid={1})", (int)this.state, arg);
				return;
			}
			this.diag.TraceDebug<int, int>(0L, "Initiate Refresh (state={0}, active pid={1})", (int)this.state, arg);
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshControlCommand, null, new object[]
			{
				arg.ToString()
			});
			this.RefreshStart();
		}

		internal void InitiatePause()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InitiatePauseEnter, ExDateTime.UtcNow);
			lock (this)
			{
				if (!this.isPaused)
				{
					this.isPaused = true;
					int arg = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
					this.diag.TraceDebug<int>(0L, "Initiate Pause (active pid={0})", arg);
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessPauseCommand, null, new object[]
					{
						arg.ToString()
					});
					this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.Pause);
				}
			}
		}

		internal void InitiateContinue()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InitiateContinueEnter, ExDateTime.UtcNow);
			lock (this)
			{
				if (this.isPaused)
				{
					this.isPaused = false;
					int arg = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
					this.diag.TraceDebug<int>(0L, "Initiate Continue (active pid={0})", arg);
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessContinueCommand, null, new object[]
					{
						arg.ToString()
					});
					this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.Continue);
				}
			}
		}

		internal void InitiateConfigUpdate()
		{
			lock (this)
			{
				this.refreshEnabled = true;
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.ConfigUpdate);
			}
		}

		internal void InitiateMemoryPressureHandler()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InitiateMemoryPressureHandlerEnter, ExDateTime.UtcNow);
			lock (this)
			{
				int num = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessForcedWatsonException, null, new object[]
				{
					num.ToString(),
					202
				});
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.HandleMemoryPressure);
			}
		}

		internal void InitiateLogFlush()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InitiateLogFlush, ExDateTime.UtcNow);
			lock (this)
			{
				if (this.activeWorker != null)
				{
					int pid = this.activeWorker.Pid;
				}
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.LogFlush);
			}
		}

		internal void InitiateClearConfigCache()
		{
			lock (this)
			{
				int num = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessClearConfigCache, null, new object[]
				{
					num.ToString(),
					203
				});
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.ClearConfigCache);
			}
		}

		internal void InitiateSubmissionQueueBlockedHandler()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InitiateSubmissionQueueBlockedHandlerEnter, ExDateTime.UtcNow);
			lock (this)
			{
				int num = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessForcedWatsonException, null, new object[]
				{
					num.ToString(),
					204
				});
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.HandleSubmissionQueueBlocked);
			}
		}

		internal void InitiateForcedCrash()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.ForcedCrash, ExDateTime.UtcNow);
			lock (this)
			{
				int num = (this.activeWorker != null) ? this.activeWorker.Pid : 0;
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessForcedWatsonException, null, new object[]
				{
					num.ToString(),
					207
				});
				this.SendCommand(WorkerProcessManager.ServiceToWorkerCommands.ForceCrash);
			}
		}

		internal bool HandleConnection(Socket connection)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			this.diag.TraceDebug(0L, "HandleConnection");
			try
			{
				while (num++ < 120)
				{
					WorkerInstance workerInstance = this.activeWorker;
					if (workerInstance != null)
					{
						num2 = workerInstance.Pid;
					}
					else
					{
						flag2 = true;
					}
					if (!flag2 && workerInstance != null && workerInstance.IsConnected)
					{
						this.diag.TraceDebug<int>(0L, "Try sending connection to pid={0}", num2);
						if (this.HandOverConnection(connection, num2, workerInstance))
						{
							flag = true;
							return true;
						}
						this.diag.TraceDebug<int>(0L, "Failed to duplicate handle into pid={0}", num2);
						this.StopInstance(workerInstance);
						return false;
					}
					else
					{
						if (this.IsStoppedOrStopping())
						{
							break;
						}
						if (num == 1)
						{
							Thread.Sleep(200);
						}
						else
						{
							Thread.Sleep(500);
						}
						flag2 = false;
					}
				}
			}
			finally
			{
				if (!flag)
				{
					this.diag.TraceDebug<IntPtr>(0L, "Close socket handle {0}", connection.Handle);
					connection.Close();
				}
			}
			return false;
		}

		internal bool IsReady()
		{
			WorkerInstance workerInstance = this.activeWorker;
			return workerInstance != null && workerInstance.IsConnected;
		}

		internal bool IsStoppedOrStopping()
		{
			return this.state == WorkerProcessManager.States.Stopping || this.state == WorkerProcessManager.States.Stopped;
		}

		internal bool IsRunning()
		{
			return this.state != WorkerProcessManager.States.Init && this.state != WorkerProcessManager.States.Stopping && this.state != WorkerProcessManager.States.Stopped;
		}

		internal bool RetryWorkerProcess(WorkerInstance workerInstance)
		{
			if (this.activeWorker == workerInstance || this.activeWorker == null)
			{
				this.CleanWorkerInstance(workerInstance);
				this.RestartActiveWorkerInstance(workerInstance);
				return true;
			}
			return false;
		}

		private bool WorkerInstancesExist(List<WorkerInstance> workers)
		{
			foreach (WorkerInstance workerInstance in workers)
			{
				if (!workerInstance.Exited)
				{
					this.diag.TraceDebug<int>(0L, "Worker instance pid={0} didn't exit yet", workerInstance.Pid);
					return true;
				}
				this.diag.TraceDebug<int>(0L, "Worker instance pid={0} exited", workerInstance.Pid);
			}
			return false;
		}

		internal void WorkerInstancesTriggerDump(List<WorkerInstance> workers)
		{
			foreach (WorkerInstance workerInstance in workers)
			{
				workerInstance.SignalHang();
			}
		}

		private bool IsStateRefesh()
		{
			return this.state == WorkerProcessManager.States.RefreshWaitingPassiveConnected || this.state == WorkerProcessManager.States.RefreshWaitingRetiredExit;
		}

		private void InternalReset()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InternalResetEnter, ExDateTime.UtcNow);
			try
			{
				lock (this)
				{
					if (!this.IsStoppedOrStopping())
					{
						if (this.retiredWorker != null)
						{
							WorkerInstance workerInstance = this.retiredWorker;
							this.retiredWorker = null;
							try
							{
								workerInstance.Process.Kill();
							}
							catch (InvalidOperationException)
							{
							}
						}
						if (this.passiveWorker != null)
						{
							WorkerInstance workerInstance = this.passiveWorker;
							this.passiveWorker = null;
							try
							{
								workerInstance.Process.Kill();
							}
							catch (InvalidOperationException)
							{
							}
						}
						if (this.activeWorker != null)
						{
							WorkerInstance workerInstance = this.activeWorker;
							this.activeWorker = null;
							try
							{
								workerInstance.Process.Kill();
							}
							catch (InvalidOperationException)
							{
							}
						}
						if (this.SetState(WorkerProcessManager.States.Idle))
						{
							GC.Collect();
							GC.WaitForPendingFinalizers();
							this.activeWorker = this.StartWorkerInstance(0, false);
						}
					}
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.InternalResetExit, ExDateTime.UtcNow);
			}
		}

		private void MonitorInstancesTimer(object state)
		{
			if (Interlocked.Exchange(ref this.monitorBusy, 1) != 0)
			{
				return;
			}
			try
			{
				lock (this)
				{
					int arg = 0;
					WorkerInstance workerInstance = this.activeWorker;
					if (workerInstance != null)
					{
						arg = workerInstance.Pid;
					}
					this.diag.TraceDebug<int>(0L, "Monitor the worker instance (pid={0})", arg);
					if (this.refreshEnabled && !this.IsStateRefesh() && workerInstance != null && workerInstance.Process != null && this.processNeedsSwapCheckDelegate != null && this.processNeedsSwapCheckDelegate(workerInstance.Process))
					{
						this.diag.TraceDebug(0L, "Start refresh");
						this.RefreshStart();
					}
				}
			}
			finally
			{
				Interlocked.Exchange(ref this.monitorBusy, 0);
			}
		}

		private WorkerInstance StartWorkerInstance(int thrashCrashCount, bool startAsPassive)
		{
			this.HandlePerformanceCountersDisconnection();
			WorkerInstance workerInstance = new WorkerInstance(startAsPassive, new WorkerInstance.WorkerContacted(this.HandleWorkerContacted), new WorkerInstance.WorkerExited(this.HandleWorkerExited), thrashCrashCount, this.diag);
			if (!workerInstance.Start(this.workerPath, this.isPaused, ProcessManagerService.instance.ServiceConfiguration.ServiceListening, this.jobObject))
			{
				workerInstance = null;
				this.diag.TraceError(0L, "Failed to start the worker process");
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerStartFailed, null, new object[]
				{
					this.workerPath
				});
				ProcessManagerService.StopService();
			}
			else
			{
				this.diag.TracePfd<int, int>(0L, "PFD ETS {0} Started a new instance pid={1}", 28695, workerInstance.Pid);
			}
			return workerInstance;
		}

		private void AsyncStartActiveWorker(object context)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.AsyncStartActiveWorkerEnter, ExDateTime.UtcNow);
			WorkerInstance workerInstance = (WorkerInstance)context;
			if (!this.WaitForProcessHandleClose(workerInstance.Pid))
			{
				this.diag.TraceDebug(0L, "Cannot verify the old handle is closed.");
			}
			lock (this)
			{
				if (this.IsStoppedOrStopping())
				{
					this.diag.TraceDebug(0L, "AsyncStartActiveWorker: Skipping because process manager is stopped.");
				}
				else
				{
					this.diag.TraceDebug(0L, "AsyncStartActiveWorker: prepare to force GC collection and then start active worker instance");
					int thrashCrashCount = 0;
					if (!workerInstance.IsConnected)
					{
						thrashCrashCount = workerInstance.ThrashCrashCount + 1;
					}
					this.activeWorker = this.StartWorkerInstance(thrashCrashCount, false);
				}
			}
		}

		private void HandlePerformanceCountersDisconnection()
		{
			if (ProcessManagerService.instance.ServiceConfiguration.DisconnectTransportPerformanceCounters)
			{
				WorkerProcessManager.DisconnectPerformanceCountersHandler onDisconnectPerformanceCounters = this.OnDisconnectPerformanceCounters;
				if (onDisconnectPerformanceCounters != null)
				{
					onDisconnectPerformanceCounters();
				}
			}
		}

		private void RestartActiveWorkerInstance(WorkerInstance workerInstance)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RestartActiveWorkerInstanceEnter, workerInstance.ThrashCrashCount, ExDateTime.UtcNow);
			this.diag.TraceDebug<string>(0L, "RestartWorkerInstance worker instance: {0}", this.workerPath);
			int num = 0;
			if (!workerInstance.IsConnected)
			{
				num = workerInstance.ThrashCrashCount + 1;
				this.diag.TraceDebug(0L, "Worker instance crashed immediately: pid={0} thrashCrashCount={1}, start time={2}, connected={3}", new object[]
				{
					workerInstance.Pid,
					num,
					workerInstance.StartTime,
					workerInstance.IsConnected
				});
			}
			int thrashCrashMaximum = ProcessManagerService.instance.ServiceConfiguration.ThrashCrashMaximum;
			if (num > thrashCrashMaximum)
			{
				this.diag.TraceError<string, int, int>(0L, "Worker instance keeps crashing on startup: {0}, thrashCrashCount={1}, maximum={2}", this.workerPath, num, thrashCrashMaximum);
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerStartThrashCount, null, new object[]
				{
					this.workerPath
				});
				ProcessManagerService.StopService();
				return;
			}
			this.diag.TraceError(0L, "Schedule a AsyncStartActiveWorker callback");
			this.asyncRestartTimer = new Timer(new TimerCallback(this.AsyncStartActiveWorker), workerInstance, 250, -1);
		}

		private void TryKillWorkerInstances(List<WorkerInstance> workers)
		{
			foreach (WorkerInstance workerInstance in workers)
			{
				if (!workerInstance.Exited)
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_AttemptToKillWorkerProcess, null, new object[]
					{
						workerInstance.Pid
					});
					try
					{
						workerInstance.Process.Kill();
					}
					catch (Win32Exception exception)
					{
						this.LogFailedKill(workerInstance.Pid, exception);
					}
					catch (NotSupportedException exception2)
					{
						this.LogFailedKill(workerInstance.Pid, exception2);
					}
					catch (InvalidOperationException exception3)
					{
						this.LogFailedKill(workerInstance.Pid, exception3);
					}
				}
			}
		}

		private void LogFailedKill(int pid, Exception exception)
		{
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_FailedToKillWorkerProcess, null, new object[]
			{
				pid,
				exception.Message
			});
		}

		private void TraceWorkers()
		{
			string arg = "";
			WorkerInstance workerInstance = this.activeWorker;
			if (workerInstance != null)
			{
				arg = string.Format("pid={0}", workerInstance.Pid);
			}
			this.diag.TraceDebug<string>(0L, "active worker: {0}", arg);
			arg = "";
			workerInstance = this.passiveWorker;
			if (workerInstance != null)
			{
				arg = string.Format("pid={0}", workerInstance.Pid);
			}
			this.diag.TraceDebug<string>(0L, "passive worker: {0}", arg);
			arg = "";
			workerInstance = this.retiredWorker;
			if (workerInstance != null)
			{
				arg = string.Format("pid={0}", workerInstance.Pid);
			}
			this.diag.TraceDebug<string>(0L, "retired worker: {0}", arg);
		}

		private void RefreshStart()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshStartEnter, ExDateTime.UtcNow);
			try
			{
				this.diag.TraceDebug<int>(0L, "RefreshStart (state={0})", (int)this.state);
				this.TraceWorkers();
				lock (this)
				{
					if (!this.IsStoppedOrStopping())
					{
						if (this.activeWorker == null)
						{
							this.diag.TraceDebug(0L, "Skipping refresh because there is no active worker");
						}
						else if (this.SetState(WorkerProcessManager.States.RefreshWaitingPassiveConnected))
						{
							this.passiveWorker = this.StartWorkerInstance(0, true);
							this.TraceWorkers();
						}
					}
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshStartExit, ExDateTime.UtcNow);
			}
		}

		private void RefreshWaitingPassiveConnected()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshWaitingPassiveConnected, ExDateTime.UtcNow);
			this.diag.TraceDebug<int>(0L, "RefreshWaitingPassiveConnected (state={0})", (int)this.state);
			lock (this)
			{
				this.TraceWorkers();
				if (!this.IsStoppedOrStopping())
				{
					WorkerInstance workerInstance = this.activeWorker;
					if (workerInstance != null)
					{
						this.SendMessage(workerInstance, WorkerProcessManager.ServiceToWorkerCommands.Retire, 0, WorkerProcessManager.ServiceToWorkerCommands.Retire.Length);
						this.retiredWorker = workerInstance;
						this.retiredWorker.IsActive = false;
						this.activeWorker = null;
						if (this.SetState(WorkerProcessManager.States.RefreshWaitingRetiredExit))
						{
							this.TraceWorkers();
						}
					}
					else
					{
						this.retiredWorker = null;
						this.RefreshWaitingRetiredExit();
					}
				}
			}
		}

		private void AsyncRefreshWaitingRetiredExit(object context)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.AsyncRefreshWaitingRetiredExitEnter, ExDateTime.UtcNow);
			WorkerInstance workerInstance = (WorkerInstance)context;
			if (!this.WaitForProcessHandleClose(workerInstance.Pid))
			{
				this.diag.TraceDebug(0L, "Cannot verify the old handle is closed.");
			}
			lock (this)
			{
				if (this.IsStoppedOrStopping())
				{
					this.diag.TraceDebug(0L, "AsyncRefreshWaitingRetiredExit: Skipping because process manager is stopped.");
				}
				else
				{
					this.diag.TraceDebug(0L, "AsyncRefreshWaitingRetiredExit: prepare to force GC collection and then start active worker instance");
					this.RefreshWaitingRetiredExit();
				}
			}
		}

		private void RefreshWaitingRetiredExit()
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshWaitingRetiredExitEnter, ExDateTime.UtcNow);
			this.diag.TraceDebug<int>(0L, "RefreshWaitingRetiredExit (state={0})", (int)this.state);
			this.TraceWorkers();
			lock (this)
			{
				if (!this.IsStoppedOrStopping())
				{
					WorkerInstance workerInstance = this.passiveWorker;
					this.passiveWorker = null;
					this.SendMessage(workerInstance, WorkerProcessManager.ServiceToWorkerCommands.Activate, 0, WorkerProcessManager.ServiceToWorkerCommands.Activate.Length);
					workerInstance.IsActive = true;
					this.activeWorker = workerInstance;
					if (this.SetState(WorkerProcessManager.States.Idle))
					{
						this.diag.TraceDebug<int>(0L, "Refresh done (state={0})", (int)this.state);
						this.TraceWorkers();
					}
				}
			}
		}

		private void RefreshExited(WorkerInstance workerInstance)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshExitedEnter, (int)this.state, ExDateTime.UtcNow);
			try
			{
				this.diag.TraceDebug<int, int>(0L, "RefreshExited (state={0}, pid={1})", (int)this.state, workerInstance.Pid);
				this.TraceWorkers();
				if (this.state == WorkerProcessManager.States.RefreshWaitingRetiredExit)
				{
					if (workerInstance == this.retiredWorker)
					{
						this.retiredWorker = null;
						this.diag.TraceError(0L, "Schedule a AsyncRefreshWaitingRetiredExit callback");
						this.asyncRestartTimer = new Timer(new TimerCallback(this.AsyncRefreshWaitingRetiredExit), workerInstance, 250, -1);
					}
					else if (this.activeWorker == null && this.passiveWorker == workerInstance)
					{
						lock (this)
						{
							if (!this.IsStoppedOrStopping())
							{
								if (this.SetState(WorkerProcessManager.States.Idle))
								{
									this.activeWorker = this.StartWorkerInstance(0, true);
								}
							}
						}
					}
				}
				else if (workerInstance == this.passiveWorker)
				{
					if (this.state == WorkerProcessManager.States.RefreshWaitingRetiredExit)
					{
						this.diag.TraceDebug<int>(0L, "Unhandled case is RefreshExited (state={0}): reset", (int)this.state);
						this.InternalReset();
					}
					else if (this.state == WorkerProcessManager.States.RefreshWaitingPassiveConnected)
					{
						this.diag.TraceDebug<int>(0L, "Unhandled case is RefreshExited (state={0}): disable refresh", (int)this.state);
						lock (this)
						{
							if (!this.IsStoppedOrStopping())
							{
								if (this.SetState(WorkerProcessManager.States.Idle))
								{
									this.refreshEnabled = false;
								}
							}
						}
					}
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.RefreshExitedExit, (int)this.state, ExDateTime.UtcNow);
			}
		}

		private void HandleWorkerContacted(WorkerInstance workerInstance)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.HandleWorkerContactedEnter, ExDateTime.UtcNow);
			this.diag.TraceDebug<int>(0L, "Worker process (pid={0}) contacted.", workerInstance.Pid);
			this.hasWorkerEverContacted = true;
			if (this.state == WorkerProcessManager.States.RefreshWaitingPassiveConnected)
			{
				this.RefreshWaitingPassiveConnected();
			}
			if (this.OnWorkerContacted != null)
			{
				this.OnWorkerContacted();
			}
		}

		private void SaveLastWorkerProcessId(WorkerInstance workerInstance)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport"))
				{
					if (registryKey.GetValue("LastWorkerProcessId") != null)
					{
						registryKey.DeleteValue("LastWorkerProcessId", false);
					}
					registryKey.SetValue("LastWorkerProcessId", workerInstance.Pid, RegistryValueKind.DWord);
				}
			}
			catch (Exception ex)
			{
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_FailedToStoreLastWorkerProcessId, null, new object[]
				{
					ex.ToString()
				});
			}
		}

		private void HandleWorkerExited(WorkerInstance workerInstance, bool resetRequested)
		{
			this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.HandleWorkerExitedEnter, resetRequested ? 1 : 0, ExDateTime.UtcNow);
			try
			{
				string stdOutText = workerInstance.StdOutText;
				string stdErrText = workerInstance.StdErrText;
				this.diag.TraceDebug(0L, "Worker process (pid={0}) {1}.\nStdOut: {2}\nStdErr: {3}", new object[]
				{
					workerInstance.Pid,
					resetRequested ? "requested reset (crashed)" : "exited",
					stdOutText,
					stdErrText
				});
				this.SaveLastWorkerProcessId(workerInstance);
				int num = 0;
				if (workerInstance.Process != null && workerInstance.Process.HasExited)
				{
					num = workerInstance.Process.ExitCode;
					this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.WorkerProcessExitCode, num);
				}
				if (!resetRequested)
				{
					string text;
					workerInstance.CloseProcess(out text);
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ExistingWorkerProcessHasExitedValue, null, new object[]
					{
						text
					});
				}
				if (num == 196)
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessExitServiceTerminateWithUnhandledException, null, new object[]
					{
						workerInstance.Pid.ToString(CultureInfo.InvariantCulture)
					});
					throw new WorkerProcessRequestedAbnormalTerminationException("Worker process requested that the service terminate with unhandled exception on machine " + Environment.MachineName);
				}
				if (num == 199)
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessExitServiceStop, null, new object[]
					{
						workerInstance.Pid.ToString(CultureInfo.InvariantCulture)
					});
					if (this.stopServiceHandler != null)
					{
						this.stopServiceHandler(false, false, false, workerInstance);
					}
					Environment.Exit(0);
				}
				else
				{
					if (this.activeWorker == workerInstance && (num == 198 || num == 197 || num == 200))
					{
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessExitServiceStop, null, new object[]
						{
							workerInstance.Pid.ToString(CultureInfo.InvariantCulture)
						});
						lock (this)
						{
							this.CleanWorkerInstance(workerInstance);
						}
						if (this.stopServiceHandler != null && this.stopServiceHandler(true, num == 197 || num == 200, num == 200, workerInstance))
						{
							Environment.Exit(0);
						}
						return;
					}
					if (resetRequested)
					{
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessReplace, null, new object[]
						{
							workerInstance.Pid.ToString(CultureInfo.InvariantCulture)
						});
					}
					else
					{
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessExit, null, new object[]
						{
							workerInstance.Pid.ToString(CultureInfo.InvariantCulture)
						});
					}
				}
				lock (this)
				{
					if (this.IsStoppedOrStopping())
					{
						this.CleanWorkerInstance(workerInstance);
						return;
					}
					if (resetRequested && this.crashingWorker != null)
					{
						this.diag.TraceDebug<int, int>(0L, "Worker process (pid={0}) requested reset, but another reset is still ongoing (pid={1}). Ignore request.", workerInstance.Pid, this.crashingWorker.Pid);
						return;
					}
					if (!resetRequested && this.crashingWorker == workerInstance)
					{
						this.diag.TraceDebug<int>(0L, "Worker process (pid={0}) finished reset.", workerInstance.Pid);
						this.crashingWorker = null;
					}
					if (resetRequested)
					{
						this.diag.TraceDebug<int>(0L, "Worker process (pid={0}) starting reset.", workerInstance.Pid);
						this.crashingWorker = workerInstance;
					}
					if (this.state == WorkerProcessManager.States.RefreshWaitingPassiveConnected || this.state == WorkerProcessManager.States.RefreshWaitingRetiredExit)
					{
						this.RefreshExited(workerInstance);
					}
					else if (this.activeWorker == workerInstance || this.activeWorker == null)
					{
						this.CleanWorkerInstance(workerInstance);
						this.hasWorkerCrashed = true;
						this.RestartActiveWorkerInstance(workerInstance);
					}
					this.CleanWorkerInstance(workerInstance);
					this.TraceWorkers();
				}
				if (this.OnWorkerExited != null)
				{
					this.OnWorkerExited();
				}
			}
			finally
			{
				this.DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs.HandleWorkerExitedExit, ExDateTime.UtcNow);
			}
		}

		private void CleanWorkerInstance(WorkerInstance workerInstance)
		{
			lock (this)
			{
				if (workerInstance == this.activeWorker)
				{
					this.activeWorker = null;
				}
				else if (workerInstance == this.passiveWorker)
				{
					this.passiveWorker = null;
				}
				else if (workerInstance == this.retiredWorker)
				{
					this.retiredWorker = null;
				}
			}
		}

		private void SendCommand(byte[] command)
		{
			this.diag.TraceDebug<byte[]>(0L, "Sending Command {0}", command);
			lock (this)
			{
				if (this.activeWorker != null)
				{
					this.SendMessage(this.activeWorker, command, 0, command.Length);
				}
				if (this.passiveWorker != null)
				{
					this.SendMessage(this.passiveWorker, command, 0, command.Length);
				}
			}
		}

		private bool SendMessage(WorkerInstance workerInstance, byte[] buffer, int offset, int count)
		{
			bool result = true;
			try
			{
				Monitor.Enter(workerInstance);
				if (!workerInstance.SendMessage(buffer, offset, count))
				{
					result = false;
				}
			}
			finally
			{
				if (Monitor.IsEntered(workerInstance))
				{
					Monitor.Exit(workerInstance);
				}
			}
			return result;
		}

		private bool HandOverConnection(Socket connection, int destinationPid, WorkerInstance workerInstance)
		{
			SocketInformation socketInformation;
			try
			{
				socketInformation = connection.DuplicateAndClose(destinationPid);
			}
			catch (SocketException ex)
			{
				this.diag.TraceDebug<int>(0L, "DuplicateAndCloseHandle failed with error code: {0}", ex.ErrorCode);
				return false;
			}
			bool result;
			lock (this.socketInformationMemoryStream)
			{
				this.socketInformationMemoryStream.Position = 0L;
				this.socketInformationMemoryStream.WriteByte(78);
				this.socketInfoFormatter.Serialize(this.socketInformationMemoryStream, socketInformation);
				result = this.SendMessage(workerInstance, this.socketInformationMemoryStream.GetBuffer(), 0, (int)this.socketInformationMemoryStream.Position);
			}
			return result;
		}

		private bool WaitForProcessHandleClose(int pid)
		{
			for (int i = 0; i < WorkerProcessManager.CheckProcessHandleMaxAttempts; i++)
			{
				lock (this)
				{
					if (this.IsStoppedOrStopping())
					{
						return false;
					}
				}
				if (this.VerifyProcessHandleClosed(pid))
				{
					this.diag.TraceDebug<int>(0L, "Handle for process {0} is closed", pid);
					return true;
				}
				this.diag.TraceDebug<int>(0L, "Handle for process {0} is not closed. Wait for 0.25 seconds.", pid);
				Thread.Sleep(250);
			}
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WaitForProcessHandleCloseTimedOut, null, new object[]
			{
				pid.ToString(),
				ProcessManagerService.instance.ServiceConfiguration.CheckProcessHandleTimeOut
			});
			return false;
		}

		private void WaitForWorkerReady(WorkerInstance worker)
		{
			if (worker == null)
			{
				return;
			}
			for (int i = 0; i < 30; i++)
			{
				if (worker.SignaledReady)
				{
					this.diag.TraceDebug<int>(0L, "Process {0} has already signaled ready.", worker.Pid);
					return;
				}
				this.diag.TraceDebug<int>(0L, "Wait for process {0} to signal ready. Sleep 1 second", worker.Pid);
				Thread.Sleep(1000);
			}
		}

		private bool VerifyProcessHandleClosed(int pid)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			bool result;
			using (SafeProcessHandle safeProcessHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccess.QueryInformation, false, (uint)pid))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				this.diag.TraceDebug<int>(0L, "OpenProcess finishes with error code: {0}", lastWin32Error);
				if (lastWin32Error == 87 && (safeProcessHandle == null || safeProcessHandle.IsInvalid))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs bc, int data)
		{
			this.DropBreadCrumb((WorkerProcessManager.WorkerProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedInt((int)bc, data));
		}

		private void DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs bc, int data, ExDateTime time)
		{
			int encodedInt = ProcessManagerService.GetEncodedInt((int)bc, data);
			WorkerProcessManager.WorkerProcessManagerBreadcrumbs encodedDateTime = (WorkerProcessManager.WorkerProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedDateTime(encodedInt, time);
			this.DropBreadCrumb(encodedDateTime);
		}

		private void DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs bc, ExDateTime time)
		{
			WorkerProcessManager.WorkerProcessManagerBreadcrumbs encodedDateTime = (WorkerProcessManager.WorkerProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedDateTime((int)bc, time);
			this.DropBreadCrumb(encodedDateTime);
		}

		private void DropBreadCrumb(WorkerProcessManager.WorkerProcessManagerBreadcrumbs bc)
		{
			this.breadcrumbs.Drop(bc);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.asyncRestartTimer != null)
			{
				this.asyncRestartTimer.Dispose();
				this.asyncRestartTimer = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WorkerProcessManager>(this);
		}

		private const int CheckProcessHandleSleepInterval = 250;

		private const long TraceId = 0L;

		private const int FirstSendRetryInterval = 200;

		private const int SendRetryInterval = 500;

		private const int MaxSendRetryCount = 120;

		private const int MonitorIntervalSec = 60;

		private const int ShutdownWorkerReadyWaitSec = 30;

		public const int TerminateServiceWithException = 196;

		public const int RetryServiceAlwaysStopProcessExitCode = 197;

		public const int RetryableServiceStopProcessExitCode = 198;

		public const int ServiceStopProcessExitCode = 199;

		public const int ImmediateRetryServiceAlwaysStopProcessExitCode = 200;

		private WorkerProcessManager.StopServiceHandler stopServiceHandler;

		private static readonly int CheckProcessHandleMaxAttempts = ProcessManagerService.instance.ServiceConfiguration.CheckProcessHandleTimeOut * 1000 / 250;

		private WorkerProcessManager.ProcessNeedsSwapCheckDelegate processNeedsSwapCheckDelegate;

		private IFormatter socketInfoFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);

		private WorkerProcessManager.States state;

		private WorkerInstance activeWorker;

		private WorkerInstance passiveWorker;

		private WorkerInstance retiredWorker;

		private WorkerInstance crashingWorker;

		private bool refreshEnabled = true;

		private bool isPaused;

		private string workerPath;

		private SafeJobHandle jobObject;

		private Microsoft.Exchange.Diagnostics.Trace diag;

		private ExEventLog eventLogger;

		private MemoryStream socketInformationMemoryStream;

		private Timer monitorTimer;

		private int monitorBusy;

		private Timer asyncRestartTimer;

		private bool hasWorkerCrashed;

		private bool hasWorkerEverContacted;

		private Breadcrumbs<WorkerProcessManager.WorkerProcessManagerBreadcrumbs> breadcrumbs = new Breadcrumbs<WorkerProcessManager.WorkerProcessManagerBreadcrumbs>(128);

		public delegate bool ProcessNeedsSwapCheckDelegate(Process process);

		public delegate bool StopServiceHandler(bool canRetry, bool retryAlways, bool retryImmediately, WorkerInstance workerProcess);

		public delegate void DisconnectPerformanceCountersHandler();

		public delegate void WorkerEventHandler();

		private enum States
		{
			Init,
			Idle,
			RefreshWaitingPassiveConnected,
			RefreshWaitingRetiredExit,
			Stopping,
			Stopped
		}

		private enum WorkerProcessManagerBreadcrumbs
		{
			SetState = 1000000,
			ReInit = 2000000,
			StartEnter = 3000000,
			StartExit = 4000000,
			StopInstanceEnter = 5000000,
			StopInstanceExit = 6000000,
			StopEnter = 7000000,
			StopExit = 8000000,
			WorkerInstancesTriggerDumpAndWaitEnter = 9000000,
			WorkerInstancesTriggerDumpAndWaitExit = 10000000,
			InitiatePauseEnter = 11000000,
			InitiateContinueEnter = 12000000,
			InitiateMemoryPressureHandlerEnter = 13000000,
			InitiateSubmissionQueueBlockedHandlerEnter = 14000000,
			RestartActiveWorkerInstanceEnter = 15000000,
			RefreshStartEnter = 16000000,
			RefreshStartExit = 17000000,
			RefreshWaitingPassiveConnected = 18000000,
			AsyncRefreshWaitingRetiredExitEnter = 19000000,
			RefreshWaitingRetiredExitEnter = 20000000,
			RefreshExitedEnter = 21000000,
			RefreshExitedExit = 22000000,
			HandleWorkerExitedEnter = 23000000,
			WorkerProcessExitCode = 24000000,
			HandleWorkerExitedExit = 25000000,
			InternalResetEnter = 26000000,
			InternalResetExit = 27000000,
			AsyncStartActiveWorkerEnter = 28000000,
			HandleWorkerContactedEnter = 29000000,
			InitiateLogFlush = 30000000,
			ForcedCrash = 31000000
		}

		private class ServiceToWorkerCommands
		{
			public const byte NewConnection = 78;

			public static readonly byte[] Retire = new byte[]
			{
				82
			};

			public static readonly byte[] Activate = new byte[]
			{
				65
			};

			public static readonly byte[] ConfigUpdate = new byte[]
			{
				85
			};

			public static readonly byte[] Pause = new byte[]
			{
				80
			};

			public static readonly byte[] Continue = new byte[]
			{
				67
			};

			public static readonly byte[] HandleMemoryPressure = new byte[]
			{
				77
			};

			public static readonly byte[] HandleSubmissionQueueBlocked = new byte[]
			{
				81
			};

			public static readonly byte[] ClearConfigCache = new byte[]
			{
				76
			};

			public static readonly byte[] ForceCrash = new byte[]
			{
				87
			};

			public static readonly byte[] LogFlush = new byte[]
			{
				70
			};
		}
	}
}
