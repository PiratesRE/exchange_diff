using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal class JobObjectManager : DisposeTrackableBase
	{
		internal CallbackMontiorEvent CallbackMonitorEvent
		{
			get
			{
				return this.callbackMonitorEvent;
			}
			set
			{
				this.callbackMonitorEvent = value;
			}
		}

		public JobObjectManager(int workerMemoryLimit) : this(1010U, workerMemoryLimit)
		{
		}

		public JobObjectManager(uint ioCompletionPortNumber, int workerMemoryLimit)
		{
			AppDomain.CurrentDomain.DomainUnload += this.OnAppDomainUnloadHandler;
			this.ioCompletionPortNumber = ioCompletionPortNumber;
			this.workerMemoryLimit = workerMemoryLimit;
			this.ioCompletionPort = NativeMethods.CreateIoCompletionPort(new SafeFileHandle(new IntPtr(-1), true), IoCompletionPort.InvalidHandle, new UIntPtr(this.ioCompletionPortNumber), 0U);
			if (this.ioCompletionPort.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				this.TraceError(this, "Failed to create IO completion port. Error code is {0}.", new object[]
				{
					lastWin32Error
				});
				throw new Win32Exception(lastWin32Error, "Failed to create IO completion port. Error code is {0}.");
			}
			this.jobObjectCompletport = new NativeMethods.JobObjectAssociateCompletionPort(new IntPtr((long)((ulong)this.ioCompletionPortNumber)), this.ioCompletionPort.DangerousGetHandle());
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.ioCompletionPort != null)
				{
					this.StopProcessMonitor();
					this.ioCompletionPort.Dispose();
					this.ioCompletionPort = null;
				}
				AppDomain.CurrentDomain.DomainUnload -= this.OnAppDomainUnloadHandler;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JobObjectManager>(this);
		}

		public IDisposable CreateJobObject(SafeProcessHandle process, bool mayRunUnderAnotherJobObject)
		{
			JobObjectManager.JobObject jobObject = new JobObjectManager.JobObject(IntPtr.Zero, null, this.workerMemoryLimit, this.jobObjectCompletport);
			bool flag = false;
			try
			{
				jobObject.Tracer = this.tracer;
				if (jobObject.Add(process, mayRunUnderAnotherJobObject))
				{
					lock (this.myLock)
					{
						this.StartProcessMonitor();
					}
					flag = true;
				}
			}
			finally
			{
				if (!flag)
				{
					jobObject.Dispose();
					jobObject = null;
				}
			}
			return jobObject;
		}

		private void StopProcessMonitor()
		{
			if (this.monitorThread != null)
			{
				if (this.ioCompletionPort != null)
				{
					this.ioCompletionPort.PostQueuedCompletionStatus(65534U, this.ioCompletionPortNumber);
				}
				this.monitorThread.Join();
				this.monitorThread = null;
			}
		}

		private void StartProcessMonitor()
		{
			if (this.monitorThread != null)
			{
				return;
			}
			this.monitorThread = new Thread(new ThreadStart(this.MemoryMonitorProc));
			this.monitorThread.Name = "Job Memory Monitor";
			this.monitorThread.IsBackground = true;
			this.monitorThread.Start();
		}

		private void OnAppDomainUnloadHandler(object sender, EventArgs e)
		{
			if (!base.IsDisposed)
			{
				this.StopProcessMonitor();
			}
		}

		private void MemoryMonitorProc()
		{
			if (this.callbackMonitorEvent != null)
			{
				try
				{
					this.callbackMonitorEvent(MonitorEvent.MonitorStart, new object[0]);
				}
				catch (Exception ex)
				{
					this.TraceError(this, "Job object monitor thread start callback function if job object failed by {0}", new object[]
					{
						ex
					});
				}
			}
			for (;;)
			{
				uint num = 0U;
				UIntPtr uintPtr = new UIntPtr(0U);
				int num2 = 0;
				try
				{
					this.ioCompletionPort.GetQueuedCompletionStatus(out num, out uintPtr, out num2, uint.MaxValue);
				}
				catch (Win32Exception ex2)
				{
					this.TraceError(this, "Call GetQueuedCompletionStatus failed when monitor the child process memory. error is {0}", new object[]
					{
						ex2.ToString()
					});
					break;
				}
				if (uintPtr.ToUInt32() == this.ioCompletionPortNumber)
				{
					if (num == 9U)
					{
						if (this.callbackMonitorEvent == null)
						{
							continue;
						}
						try
						{
							this.callbackMonitorEvent(MonitorEvent.ReachMemoryLimitation, new object[]
							{
								num2
							});
							continue;
						}
						catch (Exception ex3)
						{
							this.TraceError(this, "Memory exceeded limitation callback function if job object failed by {0}", new object[]
							{
								ex3
							});
							continue;
						}
					}
					if (num == 65534U)
					{
						break;
					}
				}
			}
			if (this.callbackMonitorEvent != null)
			{
				try
				{
					this.callbackMonitorEvent(MonitorEvent.MonitorStop, new object[0]);
				}
				catch (Exception ex4)
				{
					this.TraceError(this, "Job object monitor thread end callback function if job object failed by {0}", new object[]
					{
						ex4
					});
				}
			}
		}

		private void TraceError(object target, string formatString, params object[] args)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceError((long)((target != null) ? target.GetHashCode() : 0), formatString, args);
			}
		}

		internal Trace Tracer
		{
			get
			{
				return this.tracer;
			}
			set
			{
				this.tracer = value;
			}
		}

		private const uint defaultIoCompletionPortNumber = 1010U;

		private IoCompletionPort ioCompletionPort;

		private NativeMethods.JobObjectAssociateCompletionPort jobObjectCompletport;

		private int workerMemoryLimit;

		private uint ioCompletionPortNumber;

		private Thread monitorThread;

		private Trace tracer;

		private object myLock = new object();

		private CallbackMontiorEvent callbackMonitorEvent;

		private sealed class JobObject : DisposeTrackableBase
		{
			public JobObject(IntPtr jobAttributes, string name, int maxMemoryPerProcess, NativeMethods.JobObjectAssociateCompletionPort jobObjectCompletionPort)
			{
				if (maxMemoryPerProcess <= 0)
				{
					throw new ArgumentException("Invalid maximum memory per process.", "maxMemoryPerProcess");
				}
				this.safeJobHandle = NativeMethods.CreateJobObject(jobAttributes, name);
				if (this.safeJobHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					this.TraceError(this, "Failed to create Job object. Error code is {0}.", new object[]
					{
						lastWin32Error
					});
					throw new Win32Exception(lastWin32Error, "Failed to create Job object. Error code is {0}.");
				}
				this.jobObjectCompletionPort = jobObjectCompletionPort;
				this.safeJobHandle.SetUIRestrictions(JobObjectUILimit.ReadClipboard | JobObjectUILimit.SystemParameters | JobObjectUILimit.WriteClipboard | JobObjectUILimit.Desktop | JobObjectUILimit.DisplaySettings | JobObjectUILimit.ExitWindows | JobObjectUILimit.GlobalAtoms);
				NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedLimits = default(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION);
				extendedLimits.BasicLimitInformation.LimitFlags = 8448U;
				extendedLimits.ProcessMemoryLimit = new UIntPtr((uint)maxMemoryPerProcess);
				this.safeJobHandle.SetExtendedLimits(extendedLimits);
			}

			internal Trace Tracer
			{
				get
				{
					return this.tracer;
				}
				set
				{
					this.tracer = value;
				}
			}

			protected override void InternalDispose(bool isDisposing)
			{
				if (isDisposing && this.safeJobHandle != null)
				{
					this.safeJobHandle.Dispose();
					this.safeJobHandle = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<JobObjectManager.JobObject>(this);
			}

			internal bool Add(SafeProcessHandle process, bool ignoreAccessDeniedFailure)
			{
				if (process == null || process.IsInvalid)
				{
					throw new ArgumentException("The process handle is either null or invalid.");
				}
				this.SetCompletionPort();
				if (this.safeJobHandle.Add(process))
				{
					return true;
				}
				int lastWin32Error = Marshal.GetLastWin32Error();
				if ((lastWin32Error == 5 || lastWin32Error == 50) && ignoreAccessDeniedFailure)
				{
					this.TraceError(this, "AccessDenied or NotSupported when assigning process to job object", new object[0]);
					return false;
				}
				this.TraceError(this, "Failed to assign the process to the job object {0}", new object[]
				{
					lastWin32Error
				});
				throw new Win32Exception(lastWin32Error, "Failed to assign the process to the job object.");
			}

			private unsafe void SetCompletionPort()
			{
				bool flag;
				fixed (IntPtr* ptr = (IntPtr*)(&this.jobObjectCompletionPort))
				{
					flag = NativeMethods.SetInformationJobObject(this.safeJobHandle, NativeMethods.JOBOBJECTINFOCLASS.JobObjectAssociateCompletionPortInformation, (void*)ptr, Marshal.SizeOf(typeof(NativeMethods.JobObjectAssociateCompletionPort)));
				}
				if (!flag)
				{
					this.TraceError(this, "Call SetInformationJobObject() failed when configurate the Completion port", new object[0]);
					throw new Win32Exception("Call SetInformationJobObject() failed when configurate the Completion port");
				}
			}

			private void TraceError(object target, string formatString, params object[] args)
			{
				if (this.tracer != null)
				{
					this.tracer.TraceError((long)((target != null) ? target.GetHashCode() : 0), formatString, args);
				}
			}

			private const int Win32ErrorAccessDenied = 5;

			private const int Win32ErrorNotSupported = 50;

			private SafeJobHandle safeJobHandle;

			private NativeMethods.JobObjectAssociateCompletionPort jobObjectCompletionPort;

			private Trace tracer;

			[Flags]
			private enum JobObjectExtendedLimit : uint
			{
				LimitProcessMemory = 256U,
				LimitJobMemory = 512U,
				LimitDieOnUnhandledException = 1024U,
				LimitBreakawayOK = 2048U,
				LimitSilentBreakawayOK = 4096U,
				LimitKillOnJobClose = 8192U
			}
		}

		private enum JobObjectMessage : uint
		{
			ProcessMemoryLimit = 9U,
			StopMonitoring = 65534U
		}
	}
}
