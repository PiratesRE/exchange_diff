using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.HA.SupportApi;

namespace Microsoft.Exchange.HA.Services
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerLocatorManager : IServiceComponent
	{
		public static ServerLocatorManager Instance
		{
			get
			{
				if (ServerLocatorManager.instance == null)
				{
					lock (ServerLocatorManager.syncRoot)
					{
						if (ServerLocatorManager.instance == null)
						{
							ServerLocatorManager.instance = new ServerLocatorManager();
						}
					}
				}
				return ServerLocatorManager.instance;
			}
		}

		private static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ServerLocatorServiceTracer;
			}
		}

		public string Name
		{
			get
			{
				return "ServerLocator";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.ServerLocatorService;
			}
		}

		public bool IsCritical
		{
			get
			{
				return false;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public bool IsRunning
		{
			get
			{
				return this.isRunning;
			}
		}

		public IMonitoringADConfigProvider ADConfigProvider { get; private set; }

		public ICopyStatusClientLookup CopyStatusLookup { get; private set; }

		public ServerLocatorPerfmonCounters Counters { get; private set; }

		private ServerLocatorManager()
		{
		}

		private static ServerLocatorPerfmonCounters GetPerfCounters()
		{
			return new ServerLocatorPerfmonCounters();
		}

		public bool Start()
		{
			Exception ex;
			return this.Start(out ex);
		}

		public bool Start(out Exception ex)
		{
			ex = null;
			if (this.isRunning)
			{
				ServerLocatorManager.Tracer.TraceDebug(0L, "Server Locator Manager is already started.");
				return true;
			}
			bool flag = false;
			bool result;
			try
			{
				object obj;
				Monitor.Enter(obj = ServerLocatorManager.syncRoot, ref flag);
				if (this.isRunning)
				{
					ServerLocatorManager.Tracer.TraceDebug(0L, "Server Locator Manager is already started.");
					result = true;
				}
				else
				{
					Exception serviceHostException = null;
					try
					{
						if (this.m_restartManager == null)
						{
							ServerLocatorManager.Tracer.TraceDebug(0L, "Creating restart manager that will constantly monitor service.");
							this.m_restartManager = new ServerLocatorManager.RestartServerLocator(this, ServerLocatorManager.restartInterval);
							this.m_restartManager.Start();
						}
						ServerLocatorManager.Tracer.TraceDebug(0L, "Creating caching active manager client for Server Locator Service.");
						this.ADConfigProvider = Dependencies.MonitoringADConfigProvider;
						this.CopyStatusLookup = Dependencies.MonitoringCopyStatusClientLookup;
						this.Counters = ServerLocatorManager.GetPerfCounters();
						ServerLocatorManager.Tracer.TraceDebug<string>(0L, "Starting Server Locator Service on {0}.", ServerLocatorManager.baseAddress.AbsoluteUri);
						this.serviceHost = new ServiceHost(typeof(ServerLocator), new Uri[]
						{
							ServerLocatorManager.baseAddress
						});
						this.serviceHost.Faulted += this.RestartServiceHost;
						NetTcpBinding netTcpBinding = new NetTcpBinding();
						netTcpBinding.MaxBufferPoolSize = 16777216L;
						netTcpBinding.MaxBufferSize = 16777216;
						netTcpBinding.MaxConnections = 200;
						netTcpBinding.MaxReceivedMessageSize = 16777216L;
						netTcpBinding.ReaderQuotas.MaxDepth = 128;
						netTcpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
						netTcpBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
						netTcpBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
						netTcpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
						this.serviceHost.AddServiceEndpoint(typeof(IServerLocator), netTcpBinding, "ServerLocator");
						ServiceThrottlingBehavior serviceThrottlingBehavior = new ServiceThrottlingBehavior();
						serviceThrottlingBehavior.MaxConcurrentCalls = RegistryParameters.WcfMaxConcurrentCalls;
						serviceThrottlingBehavior.MaxConcurrentSessions = RegistryParameters.WcfMaxConcurrentSessions;
						serviceThrottlingBehavior.MaxConcurrentInstances = RegistryParameters.WcfMaxConcurrentInstances;
						ServiceThrottlingBehavior serviceThrottlingBehavior2 = this.serviceHost.Description.Behaviors.Find<ServiceThrottlingBehavior>();
						if (serviceThrottlingBehavior2 == null)
						{
							this.serviceHost.Description.Behaviors.Add(serviceThrottlingBehavior);
						}
						else
						{
							this.serviceHost.Description.Behaviors.Remove(serviceThrottlingBehavior2);
							this.serviceHost.Description.Behaviors.Add(serviceThrottlingBehavior);
						}
						ServiceMetadataBehavior item = new ServiceMetadataBehavior();
						this.serviceHost.Description.Behaviors.Add(item);
						if (RegistryParameters.WcfEnableMexEndpoint)
						{
							ServerLocatorManager.Tracer.TraceDebug(0L, "Creating Mex binding.");
							ExAssert.RetailAssert(RegistryParameters.HighAvailabilityWebServiceMexPort != RegistryParameters.HighAvailabilityWebServicePort, "Metadata Exchange port should be different from Server Locator web service port.");
							Binding binding = MetadataExchangeBindings.CreateMexTcpBinding();
							this.serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), binding, ServerLocatorManager.baseMetadataExchangeAddress);
						}
						InvokeWithTimeout.Invoke(delegate()
						{
							InvalidOperationException serviceHostException;
							try
							{
								this.serviceHost.Open();
							}
							catch (InvalidOperationException serviceHostException)
							{
								serviceHostException = serviceHostException;
							}
							catch (CommunicationException serviceHostException2)
							{
								serviceHostException = serviceHostException2;
							}
							catch (SocketException serviceHostException3)
							{
								serviceHostException = serviceHostException3;
							}
							catch (Win32Exception serviceHostException4)
							{
								serviceHostException = serviceHostException4;
							}
							if (serviceHostException != null)
							{
								ServerLocatorManager.Tracer.TraceError<string>(0L, "InokeWithTimeout() failed to start Server Locator Service communication channel. Error: {0}", serviceHostException.Message);
							}
						}, ServerLocatorManager.startTimeout);
						if (serviceHostException == null)
						{
							this.isRunning = true;
							ServerLocatorManager.Tracer.TraceDebug(0L, "Server Locator Service started.");
							ReplayEventLogConstants.Tuple_ServerLocatorServiceStarted.LogEvent(null, new object[]
							{
								ServerLocatorManager.baseAddress.AbsoluteUri
							});
							return true;
						}
					}
					catch (TimeoutException ex2)
					{
						ReplayEventLogConstants.Tuple_ServerLocatorServiceStartTimeout.LogEvent(null, new object[]
						{
							ServerLocatorManager.baseAddress.AbsoluteUri,
							this.GenerateDiagnosticException("Timeout during starting server locator WCF service from ServerLocatorManager.Start()", ex2).Message
						});
						ex = ex2;
					}
					if (serviceHostException != null)
					{
						ex = serviceHostException;
					}
					if (ex != null)
					{
						ServerLocatorManager.Tracer.TraceError<string>(0L, "Start() failed to start Server Locator Service communication channel. Error: {0}", ex.Message);
					}
					if (this.serviceHost != null)
					{
						ServerLocatorManager.Tracer.TraceDebug(0L, "Aborting server locator service communication channel from Start().");
						this.serviceHost.Abort();
					}
					ServerLocatorManager.Tracer.TraceError<string, string>(0L, "Failed to start Server Locator Service on {0}. Error: {1}", ServerLocatorManager.baseAddress.AbsoluteUri, ex.Message);
					ReplayEventLogConstants.Tuple_ServerLocatorServiceFailedToStart.LogEvent(null, new object[]
					{
						ServerLocatorManager.baseAddress.AbsoluteUri,
						ex.Message
					});
					this.m_restartManager.ChangeTimer(ServerLocatorManager.startNowInterval, ServerLocatorManager.restartInterval);
					result = false;
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
			return result;
		}

		internal bool InitiateRestartSequence()
		{
			if (this.isRestartRequired)
			{
				this.isRestartRequired = false;
				ServerLocatorManager.Tracer.TraceDebug(0L, "Restart will be attmpted now.");
				ReplayEventLogConstants.Tuple_ServerLocatorServiceRestartScheduled.LogEvent(this.Name, new object[]
				{
					TimeSpan.Zero.TotalSeconds
				});
				return true;
			}
			return false;
		}

		internal void RestartServiceHost(object sender, EventArgs e)
		{
			if (sender is SupportApiService)
			{
				ServerLocatorManager.Tracer.TraceDebug(0L, "Server Locator restart called from support api service.");
				ReplayEventLogConstants.Tuple_ServerLocatorServiceCommunicationChannelFaulted.LogEvent(null, new object[]
				{
					"Triggered by Support API."
				});
			}
			else
			{
				ServerLocatorManager.Tracer.TraceError<CommunicationState>(0L, "Service Host communication channel is {0}. Because of this error we need to stop and re-start service host.", this.serviceHost.State);
				ReplayEventLogConstants.Tuple_ServerLocatorServiceCommunicationChannelFaulted.LogEvent(null, new object[]
				{
					this.serviceHost.State,
					this.GenerateDiagnosticException("Service host communication channel faulted event received").Message
				});
			}
			this.isRestartRequired = true;
			if (this.m_restartManager != null)
			{
				bool flag = false;
				try
				{
					Monitor.TryEnter(ServerLocatorManager.syncRoot, 1000, ref flag);
					if (flag)
					{
						if (this.m_restartManager != null)
						{
							ServerLocatorManager.Tracer.TraceDebug(0L, "Pinged server locator restart manager to restart now.");
							this.m_restartManager.ChangeTimer(ServerLocatorManager.startNowInterval, ServerLocatorManager.restartInterval);
						}
						else
						{
							ServerLocatorManager.Tracer.TraceError(0L, "Cannot ping server locator restart manager because it is not available.");
						}
					}
					else
					{
						ServerLocatorManager.Tracer.TraceError(0L, "Cannot get lock to ping server locator restart manager.");
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(ServerLocatorManager.syncRoot);
					}
				}
			}
		}

		public void Stop()
		{
			this.Stop(false);
		}

		public void Stop(bool aborting)
		{
			lock (ServerLocatorManager.syncRoot)
			{
				ServerLocatorManager.Tracer.TraceDebug<bool>(0L, "Server Locator Service Stop({0}).", aborting);
				if (this.serviceHost != null)
				{
					ServerLocatorManager.Tracer.TraceDebug<CommunicationState, bool>(0L, "Server Locator Service communication channel is {0}. Aborting flag is: {1}.", this.serviceHost.State, aborting);
					if (aborting)
					{
						ServerLocatorManager.Tracer.TraceDebug(0L, "Aborting Server Locator Service communication channel.");
						this.serviceHost.Abort();
					}
					else
					{
						try
						{
							ServerLocatorManager.Tracer.TraceDebug(0L, "Closing Server Locator Service communication channel.");
							InvokeWithTimeout.Invoke(delegate()
							{
								this.serviceHost.Close();
							}, ServerLocatorManager.startTimeout);
						}
						catch (CommunicationException ex)
						{
							ServerLocatorManager.Tracer.TraceError<string>(0L, "Unable to close Server Locator Service communication channel gracefully due to: {0}", ex.Message);
							this.serviceHost.Abort();
						}
						catch (TimeoutException ex2)
						{
							ServerLocatorManager.Tracer.TraceError<string>(0L, "Unable to close Server Locator Service commuication channel gracefully due to: {0}", ex2.Message);
							this.serviceHost.Abort();
						}
					}
				}
				if (!aborting && this.m_restartManager != null)
				{
					ServerLocatorManager.Tracer.TraceDebug(0L, "Stopping restart manager.");
					this.m_restartManager.Stop();
					this.m_restartManager = null;
				}
				ServerLocatorManager.Tracer.TraceDebug(0L, "Server Locator Service stopped.");
				ReplayEventLogConstants.Tuple_ServerLocatorServiceStopped.LogEvent(null, new object[0]);
				this.isRunning = false;
			}
		}

		private Exception GenerateDiagnosticException(string errorMessage)
		{
			return this.GenerateDiagnosticException(errorMessage, null);
		}

		private Exception GenerateDiagnosticException(string errorMessage, Exception inner)
		{
			Exception ex = null;
			NativeMethods.SocketData openSocketByPort = NativeMethods.GetOpenSocketByPort(RegistryParameters.HighAvailabilityWebServicePort);
			if (openSocketByPort != null)
			{
				try
				{
					try
					{
						using (Process processById = Process.GetProcessById(openSocketByPort.OwnerPid))
						{
							using (Process currentProcess = Process.GetCurrentProcess())
							{
								ReplayEventLogConstants.Tuple_ServerLocatorServiceAnotherProcessUsingPort.LogEvent(this.Name, new object[]
								{
									RegistryParameters.HighAvailabilityWebServicePort,
									processById.ProcessName,
									openSocketByPort.OwnerPid,
									currentProcess.ProcessName,
									currentProcess.Id
								});
								ex = new Exception(string.Format("Server locator manager error: {0}. Process using port is: '{1}' (pid: {2}). Current process: '{3}' (pid: {4})", new object[]
								{
									errorMessage,
									processById.ProcessName,
									openSocketByPort.OwnerPid,
									currentProcess.ProcessName,
									currentProcess.Id
								}), inner);
							}
						}
					}
					catch (ArgumentException)
					{
					}
					catch (InvalidOperationException)
					{
					}
					catch (Win32Exception)
					{
					}
					return ex;
				}
				finally
				{
					if (ex == null)
					{
						try
						{
							using (Process currentProcess2 = Process.GetCurrentProcess())
							{
								ex = new Exception(string.Format("Server locator manager error: {0}. Process using port is: '<unknown>' (pid: {1}). Current process: {2} (pid: {3})", new object[]
								{
									errorMessage,
									openSocketByPort.OwnerPid,
									currentProcess2.ProcessName,
									currentProcess2.Id
								}), inner);
							}
						}
						catch (Win32Exception)
						{
							ex = new Exception(string.Format("Server locator manager error: {0}. Process using port is: '<unknown>' (pid: {1}).", errorMessage, openSocketByPort.OwnerPid), inner);
						}
					}
				}
			}
			try
			{
				using (Process currentProcess3 = Process.GetCurrentProcess())
				{
					ex = new Exception(string.Format("Server locator manager error: {0}. No other process using the port was found. Current process: {1} (pid: {2})", errorMessage, currentProcess3.ProcessName, currentProcess3.Id), inner);
				}
			}
			catch (Win32Exception)
			{
				ex = new Exception(string.Format("Server locator manager error: {0}. No other process using the port was found.", errorMessage), inner);
			}
			return ex;
		}

		private static volatile ServerLocatorManager instance;

		private static object syncRoot = new object();

		private static Uri baseAddress = new Uri(string.Format("net.tcp://{0}:{1}/Exchange.HighAvailability", AmServerName.LocalComputerName.Fqdn, RegistryParameters.HighAvailabilityWebServicePort));

		private static Uri baseMetadataExchangeAddress = new Uri(string.Format("net.tcp://{0}:{1}/Exchange.HighAvailability/mex", AmServerName.LocalComputerName.Fqdn, RegistryParameters.HighAvailabilityWebServiceMexPort));

		private static TimeSpan restartInterval = TimeSpan.FromMinutes(5.0);

		private static TimeSpan startNowInterval = TimeSpan.Zero;

		private static TimeSpan startTimeout = TimeSpan.FromMinutes(1.0);

		private ServiceHost serviceHost;

		private bool isRestartRequired;

		private bool isRunning;

		private ServerLocatorManager.RestartServerLocator m_restartManager;

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class RestartServerLocator : TimerComponent
		{
			public RestartServerLocator(ServerLocatorManager serverLocatorManager, TimeSpan periodicStartInterval) : base(periodicStartInterval, periodicStartInterval, "RestartServerLocator")
			{
				this.m_serverLocatorManager = serverLocatorManager;
			}

			protected override void TimerCallbackInternal()
			{
				if (this.m_serverLocatorManager.InitiateRestartSequence())
				{
					this.m_serverLocatorManager.Stop(true);
					this.m_serverLocatorManager.Start();
				}
			}

			private ServerLocatorManager m_serverLocatorManager;
		}
	}
}
