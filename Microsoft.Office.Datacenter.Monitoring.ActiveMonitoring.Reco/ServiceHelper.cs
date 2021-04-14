using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class ServiceHelper : DisposeTrackableBase
	{
		public ServiceHelper(string serviceName, CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			this.serviceController = new ServiceController(serviceName);
		}

		public ProcessNativeMethods.SERVICE_STATUS_PROCESS GetStatusInfo()
		{
			ProcessNativeMethods.SERVICE_STATUS_PROCESS result;
			using (SafeHandle serviceHandle = this.serviceController.ServiceHandle)
			{
				ProcessNativeMethods.SERVICE_STATUS_PROCESS service_STATUS_PROCESS = null;
				int num = Marshal.SizeOf(typeof(ProcessNativeMethods.SERVICE_STATUS_PROCESS));
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				try
				{
					if (!ProcessNativeMethods.QueryServiceStatusEx(serviceHandle, 0, intPtr, num, out num))
					{
						Marshal.GetLastWin32Error();
						throw new Win32Exception();
					}
					ProcessNativeMethods.SERVICE_STATUS_PROCESS service_STATUS_PROCESS2 = (ProcessNativeMethods.SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(intPtr, typeof(ProcessNativeMethods.SERVICE_STATUS_PROCESS));
					service_STATUS_PROCESS = new ProcessNativeMethods.SERVICE_STATUS_PROCESS();
					service_STATUS_PROCESS.ServiceType = service_STATUS_PROCESS2.ServiceType;
					service_STATUS_PROCESS.CurrentState = service_STATUS_PROCESS2.CurrentState;
					service_STATUS_PROCESS.ControlsAccepted = service_STATUS_PROCESS2.ControlsAccepted;
					service_STATUS_PROCESS.Win32ExitCode = service_STATUS_PROCESS2.Win32ExitCode;
					service_STATUS_PROCESS.ServiceSpecificExitCode = service_STATUS_PROCESS2.ServiceSpecificExitCode;
					service_STATUS_PROCESS.CheckPoint = service_STATUS_PROCESS2.CheckPoint;
					service_STATUS_PROCESS.WaitHint = service_STATUS_PROCESS2.WaitHint;
					service_STATUS_PROCESS.ProcessID = service_STATUS_PROCESS2.ProcessID;
					service_STATUS_PROCESS.ServiceFlags = service_STATUS_PROCESS2.ServiceFlags;
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				result = service_STATUS_PROCESS;
			}
			return result;
		}

		public void ControlService(int controlCode)
		{
			using (SafeHandle serviceHandle = this.serviceController.ServiceHandle)
			{
				ProcessNativeMethods.SERVICE_STATUS service_STATUS = default(ProcessNativeMethods.SERVICE_STATUS);
				if (!ProcessNativeMethods.ControlService(serviceHandle, controlCode, ref service_STATUS))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					string message = string.Format("ControlService({0}, {1}) failed with error: {2}", this.serviceController.ServiceName, controlCode, lastWin32Error);
					throw new Win32Exception(lastWin32Error, message);
				}
			}
		}

		public Process GetProcess()
		{
			Process result = null;
			ProcessNativeMethods.SERVICE_STATUS_PROCESS statusInfo = this.GetStatusInfo();
			if (statusInfo != null && statusInfo.CurrentState != 1 && statusInfo.ProcessID != 0)
			{
				int processID = statusInfo.ProcessID;
				if (processID != -1)
				{
					result = ProcessHelper.GetProcessByIdBestEffort(processID);
				}
			}
			return result;
		}

		public void Sleep(TimeSpan duration)
		{
			RecoveryActionHelper.Sleep(duration, this.cancellationToken);
		}

		public bool Start()
		{
			bool result = false;
			this.serviceController.Refresh();
			if (this.serviceController.Status == ServiceControllerStatus.Stopped)
			{
				this.serviceController.Start();
				result = true;
			}
			this.serviceController.Refresh();
			return result;
		}

		public bool WaitUntilProcessGoesAway(int lastPid, TimeSpan stopTimeout)
		{
			bool result = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (;;)
			{
				this.serviceController.Refresh();
				Process process = this.GetProcess();
				if (process == null || lastPid != process.Id)
				{
					break;
				}
				this.Sleep(this.defaultYieldDuration);
				this.cancellationToken.ThrowIfCancellationRequested();
				if (!(stopTimeout == this.infiniteTimespan) && !(stopwatch.Elapsed < stopTimeout))
				{
					return result;
				}
			}
			result = true;
			return result;
		}

		public void WaitForStatus(ServiceControllerStatus status, TimeSpan timeout)
		{
			bool flag = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (;;)
			{
				this.serviceController.Refresh();
				if (this.serviceController.Status == status)
				{
					break;
				}
				this.Sleep(this.defaultYieldDuration);
				this.cancellationToken.ThrowIfCancellationRequested();
				if (!(timeout == this.infiniteTimespan) && !(stopwatch.Elapsed < timeout))
				{
					goto IL_5E;
				}
			}
			flag = true;
			IL_5E:
			if (!flag)
			{
				throw new System.ServiceProcess.TimeoutException();
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.serviceController != null)
			{
				this.serviceController.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ServiceHelper>(this);
		}

		internal const int ScStatusProcssInfo = 0;

		private readonly TimeSpan infiniteTimespan = TimeSpan.FromMilliseconds(-1.0);

		private readonly TimeSpan defaultYieldDuration = TimeSpan.FromMilliseconds(300.0);

		private CancellationToken cancellationToken;

		private ServiceController serviceController;
	}
}
