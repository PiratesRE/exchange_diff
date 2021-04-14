using System;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;
using Microsoft.Exchange.Transport.Sync.Migration;
using Microsoft.Exchange.Transport.Sync.Worker.Agents;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AggregationComponent : IStartableTransportComponent, ITransportComponent
	{
		private AggregationComponent()
		{
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return AggregationComponent.eventLogger;
			}
		}

		public string CurrentState
		{
			get
			{
				string result;
				lock (this.syncRoot)
				{
					result = ((this.scheduler != null) ? this.scheduler.CurrentState : "Unknown");
				}
				return result;
			}
		}

		private bool ShouldExecute
		{
			get
			{
				return this.transportSyncEnabled && this.targetRunningState == ServiceState.Active;
			}
		}

		public static IStartableTransportComponent CreateAggregationComponent()
		{
			return AggregationComponent.instance;
		}

		public void Load()
		{
			lock (this.instrumentationSyncRoot)
			{
				this.lastLoadCallstack = this.lastLoadCallstack + "Load start.  Callstack:" + Environment.StackTrace;
			}
			this.Initialize();
			this.syncLogSession.LogInformation((TSLID)433UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Load.", new object[0]);
			if (AggregationConfiguration.IsDatacenterMode)
			{
				this.remoteServerHealthManager = new RemoteServerHealthManager(this.syncLogSession, AggregationConfiguration.Instance, new Action<EventLogEntry>(AggregationComponent.LogEvent), new Action<RemoteServerHealthData>(AggregationComponent.LogRemoteServerHealth));
				this.scheduler = new AggregationScheduler(this.syncLogSession);
				this.scheduler.EnabledAggregationTypes = AggregationConfiguration.Instance.EnabledAggregationTypes;
				SyncPoisonHandler.Load(this.syncLogSession);
				SubscriptionAgentManager.Instance.RegisterAgentFactory(new SyncMigrationSubscriptionAgentFactory());
				SubscriptionAgentManager.Instance.RegisterAgentFactory(new PeopleConnectSubscriptionAgentFactory());
				SubscriptionAgentManager.Instance.Start(this.syncLogSession, AggregationConfiguration.Instance.DisabledSubscriptionAgents);
				this.StartRpcServer();
				lock (this.instrumentationSyncRoot)
				{
					this.lastLoadCallstack = this.lastLoadCallstack + Environment.NewLine + "Load end" + Environment.NewLine;
				}
			}
		}

		public void Unload()
		{
			this.syncLogSession.LogInformation((TSLID)434UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Unload.", new object[0]);
			this.Shutdown();
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			this.syncLogSession.LogInformation((TSLID)435UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Start. (Initially Paused={0})", new object[]
			{
				initiallyPaused
			});
			this.transportSyncPaused = initiallyPaused;
			this.targetRunningState = targetRunningState;
			if (!AggregationConfiguration.IsDatacenterMode)
			{
				this.syncLogSession.LogError((TSLID)436UL, AggregationComponent.diag, (long)this.GetHashCode(), "Transport Sync is not enabled. Will not Start AggregationComponent.", new object[0]);
				return;
			}
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjectionUtil.Callback));
			if (this.ShouldExecute)
			{
				this.StartScheduler();
			}
			this.syncDiagnostics = new SyncDiagnostics(this.scheduler, this.remoteServerHealthManager);
			this.syncDiagnostics.Register();
		}

		public void Stop()
		{
			this.syncLogSession.LogInformation((TSLID)437UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Stop.", new object[0]);
			this.Shutdown();
		}

		public void Pause()
		{
			this.syncLogSession.LogInformation((TSLID)438UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Pause. (Enabled={0}, Service State={1})", new object[]
			{
				this.transportSyncEnabled,
				this.targetRunningState
			});
			lock (this.syncRoot)
			{
				this.transportSyncPaused = true;
				if (this.ShouldExecute)
				{
					this.PauseScheduler();
				}
			}
		}

		public void Continue()
		{
			this.syncLogSession.LogInformation((TSLID)439UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Continue. (Enabled={0}, Service state={1})", new object[]
			{
				this.transportSyncEnabled,
				this.targetRunningState
			});
			lock (this.syncRoot)
			{
				this.transportSyncPaused = false;
				if (this.ShouldExecute)
				{
					this.StartScheduler();
				}
			}
		}

		public string OnUnhandledException(Exception e)
		{
			if (!this.ShouldExecute)
			{
				return null;
			}
			SyncLogSession syncLogSession = this.syncLogSession;
			if (syncLogSession != null)
			{
				syncLogSession.LogInformation((TSLID)440UL, AggregationComponent.diag, (long)this.GetHashCode(), "UnhandledException handler invoked for unhandled exception: {0}.", new object[]
				{
					e
				});
			}
			SyncPoisonHandler.SavePoisonContext(e, syncLogSession);
			if (syncLogSession != null)
			{
				syncLogSession.AddBlackBoxLogToWatson();
			}
			return null;
		}

		internal static void LogEvent(EventLogEntry eventLogEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("eventLogEntry", eventLogEntry);
			AggregationComponent.EventLogger.LogEvent(eventLogEntry.Tuple, eventLogEntry.PeriodicKey, eventLogEntry.MessageArgs);
		}

		internal static void LogRemoteServerHealth(RemoteServerHealthData healthData)
		{
			SyncUtilities.ThrowIfArgumentNull("healthData", healthData);
			string machineName = Environment.MachineName;
			DateTime lastBackOffStartTime = (healthData.LastBackOffStartTime != null) ? healthData.LastBackOffStartTime.Value.UniversalTime : DateTime.MinValue;
			AggregationConfiguration.Instance.SyncHealthLog.LogRemoteServerHealth(machineName, healthData.ServerName, healthData.State.ToString(), healthData.BackOffCount, lastBackOffStartTime, healthData.LastUpdateTime.UniversalTime);
		}

		internal static SubscriptionSubmissionResult SubmitWorkItem(AggregationWorkItem workItem)
		{
			SubscriptionSubmissionResult result;
			lock (AggregationComponent.instance.syncRoot)
			{
				if (AggregationComponent.instance.scheduler != null)
				{
					result = AggregationComponent.instance.scheduler.SubmitWorkItem(workItem);
				}
				else
				{
					result = SubscriptionSubmissionResult.EdgeTransportStopped;
				}
			}
			return result;
		}

		internal static void RecordRemoteServerLatency(string remoteServerName, long remoteServerLatency)
		{
			AggregationComponent.instance.remoteServerHealthManager.RecordRemoteServerLatency(remoteServerName, remoteServerLatency);
		}

		internal static RemoteServerHealthState CalculateRemoteServerHealth(string remoteServerName, bool isPartnerRemoteServer)
		{
			return AggregationComponent.instance.remoteServerHealthManager.CalculateRemoteServerHealth(remoteServerName, isPartnerRemoteServer);
		}

		private void Shutdown()
		{
			this.syncLogSession.LogInformation((TSLID)441UL, AggregationComponent.diag, (long)this.GetHashCode(), "AggregationComponent Shutdown.", new object[0]);
			lock (this.syncRoot)
			{
				if (this.syncDiagnostics != null)
				{
					this.syncDiagnostics.Unregister();
					this.syncDiagnostics = null;
				}
				if (this.subscriptionSubmit != null)
				{
					this.subscriptionSubmit.Stop();
					this.subscriptionSubmit = null;
				}
				if (this.scheduler != null)
				{
					this.scheduler.Unload();
					this.scheduler = null;
				}
				if (this.remoteServerHealthManager != null)
				{
					this.remoteServerHealthManager.Dispose();
					this.remoteServerHealthManager = null;
				}
				SubscriptionAgentManager.Instance.Shutdown();
				SyncStorageProviderFactory.EnableRegistration();
			}
		}

		private void StartScheduler()
		{
			this.syncLogSession.LogInformation((TSLID)442UL, AggregationComponent.diag, (long)this.GetHashCode(), "Start the scheduler.", new object[0]);
			lock (this.syncRoot)
			{
				if (this.scheduler != null)
				{
					this.scheduler.Start(this.transportSyncPaused);
				}
			}
		}

		private void StopScheduler()
		{
			this.syncLogSession.LogInformation((TSLID)443UL, AggregationComponent.diag, (long)this.GetHashCode(), "Stop the scheduler.", new object[0]);
			lock (this.syncRoot)
			{
				if (this.scheduler != null)
				{
					this.scheduler.Stop();
				}
			}
		}

		private void PauseScheduler()
		{
			this.syncLogSession.LogInformation((TSLID)325UL, AggregationComponent.diag, (long)this.GetHashCode(), "Pause the scheduler.", new object[0]);
			lock (this.syncRoot)
			{
				if (this.scheduler != null)
				{
					this.scheduler.Pause();
				}
			}
		}

		private void StartRpcServer()
		{
			FileSecurity fileSecurity = new FileSecurity();
			IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 631, "StartRpcServer", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Worker\\Core\\AggregationComponent.cs");
			SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
			FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.SetAccessRule(fileSystemAccessRule);
			this.exchangeServerSddl = fileSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			fileSecurity.SetOwner(securityIdentifier);
			this.rpcSddl = fileSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			this.subscriptionSubmit = (SubscriptionSubmissionServer)RpcServerBase.RegisterServer(typeof(SubscriptionSubmissionServer), fileSecurity, 1, false, (uint)(Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions / 2));
			this.subscriptionSubmit.Start();
			this.syncLogSession.LogDebugging((TSLID)444UL, AggregationComponent.diag, (long)this.GetHashCode(), "RPC Server started with exchangerServerSDDL {0}, rpcSDDL {1}.", new object[]
			{
				this.exchangeServerSddl,
				this.rpcSddl
			});
		}

		private void Initialize()
		{
			this.syncLogSession = AggregationConfiguration.Instance.SyncLog.OpenSession();
			if (!AggregationConfiguration.IsDatacenterMode)
			{
				this.transportSyncEnabled = false;
				return;
			}
			this.transportSyncEnabled = Components.Configuration.LocalServer.TransportServer.TransportSyncEnabled;
			Components.Configuration.LocalServerChanged += this.UpdateTransportServerConfig;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType("Microsoft.Exchange.MailboxTransport.ContentAggregation.FrameworkAggregationConfiguration");
			type.InvokeMember("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.ExactBinding, null, null, null);
			Type type2 = executingAssembly.GetType("Microsoft.Exchange.MailboxTransport.ContentAggregation.FrameworkPerfCounterHandler");
			type2.InvokeMember("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.ExactBinding, null, null, null);
			this.LoadSyncStorageProviders(executingAssembly);
		}

		private void LoadSyncStorageProviders(Assembly workerAssembly)
		{
			foreach (Type type in workerAssembly.GetTypes())
			{
				Type @interface = type.GetInterface("ISyncStorageProvider");
				if (@interface != null && !type.IsAbstract)
				{
					this.syncLogSession.LogInformation((TSLID)445UL, AggregationComponent.diag, (long)this.GetHashCode(), "Creating SyncStorageProvider for type: " + type.FullName, new object[0]);
					object obj = workerAssembly.CreateInstance(type.FullName);
					ISyncStorageProvider syncStorageProvider = (ISyncStorageProvider)obj;
					SyncStorageProviderFactory.Register(syncStorageProvider, syncStorageProvider.SubscriptionType);
					this.syncLogSession.LogInformation((TSLID)446UL, AggregationComponent.diag, (long)this.GetHashCode(), "Registered {0} with SyncStorageProviderFactory.", new object[]
					{
						type.FullName
					});
				}
			}
			SyncStorageProviderFactory.DisableRegistration();
		}

		private void UpdateTransportServerConfig(TransportServerConfiguration updatedTransportServerConfig)
		{
			this.syncLogSession.LogInformation((TSLID)447UL, AggregationComponent.diag, (long)this.GetHashCode(), "Server configuration updated. (Enabled: old={0}, new={1})", new object[]
			{
				this.transportSyncEnabled,
				updatedTransportServerConfig.TransportServer.TransportSyncEnabled
			});
			AggregationConfiguration.Instance.UpdateConfiguration(updatedTransportServerConfig.TransportServer);
			lock (this.syncRoot)
			{
				bool flag2 = this.transportSyncEnabled;
				bool flag3 = updatedTransportServerConfig.TransportServer.TransportSyncEnabled;
				if (flag2 ^ flag3)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartOrStopSchedulerWorker), null);
				}
				if (this.scheduler != null)
				{
					this.scheduler.EnabledAggregationTypes = AggregationConfiguration.Instance.EnabledAggregationTypes;
				}
			}
		}

		private void StartOrStopSchedulerWorker(object stateInfo)
		{
			lock (this.syncRoot)
			{
				bool shouldExecute = this.ShouldExecute;
				this.transportSyncEnabled = AggregationConfiguration.Instance.LocalTransportServer.TransportSyncEnabled;
				if (shouldExecute && !this.ShouldExecute)
				{
					this.StopScheduler();
				}
				else if (!shouldExecute && this.ShouldExecute)
				{
					this.StartScheduler();
				}
			}
		}

		public static readonly string Name = "ContentAggregation";

		private static readonly Trace diag = ExTraceGlobals.AggregationComponentTracer;

		private static AggregationComponent instance = new AggregationComponent();

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.EventLogTracer.Category, "MSExchangeTransportSyncWorker");

		private readonly object syncRoot = new object();

		private readonly object instrumentationSyncRoot = new object();

		private AggregationScheduler scheduler;

		private SyncLogSession syncLogSession;

		private bool transportSyncEnabled;

		private bool transportSyncPaused;

		private string exchangeServerSddl;

		private string rpcSddl;

		private SubscriptionSubmissionServer subscriptionSubmit;

		private SyncDiagnostics syncDiagnostics;

		private RemoteServerHealthManager remoteServerHealthManager;

		private string lastLoadCallstack = string.Empty;

		private ServiceState targetRunningState;
	}
}
