using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ProcessManager
{
	internal abstract class ProcessManagerService : ExServiceBase, IDisposeTrackable, IDisposable
	{
		public ProcessManagerService(string serviceName, string workerProcessPathName, string jobObjectName, bool canHandleConnectionsIfPassive, int workerProcessExitTimeout, bool runningAsService, Microsoft.Exchange.Diagnostics.Trace tracer, ExEventLog eventLogger)
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.InstanceCreated, ExDateTime.UtcNow);
			this.diagnostics = tracer;
			this.eventLogger = eventLogger;
			this.runningAsService = runningAsService;
			ProcessManagerService processManagerService = Interlocked.CompareExchange<ProcessManagerService>(ref ProcessManagerService.instance, this, null);
			if (processManagerService != null)
			{
				throw new InvalidOperationException("ProcessManagerService is a singleton.");
			}
			base.ServiceName = serviceName;
			base.CanStop = true;
			base.CanShutdown = true;
			base.AutoLog = false;
			this.workerProcessPathName = workerProcessPathName;
			this.jobObjectName = jobObjectName;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public abstract bool CanHandleConnectionsIfPassive { get; }

		public virtual int MaxWorkerProcessExitTimeoutDefault
		{
			get
			{
				return 90;
			}
		}

		public virtual int MaxWorkerProcessDumpTimeoutDefault
		{
			get
			{
				return 900;
			}
		}

		internal ServiceConfiguration ServiceConfiguration
		{
			get
			{
				return this.serviceConfiguration;
			}
		}

		internal int MaxConnectionRate
		{
			get
			{
				return this.maxConnectionRate;
			}
			set
			{
				this.maxConnectionRate = value;
				if (this.tcpListener != null)
				{
					this.tcpListener.MaxConnectionRate = value;
				}
			}
		}

		internal int StopListenerAndWorkerCalled
		{
			get
			{
				return this.stopListenerAndWorkerCalled;
			}
		}

		public static bool StopService(bool canRetry, bool retryAlways, bool retryImmediately, WorkerInstance workerInstance)
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.StopService, canRetry ? 1 : 0);
			ProcessManagerService processManagerService = ProcessManagerService.instance;
			if (canRetry && processManagerService.startState == StartState.Started)
			{
				if (retryAlways || processManagerService.retryAttempts < processManagerService.serviceConfiguration.MaxProcessManagerRestartAttempts)
				{
					TimeSpan timeSpan = retryImmediately ? ProcessManagerService.ImmediateRestartWorkerProcessInterval : ProcessManagerService.RestartWorkerProcessInterval;
					processManagerService.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRestartScheduled, null, new object[]
					{
						(int)timeSpan.TotalMinutes
					});
					processManagerService.diagnostics.TraceError<int, int>(0L, "Restarting the worker process manager in {0} minutes. It has been restarted {1} times.", (int)timeSpan.TotalMinutes, processManagerService.retryAttempts);
					processManagerService.retryAttempts++;
					if (processManagerService.tcpListener != null)
					{
						processManagerService.tcpListener.StopListening();
					}
					processManagerService.workerProcessManager.Stop(processManagerService.serviceConfiguration.MaxWorkerProcessExitTimeout, processManagerService.serviceConfiguration.MaxWorkerProcessDumpTimeout);
					processManagerService.restartTimer = new Timer(new TimerCallback(processManagerService.RestartTimerCallback), null, (int)timeSpan.TotalMilliseconds, -1);
					return false;
				}
			}
			else if (canRetry && workerInstance != null && processManagerService.startState == StartState.Starting && processManagerService.retryAttempts < processManagerService.serviceConfiguration.MaxProcessRestartAttemptsWhileInStartingState)
			{
				processManagerService.retryAttempts++;
				if (processManagerService.workerProcessManager.RetryWorkerProcess(workerInstance))
				{
					return false;
				}
			}
			ProcessManagerService.StopService();
			return true;
		}

		public static void StopService()
		{
			if (ProcessManagerService.instance == null)
			{
				throw new NullReferenceException("Instance of ProcessManagerService does not exist");
			}
			if (ProcessManagerService.instance.SetStopServiceCalled() == 1)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug(0L, "StopService will be skipped because it is already called");
				return;
			}
			if (!ProcessManagerService.instance.runningAsService)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug(0L, "StopService called (running interactive)");
				Environment.Exit(1);
				return;
			}
			if (ProcessManagerService.instance.startState != StartState.None)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug(0L, "StopService called (running as a service)");
				ProcessManagerService.instance.Stop();
				return;
			}
			ProcessManagerService.instance.diagnostics.TraceDebug(0L, "StopService called (running as a service, not started yet)");
			Environment.Exit(1);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.restartTimer != null)
				{
					this.restartTimer.Dispose();
					this.restartTimer = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
			base.Dispose(disposing);
		}

		internal bool CheckCommonSwapThresholds(Process process, int refreshInterval, int maxThreads, long maxWorkingSet)
		{
			int num = -1;
			try
			{
				num = process.Id;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			if (!this.CheckRefreshIntervalOkay(process, num, refreshInterval))
			{
				return true;
			}
			if (maxThreads == 0 && maxWorkingSet == 0L)
			{
				return false;
			}
			Process process2 = null;
			bool result;
			try
			{
				try
				{
					process2 = Process.GetProcessById(num);
				}
				catch (ArgumentException)
				{
				}
				if (process2 == null)
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshProcessData, null, new object[]
					{
						num
					});
					this.diagnostics.TraceDebug<int>(0L, "Process {0}: failed to retrieve process data, so refresh", num);
					result = true;
				}
				else
				{
					int threadCount = 0;
					long workingSet = 0L;
					try
					{
						threadCount = process2.Threads.Count;
						workingSet = process2.WorkingSet64;
					}
					catch (InvalidOperationException ex)
					{
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshProcessDataFetchFailed, null, new object[]
						{
							num,
							ex
						});
						this.diagnostics.TraceDebug<int, InvalidOperationException>(0L, "Process {0}: failed to retrieve thread or working set information, so refresh. Exception {1}", num, ex);
						return true;
					}
					if (!this.CheckMaxThreadsOkay(num, maxThreads, threadCount))
					{
						result = true;
					}
					else if (!this.CheckMaxWorkingSetOkay(num, maxWorkingSet, workingSet))
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			finally
			{
				if (process2 != null)
				{
					process2.Close();
				}
			}
			return result;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ProcessManagerService>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		protected virtual bool Initialize()
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.Initialize, ExDateTime.UtcNow);
			try
			{
				this.serviceConfiguration = ServiceConfiguration.Load(this);
			}
			catch (ConfigurationErrorsException ex)
			{
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_AppConfigLoadFailed, null, new object[]
				{
					ex.ToString()
				});
				return false;
			}
			bool flag = this.CreateJobObject(this.jobObjectName);
			return flag && this.stopServiceCalled == 0;
		}

		protected abstract bool GetBindings(out IPEndPoint[] bindings);

		protected virtual void SetJobObjectExtendedInfo(ref NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION jobExtendedInfo)
		{
			jobExtendedInfo.ProcessMemoryLimit = UIntPtr.Zero;
			jobExtendedInfo.JobMemoryLimit = UIntPtr.Zero;
			jobExtendedInfo.PeakProcessMemoryUsed = UIntPtr.Zero;
			jobExtendedInfo.PeakJobMemoryUsed = UIntPtr.Zero;
			jobExtendedInfo.BasicLimitInformation.LimitFlags = 8192U;
		}

		protected virtual JobObjectUILimit GetJobObjectUIRestrictions()
		{
			return JobObjectUILimit.Handles | JobObjectUILimit.ReadClipboard | JobObjectUILimit.SystemParameters | JobObjectUILimit.WriteClipboard | JobObjectUILimit.Desktop | JobObjectUILimit.DisplaySettings | JobObjectUILimit.ExitWindows | JobObjectUILimit.GlobalAtoms;
		}

		protected virtual bool TryReadServerConfig()
		{
			return true;
		}

		protected virtual void RegisterWorkerEvents(WorkerProcessManager workerProcessManager)
		{
		}

		protected virtual void UnregisterWorkerEvents(WorkerProcessManager workerProcessManager)
		{
		}

		protected override void OnStartInternal(string[] args)
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnStartInternalEnter, ExDateTime.UtcNow);
			try
			{
				if (this.stopServiceCalled != 1)
				{
					this.startState = StartState.Starting;
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						this.diagnostics.TracePfd<int, string, DateTime>(0L, "PFD ETS {0} Starting {1} ({2})", 24599, base.ServiceName, DateTime.UtcNow);
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStartAttempt, null, new object[]
						{
							currentProcess.Id
						});
						if (this.runningAsService)
						{
							base.RequestAdditionalTime((int)ProcessManagerService.StartupTimeoutInterval.TotalMilliseconds - (int)TimeSpan.FromSeconds(30.0).TotalMilliseconds);
						}
						if (!this.TryReadServerConfig())
						{
							this.diagnostics.TraceError(0L, "TryReadServerConfig failed. The service is stopping.");
							ProcessManagerService.StopService();
						}
						else
						{
							this.SetMaxIOThreadsLimit();
							this.SetupWorkerProcessManager(this.workerProcessPathName);
							if (this.workerProcessManager == null)
							{
								this.diagnostics.TraceError(0L, "SetupWorkerProcessManager failed. The service is stopping.");
								ProcessManagerService.StopService();
							}
							else
							{
								ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.WorkerProcessSetupSuccess, ExDateTime.UtcNow);
								while (!this.workerProcessManager.HasWorkerEverContacted)
								{
									if (this.workerProcessManager.IsStoppedOrStopping())
									{
										this.diagnostics.TraceError(0L, "WorkerProcessManager is stopped before start completed");
										return;
									}
									Thread.Sleep((int)ProcessManagerService.SleepInterval.TotalMilliseconds);
								}
								if (this.serviceConfiguration.ServiceListening)
								{
									this.tcpListener = new TcpListener(new TcpListener.HandleFailure(ProcessManagerService.OnTcpListenerFailure), new TcpListener.HandleConnection(this.workerProcessManager.HandleConnection), null, this.diagnostics, this.eventLogger, this.maxConnectionRate, false, false);
									this.GetAndSetBindings(true);
									this.tcpListener.StartListening(true);
								}
								this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStartedSuccessFully, null, new object[]
								{
									currentProcess.Id
								});
								this.diagnostics.TracePfd<int, string>(0L, "PFD ETS {0} Started Successfully {1}", 19959, base.ServiceName);
								this.startState = StartState.Started;
							}
						}
					}
				}
			}
			finally
			{
				ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnStartInternalExit, (int)this.startState, ExDateTime.UtcNow);
			}
		}

		protected override void OnStopInternal()
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnStopInternalEnter, ExDateTime.UtcNow);
			try
			{
				this.diagnostics.TraceDebug(0L, "OnStopInternal is called");
				this.StopListenerAndWorker();
			}
			finally
			{
				ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnStopInternalExit, ExDateTime.UtcNow);
			}
		}

		protected override void OnShutdownInternal()
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnShutdownInternalEnter, ExDateTime.UtcNow);
			try
			{
				this.diagnostics.TraceDebug(0L, "OnShutdownInternal is called");
				this.StopListenerAndWorker();
			}
			finally
			{
				ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnShutdownInternalExit, ExDateTime.UtcNow);
			}
		}

		protected override void OnCustomCommandInternal(int command)
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.OnCustomCommandInternalEnter, command);
			this.diagnostics.TraceDebug<int>(0L, "OnCustomCommandInternal is called, with command {0}.", command);
			if (this.workerProcessManager == null)
			{
				this.diagnostics.TraceError<int>(0L, "OnCustomCommandInternal: no worker process manager.", command);
				return;
			}
			switch (command)
			{
			case 200:
				if (this.workerProcessManager.IsRunning())
				{
					this.workerProcessManager.InitiateRefresh();
					return;
				}
				this.InitiateRestart();
				return;
			case 201:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnConfigUpdate();
				return;
			case 202:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnMemoryPressure();
				return;
			case 203:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnClearConfigCache();
				return;
			case 204:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnBlockedSubmissionQueue();
				return;
			case 205:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnClearKerberosTicketCache();
				return;
			case 206:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnLogFlush();
				return;
			case 207:
				if (!this.workerProcessManager.IsRunning())
				{
					return;
				}
				this.OnForcedCrash();
				return;
			default:
				return;
			}
		}

		protected virtual void OnConfigUpdate()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateConfigUpdate();
			}
		}

		protected virtual void OnMemoryPressure()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateMemoryPressureHandler();
			}
		}

		protected virtual void OnLogFlush()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateLogFlush();
			}
		}

		protected virtual void OnClearConfigCache()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateClearConfigCache();
			}
		}

		protected virtual void OnBlockedSubmissionQueue()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateSubmissionQueueBlockedHandler();
			}
		}

		protected virtual void OnForcedCrash()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateForcedCrash();
			}
		}

		protected virtual void OnClearKerberosTicketCache()
		{
		}

		protected override void OnPauseInternal()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiatePause();
			}
		}

		protected override void OnContinueInternal()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.InitiateContinue();
			}
		}

		protected override void OnCommandTimeout()
		{
			if (this.workerProcessManager != null)
			{
				this.workerProcessManager.WorkerInstancesTriggerDumpAndWait(this.serviceConfiguration.MaxWorkerProcessDumpTimeout);
				if (this.startState == StartState.Starting && this.workerProcessManager.HasWorkerCrashed)
				{
					ProcessManagerService.instance.diagnostics.TraceError(0L, "Worker crashed during startup. No need to generate service dump.");
					Environment.Exit(1);
				}
			}
		}

		protected void StopListenerAndWorker()
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.StopListenerAndWorkerEnter, ExDateTime.UtcNow);
			try
			{
				if (this.tcpListener != null)
				{
					this.tcpListener.ProcessStopping = true;
				}
				if (Interlocked.Exchange(ref this.stopListenerAndWorkerCalled, 1) == 1)
				{
					while (!this.stopListenerAndWorkerCompleted)
					{
						Thread.Sleep((int)ProcessManagerService.SleepInterval.TotalMilliseconds);
					}
				}
				else
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						this.diagnostics.TraceDebug<string>(0L, "Stopping {0}", base.ServiceName);
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStopAttempt, null, new object[]
						{
							currentProcess.Id
						});
						if (this.tcpListener != null)
						{
							this.tcpListener.StopListening();
							this.tcpListener.Shutdown();
						}
						WorkerProcessManager workerProcessManager = this.workerProcessManager;
						if (workerProcessManager != null)
						{
							if (workerProcessManager.IsRunning())
							{
								workerProcessManager.Stop(this.serviceConfiguration.MaxWorkerProcessExitTimeout, this.serviceConfiguration.MaxWorkerProcessDumpTimeout);
							}
							this.UnregisterWorkerEvents(workerProcessManager);
						}
						this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_ServiceStopped, null, new object[]
						{
							currentProcess.Id
						});
						this.diagnostics.TraceDebug<string>(0L, "Stopped {0}", base.ServiceName);
						this.stopListenerAndWorkerCompleted = true;
					}
				}
			}
			finally
			{
				ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.StopListenerAndWorkerExit, ExDateTime.UtcNow);
			}
		}

		protected void GetAndSetBindings(bool stopOnFailure)
		{
			IPEndPoint[] newBindings = null;
			if (!this.GetBindings(out newBindings))
			{
				if (stopOnFailure)
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_BindingConfigNotFound, null, new object[0]);
					ProcessManagerService.StopService();
				}
				return;
			}
			if (this.tcpListener != null)
			{
				this.tcpListener.SetBindings(newBindings, stopOnFailure);
			}
		}

		protected bool CreateJobObject(string jobObjectName)
		{
			this.jobObject = NativeMethods.CreateJobObject(IntPtr.Zero, jobObjectName);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (this.jobObject.IsInvalid)
			{
				this.diagnostics.TraceError<string, int>(0L, "CreateJobObject(name: {0}) failed: {1}", jobObjectName, lastWin32Error);
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_CreateJobObjectFailed, null, new object[]
				{
					lastWin32Error.ToString(),
					jobObjectName
				});
				return false;
			}
			if (lastWin32Error == 183)
			{
				this.diagnostics.TraceWarning<string>(0L, "CreateJobObject(name: {0}) returned existing jobObject", jobObjectName);
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_CreatedExistingJobObject, null, new object[]
					{
						jobObjectName,
						currentProcess.Id
					});
				}
			}
			try
			{
				NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedLimits = default(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION);
				this.SetJobObjectExtendedInfo(ref extendedLimits);
				this.jobObject.SetExtendedLimits(extendedLimits);
				JobObjectUILimit jobObjectUIRestrictions = this.GetJobObjectUIRestrictions();
				this.jobObject.SetUIRestrictions(jobObjectUIRestrictions);
			}
			catch (Win32Exception ex)
			{
				this.diagnostics.TraceError<int>(0L, "SetInformationJobObject() failed: {0}", ex.NativeErrorCode);
				this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_SetJobObjectFailed, null, new object[]
				{
					ex.Message,
					jobObjectName
				});
				this.jobObject.Close();
				return false;
			}
			return true;
		}

		private void RestartTimerCallback(object obj)
		{
			this.diagnostics.TraceDebug(0L, "RestartTimerCallback invoked.");
			this.InitiateRestart();
		}

		private void InitiateRestart()
		{
			ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.InitiateRestartEnter, ExDateTime.UtcNow);
			try
			{
				if (this.stopListenerAndWorkerCalled == 1 || this.stopServiceCalled == 1)
				{
					this.diagnostics.TraceError(0L, "Ignore restart because stop service has been requested.");
				}
				else if (!this.workerProcessManager.ReInit())
				{
					this.diagnostics.TraceError(0L, "Unable to restart because the worker process manager is not stopped.");
				}
				else
				{
					this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_RestartWorkerProcess, null, new object[0]);
					this.diagnostics.TraceError(0L, "Restarting the worker process manager.");
					if (!this.workerProcessManager.Start())
					{
						this.diagnostics.TraceError(0L, "WorkerProcessManager.Start() failed. The service is stopping.");
						ProcessManagerService.StopService();
					}
					else
					{
						int i = 0;
						bool flag = false;
						while (i < (int)ProcessManagerService.StartupWaitInterval.TotalMilliseconds / (int)ProcessManagerService.SleepInterval.TotalMilliseconds)
						{
							flag = this.workerProcessManager.IsReady();
							if (flag)
							{
								break;
							}
							if (this.workerProcessManager.IsStoppedOrStopping())
							{
								this.diagnostics.TraceError(0L, "WorkerProcessManager is stopped before restart completed");
								return;
							}
							Thread.Sleep((int)ProcessManagerService.SleepInterval.TotalMilliseconds);
							i++;
						}
						if (!flag)
						{
							this.diagnostics.TraceError(0L, "InitiateRestart failed as the service did not become ready. The service is stopping.");
							ProcessManagerService.StopService();
						}
						else
						{
							this.diagnostics.TraceDebug(0L, "Restart is successful");
							if (this.tcpListener != null)
							{
								this.tcpListener.StartListening(false);
							}
							this.retryAttempts = 0;
						}
					}
				}
			}
			finally
			{
				ProcessManagerService.DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs.InitiateRestartExit, ExDateTime.UtcNow);
			}
		}

		private bool WorkerProcessManagerStopServiceHandler(bool canRetry, bool retryAlways, bool retryImmediately, WorkerInstance workerProcess)
		{
			this.diagnostics.TraceDebug(0L, "WorkerProcessManagerStopServiceHandler: service request to stop the service");
			return ProcessManagerService.StopService(canRetry, retryAlways, retryImmediately, workerProcess);
		}

		private bool SetupWorkerProcessManager(string workerPath)
		{
			try
			{
				this.workerProcessManager = new WorkerProcessManager(workerPath, new WorkerProcessManager.ProcessNeedsSwapCheckDelegate(this.CheckProcessSwap), this.jobObject, this.diagnostics, this.eventLogger, new WorkerProcessManager.StopServiceHandler(this.WorkerProcessManagerStopServiceHandler));
			}
			catch (ArgumentException arg)
			{
				this.diagnostics.TraceError<ArgumentException>(0L, "Failed to create the WorkerProcessManager. Exception: {0}", arg);
				this.workerProcessManager = null;
			}
			if (this.workerProcessManager != null)
			{
				this.RegisterWorkerEvents(this.workerProcessManager);
				if (!this.workerProcessManager.Start())
				{
					this.workerProcessManager = null;
				}
			}
			return this.workerProcessManager != null;
		}

		private void SetMaxIOThreadsLimit()
		{
			int workerThreads;
			int arg;
			ThreadPool.GetMaxThreads(out workerThreads, out arg);
			ThreadPool.SetMaxThreads(workerThreads, this.serviceConfiguration.MaxIOThreads);
			this.diagnostics.TraceDebug<int, int>(0L, "Setting max I/O threads from {0} to {1}", arg, this.serviceConfiguration.MaxIOThreads);
		}

		private bool CheckProcessSwap(Process process)
		{
			return this.CheckCommonSwapThresholds(process, this.serviceConfiguration.MaxWorkerProcessRefreshInterval, this.serviceConfiguration.MaxWorkerProcessThreads, this.serviceConfiguration.MaxWorkerProcessWorkingSet);
		}

		private bool CheckRefreshIntervalOkay(Process process, int pid, int refreshInterval)
		{
			if (refreshInterval == 0)
			{
				return true;
			}
			DateTime localTime = ExDateTime.Now.LocalTime;
			DateTime dateTime = process.StartTime.AddSeconds((double)refreshInterval);
			if (dateTime > localTime)
			{
				return true;
			}
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshInterval, null, new object[]
			{
				pid,
				refreshInterval.ToString()
			});
			this.diagnostics.TraceDebug(0L, "Process {0}: threshold 'RefreshInterval' exceeded: startTime={1}, retireTime=={2}, now={3}", new object[]
			{
				pid,
				process.StartTime,
				dateTime,
				localTime
			});
			return false;
		}

		private bool CheckMaxThreadsOkay(int pid, int maxThreads, int threadCount)
		{
			if (maxThreads == 0)
			{
				return true;
			}
			if (threadCount <= maxThreads)
			{
				return true;
			}
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshMaxThread, null, new object[]
			{
				pid,
				threadCount.ToString(),
				maxThreads.ToString()
			});
			this.diagnostics.TraceDebug<int, int, int>(0L, "Process {0}: threshold 'MaxThreads' exceeded: threadCount={1}, maxThreads={2}", pid, threadCount, maxThreads);
			return false;
		}

		private bool CheckMaxWorkingSetOkay(int pid, long maxWorkingSet, long workingSet)
		{
			if (maxWorkingSet == 0L)
			{
				return true;
			}
			if (workingSet < maxWorkingSet)
			{
				return true;
			}
			this.eventLogger.LogEvent(ProcessManagerServiceEventLogConstants.Tuple_WorkerProcessRefreshMaxWorkingSet, null, new object[]
			{
				pid,
				workingSet.ToString(),
				maxWorkingSet.ToString()
			});
			this.diagnostics.TraceDebug<int, long, long>(0L, "Process {0}: threshold 'MaxWorkingSet' exceeded: workingSet={1}, MaxWorkingSet=={2}", pid, workingSet, maxWorkingSet);
			return false;
		}

		private int SetStopServiceCalled()
		{
			return Interlocked.Exchange(ref this.stopServiceCalled, 1);
		}

		private static void DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs bc, int data)
		{
			if (ProcessManagerService.instance != null && ProcessManagerService.instance.diagnostics != null)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug<ProcessManagerService.ProcessManagerBreadcrumbs, int>(0L, "ProcessManagerService.DropBreadCrumb: {0}, {1}", bc, data);
			}
			ProcessManagerService.DropBreadCrumb((ProcessManagerService.ProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedInt((int)bc, data));
		}

		private static void DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs bc, int data, ExDateTime time)
		{
			if (ProcessManagerService.instance != null && ProcessManagerService.instance.diagnostics != null)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug<ProcessManagerService.ProcessManagerBreadcrumbs, int, ExDateTime>(0L, "ProcessManagerService.DropBreadCrumb: {0}, {1}, {2}", bc, data, time);
			}
			ProcessManagerService.ProcessManagerBreadcrumbs encodedInt = (ProcessManagerService.ProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedInt((int)bc, data);
			ProcessManagerService.DropBreadCrumb((ProcessManagerService.ProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedDateTime((int)encodedInt, time));
		}

		private static void DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs bc, ExDateTime time)
		{
			if (ProcessManagerService.instance != null && ProcessManagerService.instance.diagnostics != null)
			{
				ProcessManagerService.instance.diagnostics.TraceDebug<ProcessManagerService.ProcessManagerBreadcrumbs, ExDateTime>(0L, "ProcessManagerService.DropBreadCrumb: {0}, {1}", bc, time);
			}
			ProcessManagerService.DropBreadCrumb((ProcessManagerService.ProcessManagerBreadcrumbs)ProcessManagerService.GetEncodedDateTime((int)bc, time));
		}

		private static void DropBreadCrumb(ProcessManagerService.ProcessManagerBreadcrumbs bc)
		{
			ProcessManagerService.breadcrumbs.Drop(bc);
		}

		public static int GetEncodedDateTime(int bc, ExDateTime time)
		{
			return bc + time.Minute * 100 + time.Second;
		}

		public static int GetEncodedInt(int bc, int data)
		{
			if (data < 0 || data >= 100)
			{
				return bc + 990000;
			}
			return bc + data * 10000;
		}

		private static void OnTcpListenerFailure(bool addressAlreadyInUseFailure)
		{
			ProcessManagerService.StopService();
		}

		public const string TransportServiceName = "MSExchangeTransport";

		private const long TraceId = 0L;

		public const int NumberOfBreadcrumbs = 128;

		public static ProcessManagerService instance;

		protected Microsoft.Exchange.Diagnostics.Trace diagnostics;

		protected ExEventLog eventLogger;

		protected StartState startState;

		protected int stopServiceCalled;

		private DisposeTracker disposeTracker;

		private static readonly TimeSpan StartupTimeoutInterval = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan RestartWorkerProcessInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan ImmediateRestartWorkerProcessInterval = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan StartupWaitInterval = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan SleepInterval = TimeSpan.FromMilliseconds(250.0);

		private string workerProcessPathName;

		private string jobObjectName;

		private Timer restartTimer;

		private int retryAttempts;

		private ServiceConfiguration serviceConfiguration;

		private WorkerProcessManager workerProcessManager;

		private TcpListener tcpListener;

		private bool runningAsService;

		private SafeJobHandle jobObject;

		private int maxConnectionRate = 1200;

		private int stopListenerAndWorkerCalled;

		private bool stopListenerAndWorkerCompleted;

		private static Breadcrumbs<ProcessManagerService.ProcessManagerBreadcrumbs> breadcrumbs = new Breadcrumbs<ProcessManagerService.ProcessManagerBreadcrumbs>(128);

		public enum CustomCommands
		{
			InitiateRefresh = 200,
			InitiateConfigUpdate,
			InitiateHandleMemoryPressure,
			InitiateClearConfigCache,
			InitiateHandleSubmissionQueueBlocked,
			InitiateClearKerberosTicketCache,
			InitiateLogFlush,
			InitiateForceCrash
		}

		private enum ProcessManagerBreadcrumbs
		{
			InstanceCreated = 1000000,
			StopService = 2000000,
			Initialize = 3000000,
			OnStartInternalEnter = 4000000,
			WorkerProcessSetupSuccess = 5000000,
			OnStartInternalExit = 6000000,
			OnStopInternalEnter = 7000000,
			OnStopInternalExit = 8000000,
			OnCustomCommandInternalEnter = 9000000,
			InitiateRestartEnter = 10000000,
			InitiateRestartExit = 11000000,
			OnShutdownInternalEnter = 12000000,
			OnShutdownInternalExit = 13000000,
			StopListenerAndWorkerEnter = 14000000,
			StopListenerAndWorkerExit = 15000000
		}
	}
}
