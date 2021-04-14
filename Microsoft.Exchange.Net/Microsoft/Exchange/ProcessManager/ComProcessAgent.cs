using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessAgent<IComInterface> : DisposeTrackableBase, IComWorker<IComInterface>
	{
		private event EventHandler<EventArgs> WorkerProcessTerminatedEvent;

		public IComInterface Worker
		{
			get
			{
				base.CheckDisposed();
				return this.worker;
			}
		}

		public SafeProcessHandle SafeProcessHandle
		{
			get
			{
				base.CheckDisposed();
				return this.workerProcess;
			}
		}

		public int ProcessId
		{
			get
			{
				base.CheckDisposed();
				return this.workerProcessId;
			}
		}

		internal bool IsValid
		{
			get
			{
				return !base.IsDisposed && this.worker != null && this.workerProcess != null && !this.workerProcess.IsInvalid && !this.workerProcess.HasExited;
			}
		}

		internal bool IsWorkerBeyondMemoryLimit
		{
			get
			{
				return this.isWorkerBeyondMemoryLimit;
			}
			set
			{
				this.isWorkerBeyondMemoryLimit = value;
			}
		}

		internal ComProcessAgent(ComWorkerConfiguration configuration, JobObjectManager jobObjectManger, SafeProcessHandle currentProcessHandle, SafeUserTokenHandle workerProcessToken, ComProcessManager<IComInterface>.OnCreateWorker createWorker, object requestParameter, EventHandler<EventArgs> workerProcessTerminatedEventHandler, Trace tracer)
		{
			this.configuration = configuration;
			this.jobObjectManger = jobObjectManger;
			this.currentProcessHandle = currentProcessHandle;
			this.workerProcessToken = workerProcessToken;
			this.WorkerProcessTerminatedEvent += workerProcessTerminatedEventHandler;
			this.tracer = tracer;
			this.LaunchWorkerProcess(createWorker, requestParameter);
		}

		private void LaunchWorkerProcess(ComProcessManager<IComInterface>.OnCreateWorker createWorker, object requestParameters)
		{
			bool flag = false;
			try
			{
				this.InternalLaunchWorkProcess(createWorker, requestParameters);
				flag = true;
			}
			catch (Win32Exception e)
			{
				this.LaunchWorkProcessFailed(e);
			}
			catch (ArgumentException e2)
			{
				this.LaunchWorkProcessFailed(e2);
			}
			catch (NotSupportedException e3)
			{
				this.LaunchWorkProcessFailed(e3);
			}
			catch (MemberAccessException e4)
			{
				this.LaunchWorkProcessFailed(e4);
			}
			catch (InvalidComObjectException e5)
			{
				this.LaunchWorkProcessFailed(e5);
			}
			catch (COMException e6)
			{
				this.LaunchWorkProcessFailed(e6);
			}
			catch (TypeLoadException e7)
			{
				this.LaunchWorkProcessFailed(e7);
			}
			finally
			{
				if (!flag)
				{
					this.TerminateWorkerProcess(true);
				}
			}
		}

		private void InternalLaunchWorkProcess(ComProcessManager<IComInterface>.OnCreateWorker createWorker, object requestParameters)
		{
			Guid guid = Guid.Empty;
			string workerProcessPath = this.WorkerConfiguration.WorkerProcessPath;
			EventWaitHandle eventWaitHandle = null;
			EventWaitHandle eventWaitHandle2 = null;
			guid = Guid.NewGuid();
			string eventName = string.Format("Local\\Local_{0}-AddToJobObject", guid);
			eventWaitHandle = this.InitializeEvent(eventName);
			eventName = string.Format("Local\\Local_{0}-ComRegister", guid);
			eventWaitHandle2 = this.InitializeEvent(eventName);
			if (eventWaitHandle == null || eventWaitHandle2 == null)
			{
				throw new ComInterfaceInitializeException("Can't Create an unique wait events for worker process!");
			}
			string text = string.Format("-EventID {0} -PID {1}", guid, this.currentProcessHandle.DangerousGetHandle());
			if (this.WorkerConfiguration.ExtraCommandLineParameters != null)
			{
				text = text + " " + this.WorkerConfiguration.ExtraCommandLineParameters;
			}
			if (this.ProcessLaunchMutex != null)
			{
				try
				{
					this.ProcessLaunchMutex.WaitOne();
				}
				catch (AbandonedMutexException ex)
				{
					this.TraceError(this, "AbandonedMutexException caught: {0}", new object[]
					{
						ex
					});
				}
			}
			try
			{
				if (this.WorkerConfiguration.RunAsLocalService)
				{
					this.workerProcess = SafeProcessHandle.CreateProcessAsUser(this.workerProcessToken, workerProcessPath, string.Format("{0} {1}", workerProcessPath, text));
				}
				else
				{
					this.workerProcess = SafeProcessHandle.CreateProcess(workerProcessPath, string.Format("{0} {1}", workerProcessPath, text));
				}
				this.workerProcessId = this.workerProcess.GetProcessId();
				this.jobObject = this.jobObjectManger.CreateJobObject(this.workerProcess, this.WorkerConfiguration.MayRunUnderAnotherJobObject);
				eventWaitHandle.Set();
				IntPtr[] array = new IntPtr[]
				{
					eventWaitHandle2.SafeWaitHandle.DangerousGetHandle(),
					this.workerProcess.DangerousGetHandle()
				};
				switch (NativeMethods.WaitForMultipleObjects((uint)array.Length, array, false, (uint)ComProcessAgent<IComInterface>.MaxWaitingTimeForWorkerProcessRegister.TotalMilliseconds))
				{
				case 0U:
					try
					{
						Type workerType = this.WorkerConfiguration.WorkerType;
						this.worker = (IComInterface)((object)Activator.CreateInstance(workerType));
					}
					catch (COMException inner)
					{
						throw new ComInterfaceInitializeException("Active worker object failed", inner);
					}
					break;
				case 1U:
					throw new ComInterfaceInitializeException("Work process exit before class id registered!");
				default:
					throw new ComInterfaceInitializeException("Wait too long to register the class id");
				}
			}
			finally
			{
				if (this.ProcessLaunchMutex != null)
				{
					this.ProcessLaunchMutex.ReleaseMutex();
				}
				if (eventWaitHandle != null)
				{
					eventWaitHandle.Close();
				}
				if (eventWaitHandle2 != null)
				{
					eventWaitHandle2.Close();
				}
			}
			if (createWorker != null)
			{
				createWorker(this, requestParameters);
			}
			DateTime localTime = ExDateTime.Now.LocalTime;
			this.transactionCount = 0;
			this.ResetProcessExpirationTime(new DateTime?(localTime));
			this.ResetIdleExpirationTime(new DateTime?(localTime));
		}

		internal bool ExecuteRequest(ComProcessManager<IComInterface>.OnExecuteRequest requestDelegate, object requestParameters)
		{
			ComProcessAgent<IComInterface>.TransactionState transactionState = new ComProcessAgent<IComInterface>.TransactionState(this.transactionCount);
			Timer timer = new Timer(new TimerCallback(this.TimerProc), transactionState, this.WorkerConfiguration.TransactionTimeout, -1);
			bool flag = false;
			Exception ex = null;
			try
			{
				flag = requestDelegate(this, requestParameters);
			}
			catch (Exception ex2)
			{
				this.TraceError(this, "Request {0} terminated with an exception {1}", new object[]
				{
					transactionState.TransactionNumber,
					ex2
				});
				ex = ex2;
				throw;
			}
			finally
			{
				lock (transactionState)
				{
					this.transactionCount++;
				}
				if (transactionState.TimedOut)
				{
					string text = string.Format("Request {0} has timed out", transactionState.TransactionNumber);
					this.TraceError(this, text, new object[0]);
					throw new ComProcessTimeoutException(text, ex);
				}
				if (this.isWorkerBeyondMemoryLimit)
				{
					string text2 = string.Format("Worker process (PID = {0}) reaches the memory limit.", this.workerProcessId);
					this.TraceError(this, text2, new object[0]);
					throw new ComProcessBeyondMemoryLimitException(text2, ex);
				}
				if (ex != null)
				{
					this.TerminateWorkerProcess(true);
				}
				else if (!flag)
				{
					this.TerminateWorkerProcess(false);
				}
				this.ResetIdleExpirationTime(new DateTime?(ExDateTime.Now.LocalTime));
				timer.Dispose();
			}
			return flag;
		}

		private void TimerProc(object state)
		{
			ComProcessAgent<IComInterface>.TransactionState transactionState = (ComProcessAgent<IComInterface>.TransactionState)state;
			lock (transactionState)
			{
				if (this.transactionCount > transactionState.TransactionNumber)
				{
					return;
				}
				transactionState.SetTimedOut();
			}
			this.TerminateWorkerProcess(true);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.workerProcess != null)
				{
					this.workerProcess.Dispose();
					this.workerProcess = null;
				}
				if (this.jobObject != null)
				{
					this.jobObject.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ComProcessAgent<IComInterface>>(this);
		}

		private void LaunchWorkProcessFailed(Exception e)
		{
			this.TraceError(this, "Launch work process failed! error is {0}", new object[]
			{
				e
			});
			throw new ComInterfaceInitializeException("Launch work process failed", e);
		}

		private EventWaitHandle InitializeEvent(string eventName)
		{
			bool flag = false;
			EventWaitHandle eventWaitHandle = null;
			EventWaitHandleSecurity eventWaitHandleSecurity = null;
			if (this.WorkerConfiguration.RunAsLocalService)
			{
				eventWaitHandleSecurity = new EventWaitHandleSecurity();
				EventWaitHandleAccessRule rule = new EventWaitHandleAccessRule("NT AUTHORITY\\LocalService", EventWaitHandleRights.FullControl, AccessControlType.Allow);
				eventWaitHandleSecurity.AddAccessRule(rule);
			}
			try
			{
				eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, eventName, ref flag, eventWaitHandleSecurity);
			}
			catch (UnauthorizedAccessException)
			{
				return null;
			}
			catch (WaitHandleCannotBeOpenedException)
			{
				return null;
			}
			if (!flag && eventWaitHandle != null)
			{
				eventWaitHandle.Close();
				eventWaitHandle = null;
			}
			return eventWaitHandle;
		}

		public void TerminateWorkerProcess(bool forceTermination)
		{
			if (base.IsDisposed)
			{
				return;
			}
			try
			{
				if (!forceTermination && this.worker != null)
				{
					try
					{
						Marshal.ReleaseComObject(this.worker);
					}
					catch (SystemException)
					{
					}
				}
				this.KillProcess(this.workerProcess);
				if (this.WorkerProcessTerminatedEvent != null)
				{
					this.WorkerProcessTerminatedEvent(this, null);
				}
			}
			finally
			{
				this.worker = default(IComInterface);
			}
		}

		private void KillProcess(SafeProcessHandle process)
		{
			if (process != null)
			{
				try
				{
					process.TerminateProcess(0U);
				}
				catch (Win32Exception ex)
				{
					this.TraceError(this, "Kill process failed reason is {0}", new object[]
					{
						ex.ToString()
					});
				}
				catch (InvalidOperationException ex2)
				{
					this.TraceError(this, "Target process is invalid. {0}", new object[]
					{
						ex2.ToString()
					});
				}
			}
		}

		internal bool IsExpiredLifetimeOrIdleTime(DateTime now)
		{
			return (this.processExpirationTime != null && DateTime.Compare(this.processExpirationTime.Value, now) < 0) || (this.idleExpirationTime != null && DateTime.Compare(this.idleExpirationTime.Value, now) < 0);
		}

		internal bool IsExpiredLifetimeOrTransactionCount(DateTime now)
		{
			return this.transactionCount >= this.WorkerConfiguration.MaxTransactionsPerProcess || (this.processExpirationTime != null && DateTime.Compare(this.processExpirationTime.Value, now) < 0);
		}

		private void ResetProcessExpirationTime(DateTime? fromTime)
		{
			if (fromTime == null)
			{
				this.processExpirationTime = null;
				return;
			}
			if (this.WorkerConfiguration.WorkerLifetimeLimit != 0)
			{
				this.processExpirationTime = new DateTime?(fromTime.Value.AddMilliseconds((double)this.WorkerConfiguration.WorkerLifetimeLimit));
			}
		}

		private void ResetIdleExpirationTime(DateTime? fromTime)
		{
			if (fromTime == null)
			{
				this.idleExpirationTime = null;
				return;
			}
			if (this.WorkerConfiguration.WorkerIdleTimeout != 0)
			{
				this.idleExpirationTime = new DateTime?(fromTime.Value.AddMilliseconds((double)this.WorkerConfiguration.WorkerIdleTimeout));
			}
		}

		private ComWorkerConfiguration WorkerConfiguration
		{
			get
			{
				return this.configuration;
			}
		}

		private Mutex ProcessLaunchMutex
		{
			get
			{
				return this.WorkerConfiguration.ProcessLaunchMutex;
			}
		}

		private void TraceError(object target, string formatString, params object[] args)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceError((long)((target != null) ? target.GetHashCode() : 0), formatString, args);
			}
		}

		private static readonly TimeSpan MaxWaitingTimeForWorkerProcessRegister = TimeSpan.FromSeconds(10.0);

		private IDisposable jobObject;

		private JobObjectManager jobObjectManger;

		private SafeProcessHandle currentProcessHandle;

		private SafeUserTokenHandle workerProcessToken;

		private ComWorkerConfiguration configuration;

		private SafeProcessHandle workerProcess;

		private int workerProcessId;

		private IComInterface worker;

		private int transactionCount;

		private DateTime? processExpirationTime;

		private DateTime? idleExpirationTime;

		private Trace tracer;

		private bool isWorkerBeyondMemoryLimit;

		internal class TransactionState
		{
			internal TransactionState(int transactionNumber)
			{
				this.transactionNumber = transactionNumber;
				this.isTimedOut = false;
			}

			internal bool TimedOut
			{
				get
				{
					return this.isTimedOut;
				}
			}

			internal int TransactionNumber
			{
				get
				{
					return this.transactionNumber;
				}
			}

			internal void SetTimedOut()
			{
				this.isTimedOut = true;
			}

			private int transactionNumber;

			private bool isTimedOut;
		}
	}
}
