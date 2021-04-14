using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ManageSetupService : ManageServiceBase
	{
		[Parameter(Mandatory = true)]
		public string ServiceName
		{
			get
			{
				return (string)base.Fields["ServiceName"];
			}
			set
			{
				base.Fields["ServiceName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IgnoreTimeout
		{
			get
			{
				return (bool)base.Fields["IgnoreTimeout"];
			}
			set
			{
				base.Fields["IgnoreTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] ServiceParameters
		{
			get
			{
				return (string[])base.Fields["ServiceParameters"];
			}
			set
			{
				base.Fields["ServiceParameters"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FailIfServiceNotInstalled
		{
			get
			{
				return (bool)base.Fields["FailIfServiceNotInstalled"];
			}
			set
			{
				base.Fields["FailIfServiceNotInstalled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> MaximumWaitTime
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)base.Fields["MaximumWaitTime"];
			}
			set
			{
				base.Fields["MaximumWaitTime"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> MaxWaitTimeForRunningState
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)base.Fields["MaxWaitTimeForRunningState"];
			}
			set
			{
				base.Fields["MaxWaitTimeForRunningState"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!base.Fields.IsModified("IgnoreTimeout"))
			{
				this.IgnoreTimeout = false;
			}
			if (!base.Fields.IsModified("MaximumWaitTime"))
			{
				this.MaximumWaitTime = EnhancedTimeSpan.FromMinutes(15.0);
			}
			if (!base.Fields.IsModified("MaxWaitTimeForRunningState"))
			{
				this.MaxWaitTimeForRunningState = EnhancedTimeSpan.FromSeconds(20.0);
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		private Exception StartServiceWorker(ServiceController serviceController, string[] serviceParameters)
		{
			Exception result;
			try
			{
				if (serviceParameters != null && serviceParameters.Length > 0)
				{
					serviceController.Start(serviceParameters);
				}
				else
				{
					serviceController.Start();
				}
				result = null;
			}
			catch (InvalidOperationException ex)
			{
				result = ex;
			}
			catch (Win32Exception ex2)
			{
				result = ex2;
			}
			return result;
		}

		internal void StartService(string serviceName, bool ignoreServiceStartTimeout, bool failIfServiceNotInstalled, Unlimited<EnhancedTimeSpan> maximumWaitTime, string[] serviceParameters)
		{
			TaskLogger.LogEnter(new object[]
			{
				serviceName
			});
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				this.StartService(serviceController, ignoreServiceStartTimeout, failIfServiceNotInstalled, maximumWaitTime, serviceParameters);
			}
		}

		internal void StartService(ServiceController serviceController, bool ignoreServiceStartTimeout, bool failIfServiceNotInstalled, Unlimited<EnhancedTimeSpan> maximumWaitTime, string[] serviceParameters)
		{
			if (!ServiceControllerUtils.IsInstalled(serviceController))
			{
				string text;
				try
				{
					text = serviceController.ServiceName;
				}
				catch (InvalidOperationException)
				{
					text = "";
				}
				TaskLogger.Trace("Service {0} is not installed. Cannot start the service", new object[]
				{
					text
				});
				if (failIfServiceNotInstalled)
				{
					base.WriteError(new ServiceNotInstalledException(text), ErrorCategory.InvalidOperation, null);
					return;
				}
				this.WriteWarning(Strings.ServiceNotInstalled(text));
				return;
			}
			else
			{
				string text = serviceController.ServiceName;
				bool flag = false;
				ServiceControllerStatus status = serviceController.Status;
				if (status == ServiceControllerStatus.Running)
				{
					TaskLogger.Trace("Service {0} is already running. ", new object[]
					{
						text
					});
					TaskLogger.LogExit();
					return;
				}
				foreach (ServiceController serviceController2 in serviceController.ServicesDependedOn)
				{
					this.StartService(serviceController2, ignoreServiceStartTimeout, failIfServiceNotInstalled, maximumWaitTime, null);
				}
				if (status == ServiceControllerStatus.StopPending)
				{
					TaskLogger.Trace("Service {0} is stopping. Waiting for the service to stop", new object[]
					{
						text
					});
					if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Stopped, maximumWaitTime, true, false))
					{
						TaskLogger.Trace("Service {0} is stopped", new object[]
						{
							text
						});
					}
				}
				else if (status == ServiceControllerStatus.PausePending)
				{
					TaskLogger.Trace("Service {0} is pausing. Waiting for the service to pause", new object[]
					{
						text
					});
					if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Paused, maximumWaitTime, true, false))
					{
						TaskLogger.Trace("Service {0} is paused", new object[]
						{
							text
						});
					}
				}
				else if (status == ServiceControllerStatus.ContinuePending || status == ServiceControllerStatus.StartPending)
				{
					TaskLogger.Trace("Service {0} is already starting", new object[]
					{
						text
					});
					flag = true;
				}
				if (!flag)
				{
					TaskLogger.Trace("Attempting to start service {0}", new object[]
					{
						text
					});
					ManageSetupService.StartServiceWorkerDelegate startServiceWorkerDelegate = new ManageSetupService.StartServiceWorkerDelegate(this.StartServiceWorker);
					IAsyncResult asyncResult = startServiceWorkerDelegate.BeginInvoke(serviceController, serviceParameters, null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(this.MaxWaitTimeForRunningState.Value, false))
					{
						this.SendWatsonReportForHungService(text);
						asyncResult.AsyncWaitHandle.WaitOne();
					}
					Exception ex = startServiceWorkerDelegate.EndInvoke(asyncResult);
					if (ex is InvalidOperationException)
					{
						InvalidOperationException ex2 = ex as InvalidOperationException;
						Win32Exception ex3 = ex2.InnerException as Win32Exception;
						if (ex3 != null && ex3.NativeErrorCode == 1058)
						{
							if (failIfServiceNotInstalled)
							{
								base.WriteError(new ServiceDisabledException(text, ex2), ErrorCategory.InvalidOperation, null);
								return;
							}
							this.WriteWarning(Strings.ServiceDisabled(text));
							return;
						}
						else if (ex3 != null && ex3.NativeErrorCode == 1053)
						{
							if (!ignoreServiceStartTimeout)
							{
								base.WriteError(new ServiceDidNotReachStatusException(text, ServiceControllerStatus.Running.ToString(), ex2), ErrorCategory.InvalidOperation, null);
							}
							else
							{
								this.WriteWarning(Strings.ServiceDidNotReachStatus(text, ServiceControllerStatus.Running.ToString()));
							}
						}
						else
						{
							base.WriteError(new ServiceStartFailureException(text, ex2.Message, ex2), ErrorCategory.InvalidOperation, null);
						}
					}
				}
				if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Running, maximumWaitTime, ignoreServiceStartTimeout, true))
				{
					TaskLogger.Trace("Service {0} is running", new object[]
					{
						text
					});
				}
				TaskLogger.LogExit();
				return;
			}
		}

		internal void StopService(string serviceName, bool ignoreServiceStopTimeout, bool failIfServiceNotInstalled, Unlimited<EnhancedTimeSpan> maximumWaitTime)
		{
			TaskLogger.LogEnter(new object[]
			{
				serviceName
			});
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				this.StopService(serviceController, ignoreServiceStopTimeout, failIfServiceNotInstalled, maximumWaitTime);
			}
		}

		internal void StopService(ServiceController serviceController, bool ignoreServiceStopTimeout, bool failIfServiceNotInstalled, Unlimited<EnhancedTimeSpan> maximumWaitTime)
		{
			if (!ServiceControllerUtils.IsInstalled(serviceController))
			{
				string text;
				try
				{
					text = serviceController.ServiceName;
				}
				catch (InvalidOperationException)
				{
					text = "";
				}
				TaskLogger.Trace("Service {0} is not installed. Cannot stop the service", new object[]
				{
					text
				});
				if (failIfServiceNotInstalled)
				{
					base.WriteError(new ServiceNotInstalledException(text), ErrorCategory.InvalidOperation, null);
					return;
				}
				this.WriteWarning(Strings.ServiceNotInstalled(text));
				return;
			}
			else
			{
				string text = serviceController.ServiceName;
				bool flag = false;
				ServiceControllerStatus status = serviceController.Status;
				if (status == ServiceControllerStatus.Stopped)
				{
					TaskLogger.Trace("Service {0} is already stopped. ", new object[]
					{
						text
					});
					TaskLogger.LogExit();
					return;
				}
				foreach (ServiceController serviceController2 in serviceController.DependentServices)
				{
					this.StopService(serviceController2, ignoreServiceStopTimeout, failIfServiceNotInstalled, maximumWaitTime);
				}
				if (status == ServiceControllerStatus.StartPending || status == ServiceControllerStatus.ContinuePending)
				{
					TaskLogger.Trace("Service {0} is starting. Waiting for the service to start", new object[]
					{
						text
					});
					if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Running, maximumWaitTime, true, false))
					{
						TaskLogger.Trace("Service {0} is running", new object[]
						{
							text
						});
					}
				}
				else if (status == ServiceControllerStatus.PausePending)
				{
					TaskLogger.Trace("Service {0} is pausing. Waiting for the service to pause", new object[]
					{
						text
					});
					if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Paused, maximumWaitTime, true, false))
					{
						TaskLogger.Trace("Service {0} is paused", new object[]
						{
							text
						});
					}
				}
				else if (status == ServiceControllerStatus.StopPending)
				{
					TaskLogger.Trace("Service {0} is already stopping", new object[]
					{
						text
					});
					flag = true;
				}
				if (!flag)
				{
					TaskLogger.Trace("Attempting to stop service {0}", new object[]
					{
						text
					});
					try
					{
						serviceController.Stop();
					}
					catch (InvalidOperationException ex)
					{
						Win32Exception ex2 = ex.InnerException as Win32Exception;
						if (ex2 != null && ex2.NativeErrorCode == 1058)
						{
							this.WriteWarning(Strings.ServiceDisabled(text));
						}
						else if (ex2 != null && ex2.NativeErrorCode == 1053)
						{
							if (!ignoreServiceStopTimeout)
							{
								base.WriteError(new ServiceDidNotReachStatusException(text, ServiceControllerStatus.Stopped.ToString(), ex), ErrorCategory.InvalidOperation, null);
							}
							else
							{
								this.WriteWarning(Strings.ServiceDidNotReachStatus(text, ServiceControllerStatus.Stopped.ToString()));
							}
						}
						else
						{
							base.WriteError(new ServiceStopFailureException(text, ex.Message, ex), ErrorCategory.InvalidOperation, null);
						}
					}
				}
				if (this.WaitForServiceStatus(serviceController, ServiceControllerStatus.Stopped, maximumWaitTime, ignoreServiceStopTimeout, false))
				{
					TaskLogger.Trace("Service {0} is stopped", new object[]
					{
						text
					});
				}
				TaskLogger.LogExit();
				return;
			}
		}

		private bool WaitForServiceStatus(ServiceController serviceController, ServiceControllerStatus status, Unlimited<EnhancedTimeSpan> maximumWaitTime, bool ignoreFailures, bool sendWatsonReportForHungService)
		{
			ExDateTime now = ExDateTime.Now;
			string serviceName = serviceController.ServiceName;
			SafeHandle safeHandle = null;
			try
			{
				safeHandle = serviceController.ServiceHandle;
			}
			catch (Exception e)
			{
				base.WriteVerbose(Strings.ExceptionCannotGetServiceHandle(serviceName, e));
			}
			ExDateTime exDateTime = now;
			SERVICE_STATUS service_STATUS = default(SERVICE_STATUS);
			service_STATUS.dwCheckPoint = 0U;
			service_STATUS.dwWaitHint = 25000U;
			bool result;
			try
			{
				for (;;)
				{
					ExDateTime exDateTime2 = exDateTime;
					SERVICE_STATUS service_STATUS2 = service_STATUS;
					exDateTime = ExDateTime.Now;
					if (safeHandle != null)
					{
						if (NativeMethods.QueryServiceStatus(safeHandle, ref service_STATUS) == 0)
						{
							base.WriteError(new Win32Exception(Marshal.GetLastWin32Error()), ErrorCategory.InvalidOperation, null);
						}
						if ((ulong)service_STATUS.dwCurrentState == (ulong)((long)status))
						{
							break;
						}
						if (service_STATUS.dwCheckPoint == service_STATUS2.dwCheckPoint)
						{
							this.WriteWarning(Strings.CheckPointNotProgressed(service_STATUS2.dwCheckPoint, service_STATUS.dwCheckPoint));
							base.WriteVerbose(Strings.PreviousQueryTime(exDateTime2.ToString()));
							base.WriteVerbose(Strings.CurrentQueryTime(exDateTime.ToString()));
							if (exDateTime2.AddMilliseconds(service_STATUS2.dwWaitHint + 600000U) < exDateTime)
							{
								goto Block_9;
							}
						}
						else
						{
							base.WriteVerbose(Strings.CheckPointProgressed(service_STATUS2.dwCheckPoint, service_STATUS.dwCheckPoint));
						}
					}
					uint num = (service_STATUS.dwWaitHint == 0U) ? 25000U : service_STATUS.dwWaitHint;
					try
					{
						base.WriteVerbose(Strings.WaitForServiceStatusChange(num, serviceName, status.ToString()));
						serviceController.WaitForStatus(status, TimeSpan.FromMilliseconds(num));
						base.WriteVerbose(Strings.ServiceReachedStatusDuringWait(serviceName, status.ToString()));
						return true;
					}
					catch (System.ServiceProcess.TimeoutException)
					{
						base.WriteVerbose(Strings.ServiceDidNotReachStatusDuringWait(serviceName, status.ToString(), num));
						if (!maximumWaitTime.IsUnlimited && now.AddMilliseconds(maximumWaitTime.Value.TotalMilliseconds) < ExDateTime.Now)
						{
							if (!ignoreFailures)
							{
								if (sendWatsonReportForHungService)
								{
									this.SendWatsonReportForHungService(serviceName);
								}
								base.WriteError(new ServiceDidNotReachStatusException(serviceName, status.ToString()), ErrorCategory.InvalidOperation, null);
							}
							else
							{
								this.WriteWarning(Strings.ServiceDidNotReachStatus(serviceName, status.ToString()));
							}
							return false;
						}
					}
				}
				base.WriteVerbose(Strings.ServiceReachedStatus(serviceName, status.ToString()));
				return true;
				Block_9:
				if (!ignoreFailures)
				{
					if (sendWatsonReportForHungService)
					{
						this.SendWatsonReportForHungService(serviceName);
					}
					base.WriteError(new ServiceDidNotReachStatusException(serviceName, status.ToString()), ErrorCategory.InvalidOperation, null);
				}
				else
				{
					this.WriteWarning(Strings.ServiceDidNotReachStatus(serviceName, status.ToString()));
				}
				result = false;
			}
			finally
			{
				if (safeHandle != null)
				{
					safeHandle.Close();
				}
			}
			return result;
		}

		private void SendWatsonReportForHungService(string serviceName)
		{
			Process process = null;
			try
			{
				process = this.GetProcess(serviceName);
			}
			catch (Win32Exception ex)
			{
				TaskLogger.Log(Strings.UnableToGetProcessForService(serviceName, ex.ToString()));
			}
			if (process == null)
			{
				try
				{
					process = this.GetFarFetchedProcess(serviceName);
				}
				catch (Win32Exception ex2)
				{
					TaskLogger.Log(Strings.UnableToGetProcessForService(serviceName, ex2.ToString()));
				}
			}
			if (process != null)
			{
				TaskLogger.Trace("Service {0} has process id {1}", new object[]
				{
					serviceName,
					process.Id
				});
				try
				{
					ExWatson.SendHangWatsonReport(new System.ServiceProcess.TimeoutException(serviceName), process);
					TaskLogger.Log(Strings.GeneratedWatsonReportForHungService(serviceName));
				}
				catch (Win32Exception ex3)
				{
					TaskLogger.Log(Strings.UnableToGeneratedWatsonReportForHungService(serviceName, ex3.ToString()));
				}
				catch (InvalidOperationException ex4)
				{
					TaskLogger.Log(Strings.UnableToGeneratedWatsonReportForHungService(serviceName, ex4.ToString()));
				}
			}
		}

		private int GetProcessId(string serviceName)
		{
			SERVICE_STATUS_EX status = default(SERVICE_STATUS_EX);
			base.DoNativeServiceTask(serviceName, ServiceAccessFlags.AllAccess, delegate(IntPtr serviceHandle)
			{
				uint num = 0U;
				if (NativeMethods.QueryServiceStatusEx(serviceHandle, ServiceQueryStatus.ProcessInfo, ref status, (uint)Marshal.SizeOf(status), out num) == 0U)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			});
			return (int)status.dwProcessId;
		}

		private Process GetProcess(string serviceName)
		{
			int num = 0;
			try
			{
				num = this.GetProcessId(serviceName);
			}
			catch (Win32Exception ex)
			{
				TaskLogger.Log(Strings.UnableToGetProcessIdOfService(serviceName, ex.ToString()));
				return null;
			}
			if (num == 0)
			{
				TaskLogger.Log(Strings.ServiceHasNoProcessId(serviceName));
				return null;
			}
			Process result;
			try
			{
				result = Process.GetProcessById(num);
			}
			catch (ArgumentException ex2)
			{
				TaskLogger.Log(Strings.UnableToGetProcessForService(serviceName, ex2.ToString()));
				result = null;
			}
			return result;
		}

		private Process GetFarFetchedProcess(string serviceName)
		{
			QUERY_SERVICE_CONFIG serviceConfig = new QUERY_SERVICE_CONFIG();
			base.DoNativeServiceTask(serviceName, ServiceAccessFlags.AllAccess, delegate(IntPtr serviceHandle)
			{
				uint num = 1024U;
				uint num2 = 0U;
				IntPtr intPtr = Marshal.AllocHGlobal((int)num);
				try
				{
					if (!NativeMethods.QueryServiceConfig(serviceHandle, intPtr, num, out num2))
					{
						if (Marshal.GetLastWin32Error() != 122)
						{
							throw new Win32Exception(Marshal.GetLastWin32Error());
						}
						Marshal.FreeHGlobal(intPtr);
						intPtr = Marshal.AllocHGlobal((int)num2);
						if (!NativeMethods.QueryServiceConfig(serviceHandle, intPtr, num2, out num2))
						{
							throw new Win32Exception(Marshal.GetLastWin32Error());
						}
					}
					Marshal.PtrToStructure(intPtr, serviceConfig);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			});
			if ((serviceConfig.dwServiceType & 16U) == 0U)
			{
				TaskLogger.Log(Strings.NoOwnProcessService(serviceName));
				return null;
			}
			if (string.IsNullOrEmpty(serviceConfig.binaryPathName))
			{
				TaskLogger.Log(Strings.NoExecutableForService(serviceName));
				return null;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ManageSetupService.GetPathWithoutQuotes(ManageSetupService.GetFileNameWithoutFolderNameAndParam(serviceConfig.binaryPathName)));
			Process[] processesByName = Process.GetProcessesByName(fileNameWithoutExtension);
			foreach (Process process in processesByName)
			{
				string processId = "-";
				string startTime = "-";
				string endTime = "-";
				try
				{
					processId = process.Id.ToString();
					if (process.HasExited)
					{
						endTime = process.ExitTime.ToString();
					}
					else
					{
						startTime = process.StartTime.ToString();
					}
				}
				catch (InvalidOperationException)
				{
				}
				TaskLogger.Log(Strings.FoundProcessesForService(fileNameWithoutExtension, serviceName, processId, startTime, endTime));
			}
			if (processesByName.Length != 1)
			{
				TaskLogger.Log(Strings.BadNumberOfProcessesForService(serviceName, processesByName.Length.ToString()));
				return null;
			}
			return processesByName[0];
		}

		internal static string GetPathWithoutQuotes(string path)
		{
			int num = (path[0] == '"') ? 1 : 0;
			int num2 = (path[path.Length - 1] == '"') ? 1 : 0;
			return path.Substring(num, path.Length - (num + num2));
		}

		internal static string GetFileNameWithoutFolderNameAndParam(string path)
		{
			if (path == null || path == "")
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < path.Length; i++)
			{
				if (path[i] == '\\')
				{
					path = path.Substring(i + 1);
					i = -1;
				}
				else if (path[i] == '"')
				{
					flag = !flag;
				}
				else if (path[i] == ' ' && !flag)
				{
					path = path.Substring(0, i);
				}
			}
			int num = path.LastIndexOf('\\');
			string result;
			if (num == -1)
			{
				result = path;
			}
			else
			{
				result = path.Substring(num + 1);
			}
			return result;
		}

		private const uint AdditionalWaitTimeForStatusUpdate = 600000U;

		private delegate Exception StartServiceWorkerDelegate(ServiceController serviceController, string[] serviceParameters);
	}
}
