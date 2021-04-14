using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ServiceOperations
	{
		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ServiceOperationsTracer;
			}
		}

		internal static Process GetServiceProcess(string serviceName, out Exception exception)
		{
			exception = null;
			Process result = null;
			try
			{
				NativeMethods.SERVICE_STATUS_PROCESS serviceStatusInfo = ServiceOperations.GetServiceStatusInfo(serviceName);
				if (serviceStatusInfo != null)
				{
					if (serviceStatusInfo.currentState != 1 && serviceStatusInfo.processID != 0)
					{
						result = ServiceOperations.GetProcessByIdBestEffort(serviceStatusInfo.processID, out exception);
					}
					else
					{
						exception = new AmGetServiceProcessException(serviceName, serviceStatusInfo.currentState, serviceStatusInfo.processID);
					}
				}
			}
			catch (Win32Exception ex)
			{
				exception = ex;
			}
			if (exception != null)
			{
				ServiceOperations.Tracer.TraceError<string, Exception>(0L, "GetServiceProcess({0}) failed: {1}", serviceName, exception);
			}
			return result;
		}

		internal static Process GetProcessByIdBestEffort(int pid, out Exception exception)
		{
			exception = null;
			Process result = null;
			try
			{
				result = Process.GetProcessById(pid);
			}
			catch (Win32Exception ex)
			{
				exception = ex;
			}
			catch (ArgumentException ex2)
			{
				exception = ex2;
			}
			if (exception != null)
			{
				ServiceOperations.Tracer.TraceError<int, Exception>(0L, "GetProcessByIdBestEffort({0}) failed: {1}", pid, exception);
			}
			return result;
		}

		internal static NativeMethods.SERVICE_STATUS_PROCESS GetServiceStatusInfo(string serviceName)
		{
			NativeMethods.SERVICE_STATUS_PROCESS result;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				using (SafeHandle serviceHandle = serviceController.ServiceHandle)
				{
					NativeMethods.SERVICE_STATUS_PROCESS service_STATUS_PROCESS = null;
					int num = Marshal.SizeOf(typeof(NativeMethods.SERVICE_STATUS_PROCESS));
					IntPtr intPtr = Marshal.AllocHGlobal(num);
					try
					{
						if (!NativeMethods.QueryServiceStatusEx(serviceHandle, 0, intPtr, num, out num))
						{
							int lastWin32Error = Marshal.GetLastWin32Error();
							ServiceOperations.Tracer.TraceError<int>(0L, "QueryServiceStatusEx() failed with error: {0}", lastWin32Error);
							throw new Win32Exception();
						}
						NativeMethods.SERVICE_STATUS_PROCESS service_STATUS_PROCESS2 = (NativeMethods.SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.SERVICE_STATUS_PROCESS));
						service_STATUS_PROCESS = new NativeMethods.SERVICE_STATUS_PROCESS();
						service_STATUS_PROCESS.serviceType = service_STATUS_PROCESS2.serviceType;
						service_STATUS_PROCESS.currentState = service_STATUS_PROCESS2.currentState;
						service_STATUS_PROCESS.controlsAccepted = service_STATUS_PROCESS2.controlsAccepted;
						service_STATUS_PROCESS.win32ExitCode = service_STATUS_PROCESS2.win32ExitCode;
						service_STATUS_PROCESS.serviceSpecificExitCode = service_STATUS_PROCESS2.serviceSpecificExitCode;
						service_STATUS_PROCESS.checkPoint = service_STATUS_PROCESS2.checkPoint;
						service_STATUS_PROCESS.waitHint = service_STATUS_PROCESS2.waitHint;
						service_STATUS_PROCESS.processID = service_STATUS_PROCESS2.processID;
						service_STATUS_PROCESS.serviceFlags = service_STATUS_PROCESS2.serviceFlags;
					}
					finally
					{
						Marshal.FreeHGlobal(intPtr);
					}
					result = service_STATUS_PROCESS;
				}
			}
			return result;
		}

		public static Exception ControlService(string serviceName, int controlCode)
		{
			Exception result = null;
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				using (SafeHandle serviceHandle = serviceController.ServiceHandle)
				{
					NativeMethods.SERVICE_STATUS service_STATUS = default(NativeMethods.SERVICE_STATUS);
					if (!NativeMethods.ControlService(serviceHandle, controlCode, ref service_STATUS))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						string message = string.Format("ControlService({0}, {1}) failed with error: {2}", serviceName, controlCode, lastWin32Error);
						ServiceOperations.Tracer.TraceError(0L, message);
						result = new Win32Exception(lastWin32Error, message);
					}
				}
			}
			return result;
		}

		public static Exception KillService(string serviceName, string reason)
		{
			ServiceOperations.Tracer.TraceDebug<string>(0L, "KillService({0}) called", serviceName);
			Exception innerEx = null;
			Exception ex = null;
			try
			{
				ex = ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
				{
					using (Process serviceProcess = ServiceOperations.GetServiceProcess(serviceName, out innerEx))
					{
						if (serviceProcess != null)
						{
							serviceProcess.Kill();
							ServiceOperations.Tracer.TraceDebug<string>(0L, "Killed({0})", serviceName);
						}
					}
				});
				if (ex == null)
				{
					ex = innerEx;
				}
				if (ex != null)
				{
					ServiceOperations.Tracer.TraceError<string, Exception>(0L, "KillService({0}) fails: {1}", serviceName, ex);
				}
			}
			finally
			{
				ReplayCrimsonEvents.ServiceKilled.Log<string, string, string>(serviceName, reason, (ex != null) ? ex.Message : "<None>");
			}
			return ex;
		}

		internal static bool IsWindowsCoreProcess(Process process)
		{
			string b = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), process.MainModule.ModuleName);
			return string.Equals(process.MainModule.FileName, b, StringComparison.OrdinalIgnoreCase);
		}

		public static Exception KillProcess(string processName, bool isCoreProcess)
		{
			return ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
			{
				Process[] processesByName = Process.GetProcessesByName(processName);
				if (processesByName != null)
				{
					foreach (Process process in processesByName)
					{
						using (process)
						{
							if (!isCoreProcess || ServiceOperations.IsWindowsCoreProcess(process))
							{
								ServiceOperations.Tracer.TraceDebug<string>(0L, "Killing process: {0}", process.MainModule.FileName);
								process.Kill();
							}
							else
							{
								ServiceOperations.Tracer.TraceDebug<string>(0L, "Skipped killing {0} since it does not appear to be a windows core process", process.MainModule.FileName);
							}
						}
					}
				}
			});
		}

		internal static Exception RunOperation(EventHandler ev)
		{
			Exception result = null;
			try
			{
				ev(null, null);
			}
			catch (Win32Exception ex)
			{
				result = ex;
			}
			catch (ArgumentException ex2)
			{
				result = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				result = ex3;
			}
			catch (System.ServiceProcess.TimeoutException ex4)
			{
				result = ex4;
			}
			return result;
		}

		public static bool IsServiceRunningOnNode(string serviceName, string nodeName, out Exception ex)
		{
			ex = null;
			bool fRunning = false;
			ex = ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
			{
				ServiceController serviceController = new ServiceController(serviceName, nodeName);
				using (serviceController)
				{
					ServiceOperations.Tracer.TraceDebug<string, ServiceControllerStatus, string>(0L, "IsServiceRunningOnNode: {0} is {1} on {2}.", serviceName, serviceController.Status, nodeName);
					if (serviceController.Status == ServiceControllerStatus.Running)
					{
						fRunning = true;
					}
				}
			});
			if (ex != null)
			{
				ServiceOperations.Tracer.TraceError<string, string, Exception>(0L, "IsServiceRunningOnNode( {0}, {1} ): Caught exception {2}", serviceName, nodeName, ex);
			}
			return fRunning;
		}

		public const string MsExchangeReplServiceName = "msexchangerepl";

		public const string MsExchangeISServiceName = "MSExchangeIS";

		public const string ClusterServiceName = "Clussvc";
	}
}
