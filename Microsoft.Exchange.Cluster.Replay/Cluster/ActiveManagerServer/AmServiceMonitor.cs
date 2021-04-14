using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmServiceMonitor : ChangePoller
	{
		public AmServiceMonitor(string serviceName) : base(true)
		{
			this.ServiceName = serviceName;
			this.m_scm = new ServiceController(serviceName);
		}

		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmServiceMonitorTracer;
			}
		}

		protected int ProcessId
		{
			get
			{
				return this.m_serviceProcessId;
			}
		}

		protected DateTime ProcessStartedTime
		{
			get
			{
				return this.m_serviceProcessStartTime;
			}
		}

		protected string ServiceName { get; set; }

		public Exception StartService()
		{
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "StartService({0}) called", this.ServiceName);
			Exception ex = ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
			{
				this.m_scm.Start();
				this.WaitForStart();
			});
			if (ex != null)
			{
				ExTraceGlobals.ClusterEventsTracer.TraceError<string, Exception>(0L, "StartService({0}) fails: {1}", this.ServiceName, ex);
				ReplayEventLogConstants.Tuple_AmFailedToStartService.LogEvent(string.Empty, new object[]
				{
					this.ServiceName,
					ex.ToString()
				});
			}
			return ex;
		}

		public ServiceStartMode GetStartMode()
		{
			ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Service WHERE Name='" + this.ServiceName + "'");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					using (managementObject)
					{
						AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "service={0}", managementObject.ToString());
						string mode = (string)managementObject["StartMode"];
						return this.MapStartMode(mode);
					}
				}
				throw new ArgumentException("no such service");
			}
			ServiceStartMode result;
			return result;
		}

		public Exception StopService()
		{
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "StopService({0}) called", this.ServiceName);
			Exception ex = ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
			{
				this.m_scm.Stop();
				this.WaitForStop();
				this.m_serviceProcessId = -1;
			});
			if (ex != null)
			{
				ExTraceGlobals.ClusterEventsTracer.TraceError<string, Exception>(0L, "StopService({0}) fails: {1}", this.ServiceName, ex);
				ReplayEventLogConstants.Tuple_AmFailedToStopService.LogEvent(string.Empty, new object[]
				{
					this.ServiceName,
					ex.ToString()
				});
			}
			return ex;
		}

		protected override void PollerThread()
		{
			AmServiceMonitor.Tracer.TraceFunction<string>((long)this.GetHashCode(), "Entering Service monitor '{0}'", this.ServiceName);
			try
			{
				this.StartMonitoring();
			}
			catch (AmServiceMonitorSystemShutdownException arg)
			{
				AmServiceMonitor.Tracer.TraceWarning<string, AmServiceMonitorSystemShutdownException>((long)this.GetHashCode(), "'{0}' service monitor is exiting since system shutdown is in progress (Exception: {1})", this.ServiceName, arg);
			}
			AmServiceMonitor.Tracer.TraceFunction<string>((long)this.GetHashCode(), "Leaving Service monitor '{0}'", this.ServiceName);
		}

		protected abstract void OnStart();

		protected abstract void OnStop();

		protected virtual void OnWaitingForStart()
		{
		}

		protected virtual void OnWaitingForStop()
		{
		}

		protected virtual bool IsServiceReady()
		{
			return true;
		}

		private void StartMonitoring()
		{
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Starting service monitor for {0}", this.ServiceName);
			while (!this.m_fShutdown)
			{
				this.WaitForStart();
				if (this.m_fShutdown)
				{
					break;
				}
				AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Service monitor detected that '{0}' is started", this.ServiceName);
				ReplayCrimsonEvents.ServiceStartDetected.Log<string>(this.ServiceName);
				this.OnStart();
				this.WaitForStop();
				if (this.m_fShutdown)
				{
					break;
				}
				AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Service monitor detected that '{0}' is stopped", this.ServiceName);
				ReplayCrimsonEvents.ServiceStopDetected.Log<string>(this.ServiceName);
				this.OnStop();
			}
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Service monitor monitoring stopped for {0}", this.ServiceName);
		}

		private void WaitForStart()
		{
			this.m_serviceProcessId = -1;
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WaitForStart({0}) called", this.ServiceName);
			while (!this.m_fShutdown)
			{
				try
				{
					if (this.CheckServiceStatus(ServiceControllerStatus.Running) && this.IsServiceReady())
					{
						Exception ex;
						using (Process serviceProcess = ServiceOperations.GetServiceProcess(this.ServiceName, out ex))
						{
							if (serviceProcess != null)
							{
								Exception ex2 = null;
								try
								{
									this.m_serviceProcessId = serviceProcess.Id;
									this.m_serviceProcessStartTime = serviceProcess.StartTime;
									break;
								}
								catch (Win32Exception ex3)
								{
									ex2 = ex3;
								}
								catch (InvalidOperationException ex4)
								{
									ex2 = ex4;
								}
								if (ex2 != null)
								{
									AmTrace.Error("Service status for {0} is Running, but unable to read the process object details. Ex = {1}", new object[]
									{
										this.ServiceName,
										ex2
									});
									this.m_serviceProcessId = -1;
								}
							}
							else
							{
								AmTrace.Error("Service status for {0} is Running, but unable to get the process object", new object[]
								{
									this.ServiceName
								});
							}
						}
					}
					if (this.m_shutdownEvent.WaitOne(5000, false))
					{
						break;
					}
					this.OnWaitingForStart();
				}
				catch (Win32Exception e)
				{
					if (!this.HandleKnownException(e))
					{
						throw;
					}
				}
				catch (InvalidOperationException e2)
				{
					if (!this.HandleKnownException(e2))
					{
						throw;
					}
				}
			}
		}

		private void WaitForStop()
		{
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WaitForStop({0}) called", this.ServiceName);
			using (PrivilegeControl privilegeControl = new PrivilegeControl())
			{
				Exception arg;
				if (!privilegeControl.TryEnable("SeDebugPrivilege", out arg))
				{
					AmServiceMonitor.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "WaitForStop({0}) failed to set debug priv: {1}", this.ServiceName, arg);
				}
				this.WaitForStopInternal();
			}
			AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WaitForStop({0}) exits", this.ServiceName);
		}

		private void WaitForStopInternal()
		{
			while (!this.m_fShutdown)
			{
				Process process = null;
				ProcessHandle processHandle = null;
				WaitHandle[] waitHandles = null;
				int millisecondsTimeout = 30000;
				try
				{
					while (!this.m_fShutdown)
					{
						if (this.CheckServiceStatus(ServiceControllerStatus.Stopped))
						{
							AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Service stop detected for {0} based on the result from service controller", this.ServiceName);
							return;
						}
						Exception ex = null;
						if (process == null)
						{
							process = ServiceOperations.GetProcessByIdBestEffort(this.m_serviceProcessId, out ex);
							if (process == null)
							{
								AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Service stop detected for {0} since process is not running anymore", this.ServiceName);
								return;
							}
							try
							{
								if (!process.StartTime.Equals(this.m_serviceProcessStartTime))
								{
									AmServiceMonitor.Tracer.TraceDebug<string, DateTime, DateTime>((long)this.GetHashCode(), "Service stop detected for {0} by the change in start times (prev={1}, current={2})", this.ServiceName, this.m_serviceProcessStartTime, process.StartTime);
									return;
								}
							}
							catch (InvalidOperationException ex2)
							{
								AmServiceMonitor.Tracer.TraceError<string, string>((long)this.GetHashCode(), "ps.StartTime for service {0} generated exception {1} . Assuming that the process had exited", this.ServiceName, ex2.Message);
								return;
							}
							if (processHandle == null)
							{
								processHandle = new ProcessHandle();
							}
							Exception arg;
							if (processHandle.TryGetWaitHandle(process, out arg))
							{
								millisecondsTimeout = 30000;
								waitHandles = new WaitHandle[]
								{
									this.m_shutdownEvent,
									processHandle.WaitHandle
								};
							}
							else
							{
								AmServiceMonitor.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "WaitForStop({0}) hit exception opening process handle: {1}", this.ServiceName, arg);
								waitHandles = new WaitHandle[]
								{
									this.m_shutdownEvent
								};
								millisecondsTimeout = 5000;
							}
						}
						int num = WaitHandle.WaitAny(waitHandles, millisecondsTimeout);
						if (num == 0)
						{
							AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Shutdown signalled for {0}", this.ServiceName);
							return;
						}
						if (num == 1)
						{
							AmServiceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Process exit detected for {0}", this.ServiceName);
							return;
						}
						this.OnWaitingForStop();
					}
				}
				catch (Win32Exception ex3)
				{
					AmServiceMonitor.Tracer.TraceError<string, string>((long)this.GetHashCode(), "WaitForStop({0}) encountered exception: {1}", this.ServiceName, ex3.Message);
					if (!this.HandleKnownException(ex3))
					{
						throw;
					}
				}
				catch (InvalidOperationException ex4)
				{
					AmServiceMonitor.Tracer.TraceError<string, string>((long)this.GetHashCode(), "WaitForStop({0}) encountered exception: {1}", this.ServiceName, ex4.Message);
					if (!this.HandleKnownException(ex4))
					{
						throw;
					}
				}
				finally
				{
					if (processHandle != null)
					{
						processHandle.Dispose();
						processHandle = null;
					}
					if (process != null)
					{
						process.Dispose();
						process = null;
					}
				}
			}
		}

		private bool HandleKnownException(Exception e)
		{
			bool result = false;
			if (AmExceptionHelper.CheckExceptionCode(e, 1060U))
			{
				result = true;
				this.m_shutdownEvent.WaitOne(30000, false);
			}
			else if (AmExceptionHelper.CheckExceptionCode(e, 1115U))
			{
				throw new AmServiceMonitorSystemShutdownException(this.ServiceName);
			}
			return result;
		}

		private ServiceStartMode MapStartMode(string mode)
		{
			if (SharedHelper.StringIEquals(mode, "Manual"))
			{
				return ServiceStartMode.Manual;
			}
			if (SharedHelper.StringIEquals(mode, "Disabled"))
			{
				return ServiceStartMode.Disabled;
			}
			return ServiceStartMode.Automatic;
		}

		private bool CheckServiceStatus(ServiceControllerStatus status)
		{
			this.m_scm.Refresh();
			return this.m_scm.Status.Equals(status);
		}

		protected const int InvalidProcessId = -1;

		private const int m_WaitTimeoutFastMs = 5000;

		private const int m_WaitTimeoutSlowMs = 30000;

		private ServiceController m_scm;

		private int m_serviceProcessId = -1;

		private DateTime m_serviceProcessStartTime;
	}
}
