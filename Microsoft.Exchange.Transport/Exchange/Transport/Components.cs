using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessageSecurity;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.IPFilter;
using Microsoft.Exchange.Rpc.MailSubmission;
using Microsoft.Exchange.Rpc.OfflineRms;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport.Admin.IPFiltering;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Delivery;
using Microsoft.Exchange.Transport.Extensibility;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Logging.WorkloadManagement;
using Microsoft.Exchange.Transport.MessageResubmission;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.Pickup;
using Microsoft.Exchange.Transport.QueueViewer;
using Microsoft.Exchange.Transport.RecipientAPI;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.Win32;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class Components
	{
		static Components()
		{
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(Components.FaultInjectionCallback));
		}

		public Components(string hostServiceName, bool stateManagementEnabled)
		{
			this.hostServiceName = hostServiceName;
			this.stateManagementEnabled = stateManagementEnabled;
		}

		public static event EventHandler ConfigChanged;

		public static object SyncRoot
		{
			get
			{
				return Components.syncRoot;
			}
		}

		public static IStoreDriver StoreDriver
		{
			get
			{
				if (Components.storeDriver == null)
				{
					throw new Components.ComponentNotAvailableException("StoreDriver");
				}
				return Components.storeDriver;
			}
			set
			{
				Components.storeDriver = value;
			}
		}

		public static IStoreDriverSubmission StoreDriverSubmission
		{
			get
			{
				Components.ThrowIfNullComponent("StoreDriverSubmission", Components.storeDriverSubmission);
				return Components.storeDriverSubmission;
			}
			internal set
			{
				Components.storeDriverSubmission = value;
			}
		}

		public static IStoreDriverDelivery StoreDriverDelivery
		{
			get
			{
				Components.ThrowIfNullComponent("StoreDriverDelivery", Components.storeDriverDelivery);
				return Components.storeDriverDelivery;
			}
			internal set
			{
				Components.storeDriverDelivery = value;
			}
		}

		public static IStartableTransportComponent StoreDriverLoaderComponent
		{
			get
			{
				Components.ThrowIfNullComponent("StoreDriverLoaderComponent", Components.storeDriverLoaderComponent);
				return Components.storeDriverLoaderComponent;
			}
			set
			{
				Components.storeDriverLoaderComponent = (Components.StoreDriverLoader)value;
			}
		}

		public static IStartableTransportComponent Aggregator
		{
			get
			{
				Components.ThrowIfNullComponent("Aggregator", Components.aggregator);
				return Components.aggregator;
			}
			set
			{
				Components.aggregator = value;
			}
		}

		public static TransportAppConfig TransportAppConfig
		{
			get
			{
				if (Components.transportAppConfig == null)
				{
					Components.transportAppConfig = TransportAppConfig.Load();
				}
				return Components.transportAppConfig;
			}
		}

		public static MessagingDatabaseComponent MessagingDatabase
		{
			get
			{
				Components.ThrowIfNullComponent("MessagingDatabase", Components.messagingDatabaseComponent);
				return Components.messagingDatabaseComponent;
			}
			set
			{
				Components.messagingDatabaseComponent = value;
			}
		}

		public static ISmtpInComponent SmtpInComponent
		{
			get
			{
				Components.ThrowIfNullComponent("SmtpInComponent", Components.smtpInComponent);
				return Components.smtpInComponent;
			}
			set
			{
				Components.smtpInComponent = value;
			}
		}

		public static SmtpOutConnectionHandler SmtpOutConnectionHandler
		{
			get
			{
				Components.ThrowIfNullComponent("SmtpOutConnectionHandler", Components.smtpOutConnectionHandler);
				return Components.smtpOutConnectionHandler;
			}
			set
			{
				Components.smtpOutConnectionHandler = value;
			}
		}

		public static PickupComponent PickupComponent
		{
			get
			{
				Components.ThrowIfNullComponent("PickupComponent", Components.pickupComponent);
				return Components.pickupComponent;
			}
			set
			{
				Components.pickupComponent = value;
			}
		}

		public static IBootLoader BootScanner
		{
			get
			{
				Components.ThrowIfNullComponent("BootScanner", Components.bootScanner);
				return Components.bootScanner;
			}
			set
			{
				Components.bootScanner = value;
			}
		}

		public static CategorizerComponent CategorizerComponent
		{
			get
			{
				Components.ThrowIfNullComponent("CategorizerComponent", Components.categorizerComponent);
				return Components.categorizerComponent;
			}
			set
			{
				Components.categorizerComponent = value;
				Components.categorizer = value;
			}
		}

		public static CategorizerAdapterComponent CategorizerAdapterComponent
		{
			get
			{
				Components.ThrowIfNullComponent("CategorizerAdapterComponent", Components.categorizerAdapterComponent);
				return Components.categorizerAdapterComponent;
			}
			set
			{
				Components.categorizerAdapterComponent = value;
			}
		}

		public static SchedulerAdapterComponent SchedulerAdapterComponent
		{
			get
			{
				Components.ThrowIfNullComponent("SchedulerAdapterComponent", Components.schedulerAdapterComponent);
				return Components.schedulerAdapterComponent;
			}
			set
			{
				Components.schedulerAdapterComponent = value;
			}
		}

		public static ICategorizer Categorizer
		{
			set
			{
				Components.categorizer = value;
			}
		}

		public static IRoutingComponent RoutingComponent
		{
			get
			{
				Components.ThrowIfNullComponent("RoutingComponent", Components.routingComponent);
				return Components.routingComponent;
			}
			set
			{
				Components.routingComponent = value;
			}
		}

		public static UnhealthyTargetFilterComponent UnhealthyTargetFilterComponent
		{
			get
			{
				Components.ThrowIfNullComponent("UnhealthyTargetFilterComponent", Components.unhealthyTargetFilterComponent);
				return Components.unhealthyTargetFilterComponent;
			}
			set
			{
				Components.unhealthyTargetFilterComponent = value;
			}
		}

		public static EnhancedDns EnhancedDns
		{
			get
			{
				Components.ThrowIfNullComponent("EnhancedDnsComponent", Components.enhancedDnsComponent);
				return Components.enhancedDnsComponent;
			}
			set
			{
				Components.enhancedDnsComponent = value;
			}
		}

		public static IProxyHubSelectorComponent ProxyHubSelectorComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ProxyHubSelectorComponent", Components.proxyHubSelectorComponent);
				return Components.proxyHubSelectorComponent;
			}
			set
			{
				Components.proxyHubSelectorComponent = value;
			}
		}

		public static ITransportConfiguration Configuration
		{
			get
			{
				if (Components.configurationComponent == null)
				{
					throw new Components.ComponentNotAvailableException("Configuration");
				}
				return Components.configurationComponent;
			}
			set
			{
				Components.configurationComponent = value;
			}
		}

		public static IAgentRuntime AgentComponent
		{
			get
			{
				Components.ThrowIfNullComponent("AgentComponent", Components.agentComponent);
				return Components.agentComponent;
			}
			set
			{
				Components.agentComponent = (AgentComponent)value;
			}
		}

		public static RemoteDeliveryComponent RemoteDeliveryComponent
		{
			get
			{
				Components.ThrowIfNullComponent("RemoteDeliveryComponent", Components.remoteDeliveryComponent);
				return Components.remoteDeliveryComponent;
			}
			set
			{
				Components.remoteDeliveryComponent = value;
			}
		}

		public static IQueueQuotaComponent QueueQuotaComponent
		{
			get
			{
				Components.ThrowIfNullComponent("QueueQuotaComponent", Components.queueQuotaComponent);
				return Components.queueQuotaComponent;
			}
			set
			{
				Components.queueQuotaComponent = value;
			}
		}

		public static IMeteringComponent Metering
		{
			get
			{
				Components.ThrowIfNullComponent("MeteringComponent", Components.meteringComponent);
				return Components.meteringComponent;
			}
			set
			{
				Components.meteringComponent = value;
			}
		}

		public static IProcessingQuotaComponent ProcessingQuotaComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ProcessingQuotaComponent", Components.processingQuotaComponent);
				return Components.processingQuotaComponent;
			}
			set
			{
				Components.processingQuotaComponent = value;
			}
		}

		public static DeliveryAgentConnectionHandler DeliveryAgentConnectionHandler
		{
			get
			{
				Components.ThrowIfNullComponent("DeliveryAgentConnectionHandler", Components.deliveryAgentConnectionHandler);
				return Components.deliveryAgentConnectionHandler;
			}
			set
			{
				Components.deliveryAgentConnectionHandler = value;
			}
		}

		public static int InstanceId
		{
			get
			{
				return Components.instanceId;
			}
		}

		public static Dns Dns
		{
			get
			{
				return Components.enhancedDnsComponent;
			}
		}

		public static bool IsActive
		{
			get
			{
				return Components.active;
			}
		}

		public static bool IsPaused
		{
			get
			{
				return Components.paused;
			}
		}

		public static bool ShuttingDown
		{
			get
			{
				return Components.busyExit == 1;
			}
		}

		public static bool IsDatabaseShuttingDown
		{
			get
			{
				return Components.databaseComponents.IsUnloading;
			}
		}

		public static bool ServiceControlled
		{
			get
			{
				return Components.serviceControlled;
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return Components.eventLogger;
			}
		}

		internal static ResourceManager ResourceManager
		{
			get
			{
				Components.ThrowIfNullComponent("ResourceManager", Components.resourceManagerComponent);
				return Components.ResourceManagerComponent.ResourceManager;
			}
		}

		internal static ResourceManagerComponent ResourceManagerComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ResourceManagerComponent", Components.resourceManagerComponent);
				return Components.resourceManagerComponent;
			}
			set
			{
				Components.resourceManagerComponent = value;
			}
		}

		internal static ResourceThrottlingComponent ResourceThrottlingComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ResourceThrottlingComponent", Components.resourceThrottlingComponent);
				return Components.resourceThrottlingComponent;
			}
			set
			{
				Components.resourceThrottlingComponent = value;
			}
		}

		internal static RmsClientManagerComponent RmsClientManagerComponent
		{
			get
			{
				Components.ThrowIfNullComponent("RmsClientManagerComponent", Components.rmsClientManagerComponent);
				return Components.rmsClientManagerComponent;
			}
			set
			{
				Components.rmsClientManagerComponent = value;
			}
		}

		internal static QueueManager QueueManager
		{
			get
			{
				Components.ThrowIfNullComponent("QueueManager", Components.queueManager);
				return Components.queueManager;
			}
			set
			{
				Components.queueManager = value;
			}
		}

		internal static IProcessingSchedulerComponent ProcessingSchedulerComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ProcessignSchedulerComponent", Components.processingSchedulerComponent);
				return Components.processingSchedulerComponent;
			}
			set
			{
				Components.processingSchedulerComponent = value;
			}
		}

		internal static IMessageDepotComponent MessageDepotComponent
		{
			get
			{
				Components.ThrowIfNullComponent("MessageDepotComponent", Components.messageDepotComponent);
				return Components.messageDepotComponent;
			}
			set
			{
				Components.messageDepotComponent = value;
			}
		}

		internal static IMessageDepotQueueViewerComponent MessageDepotQueueViewerComponent
		{
			get
			{
				Components.ThrowIfNullComponent("MessageDepotQueueViewerComponent", Components.messageDepotQueueViewerComponent);
				return Components.messageDepotQueueViewerComponent;
			}
			set
			{
				Components.messageDepotQueueViewerComponent = value;
			}
		}

		internal static DsnSchedulerComponent DsnSchedulerComponent
		{
			get
			{
				if (Components.dsnSchedulerComponent == null)
				{
					throw new Components.ComponentNotAvailableException("dsnSchedulerComponent");
				}
				return Components.dsnSchedulerComponent;
			}
			set
			{
				Components.dsnSchedulerComponent = value;
			}
		}

		internal static OrarGenerator OrarGenerator
		{
			get
			{
				Components.ThrowIfNullComponent("OrarGenerator", Components.orarGenerator);
				return Components.orarGenerator;
			}
			set
			{
				Components.orarGenerator = value;
			}
		}

		internal static DsnGenerator DsnGenerator
		{
			get
			{
				Components.ThrowIfNullComponent("DsnGenerator", Components.dsnGenerator);
				return Components.dsnGenerator;
			}
			set
			{
				Components.dsnGenerator = value;
			}
		}

		internal static ClassificationConfig ClassificationConfig
		{
			get
			{
				return Components.classificationConfig;
			}
		}

		internal static MessageResubmissionComponent MessageResubmissionComponent
		{
			get
			{
				Components.ThrowIfNullComponent("MessageResubmissionComponent", Components.messageResubmissionComponent);
				return Components.messageResubmissionComponent;
			}
			set
			{
				Components.messageResubmissionComponent = value;
			}
		}

		internal static ShadowRedundancyComponent ShadowRedundancyComponent
		{
			get
			{
				Components.ThrowIfNullComponent("ShadowRedundancyComponent", Components.shadowRedundancyComponent);
				return Components.shadowRedundancyComponent;
			}
			set
			{
				Components.shadowRedundancyComponent = value;
			}
		}

		internal static Components.LoggingComponent Logging
		{
			get
			{
				Components.ThrowIfNullComponent("LoggingComponent", Components.loggingComponent);
				return Components.loggingComponent;
			}
			set
			{
				Components.loggingComponent = value;
			}
		}

		internal static MessageThrottlingComponent MessageThrottlingComponent
		{
			get
			{
				Components.ThrowIfNullComponent("MessageThrottlingComponent", Components.messageThrottlingComponent);
				return Components.messageThrottlingComponent;
			}
			set
			{
				Components.messageThrottlingComponent = value;
			}
		}

		internal static PoisonMessage PoisonMessageComponent
		{
			get
			{
				Components.ThrowIfNullComponent("PoisonMessageComponent", Components.poisonMessageComponent);
				return Components.poisonMessageComponent;
			}
			set
			{
				Components.poisonMessageComponent = value;
			}
		}

		internal static IsMemberOfResolverComponent<RoutingAddress> TransportIsMemberOfResolverComponent
		{
			get
			{
				Components.ThrowIfNullComponent("TransportIsMemberOfResolverComponent", Components.transportIsMemberOfResolverComponent);
				return Components.transportIsMemberOfResolverComponent;
			}
			set
			{
				Components.transportIsMemberOfResolverComponent = value;
			}
		}

		internal static IsMemberOfResolverComponent<string> MailboxRulesIsMemberOfResolverComponent
		{
			get
			{
				Components.ThrowIfNullComponent("MailboxRulesIsMemberOfResolverComponent", Components.mailboxRulesIsMemberOfResolverComponent);
				return Components.mailboxRulesIsMemberOfResolverComponent;
			}
			set
			{
				Components.mailboxRulesIsMemberOfResolverComponent = value;
			}
		}

		internal static SystemCheckComponent SystemCheckComponent
		{
			get
			{
				Components.ThrowIfNullComponent("SystemCheckComponent", Components.systemCheckComponent);
				return Components.systemCheckComponent;
			}
			set
			{
				Components.systemCheckComponent = value;
			}
		}

		internal static bool IsBridgehead
		{
			get
			{
				return Components.Configuration.LocalServer.IsBridgehead;
			}
		}

		internal static bool IsMailboxProcess
		{
			get
			{
				return Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery;
			}
		}

		internal static CertificateComponent CertificateComponent
		{
			get
			{
				Components.ThrowIfNullComponent("CertificateComponent", Components.certificateComponent);
				return Components.certificateComponent;
			}
			set
			{
				Components.certificateComponent = value;
			}
		}

		private ServiceState ServiceState
		{
			get
			{
				ServiceState serviceState = ServiceState.Active;
				if (this.stateManagementEnabled)
				{
					serviceState = ServiceStateHelper.GetServiceState(Components.Configuration, this.hostServiceName);
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_RetrieveServiceState, null, new object[]
					{
						this.hostServiceName,
						serviceState
					});
				}
				return serviceState;
			}
		}

		public static IPerfCountersLoader PerfCountersLoaderComponent
		{
			get
			{
				if (Components.perfCountersLoader == null)
				{
					throw new Components.ComponentNotAvailableException("perfCountersLoader");
				}
				return Components.perfCountersLoader;
			}
			set
			{
				Components.perfCountersLoader = value;
			}
		}

		private static void ThrowIfNullComponent(string componentName, ITransportComponent transportComponent)
		{
			if (transportComponent == null)
			{
				throw new Components.ComponentNotAvailableException(componentName);
			}
		}

		public static bool TryGetStoreDriver(out IStoreDriver component)
		{
			component = Components.storeDriver;
			return Components.storeDriver != null;
		}

		public static bool TryGetStoreDriverDelivery(out IStoreDriverDelivery component)
		{
			component = Components.storeDriverDelivery;
			return Components.storeDriverDelivery != null;
		}

		public static bool TryGetStoreDriverSubmission(out IStoreDriverSubmission component)
		{
			component = Components.storeDriverSubmission;
			return Components.storeDriverSubmission != null;
		}

		public static bool TryGetRemoteDeliveryComponent(out RemoteDeliveryComponent component)
		{
			component = Components.remoteDeliveryComponent;
			return Components.remoteDeliveryComponent != null;
		}

		public static bool TryGetTransportIsMemberOfResolverComponent(out IsMemberOfResolverComponent<RoutingAddress> component)
		{
			component = Components.transportIsMemberOfResolverComponent;
			return Components.transportIsMemberOfResolverComponent != null;
		}

		public static bool TryGetSmtpInComponent(out ISmtpInComponent component)
		{
			component = Components.smtpInComponent;
			return Components.smtpInComponent != null;
		}

		public static bool TryGetAggregator(out IStartableTransportComponent component)
		{
			component = Components.aggregator;
			return Components.aggregator != null;
		}

		public static bool TryGetConfigurationComponent(out ITransportConfiguration component)
		{
			component = Components.configurationComponent;
			return Components.configurationComponent != null;
		}

		public static bool TryGetMessagingDatabaseComponent(out MessagingDatabaseComponent component)
		{
			component = Components.messagingDatabaseComponent;
			return Components.messagingDatabaseComponent != null;
		}

		public static bool TryGetPickupComponent(out PickupComponent component)
		{
			component = Components.pickupComponent;
			return Components.pickupComponent != null;
		}

		public static bool TryGetStoreDriverLoaderComponent(out IStartableTransportComponent component)
		{
			component = Components.storeDriverLoaderComponent;
			return Components.storeDriverLoaderComponent != null;
		}

		public static bool TryGetBootScanner(out IStartableTransportComponent component)
		{
			component = Components.bootScanner;
			return Components.bootScanner != null;
		}

		public static bool TryGetShadowRedundancyComponent(out ShadowRedundancyComponent component)
		{
			component = Components.shadowRedundancyComponent;
			return Components.shadowRedundancyComponent != null;
		}

		public static bool TryGetMessageResubmissionComponent(out MessageResubmissionComponent component)
		{
			component = Components.messageResubmissionComponent;
			return Components.messageResubmissionComponent != null;
		}

		public static bool TryGetEnhancedDnsComponent(out EnhancedDns component)
		{
			component = Components.enhancedDnsComponent;
			return component != null;
		}

		public static bool TryGetMeteringComponent(out IMeteringComponent component)
		{
			component = Components.meteringComponent;
			return Components.meteringComponent != null;
		}

		public static bool TryGetQueueQuotaComponent(out IQueueQuotaComponent component)
		{
			component = Components.queueQuotaComponent;
			return Components.queueQuotaComponent != null;
		}

		public static bool TryGetProcessingQuotaComponent(out IProcessingQuotaComponent component)
		{
			component = Components.processingQuotaComponent;
			return Components.processingQuotaComponent != null;
		}

		public static void SetRootComponent(SequentialTransportComponent rootComponent)
		{
			Components.rootComponent = rootComponent;
		}

		public static void SetDatabaseComponents(SequentialTransportComponent databaseComponents)
		{
			Components.databaseComponents = databaseComponents;
		}

		public static bool TryLoadTransportAppConfig(out string exceptionMessage)
		{
			exceptionMessage = null;
			bool result;
			try
			{
				Components.transportAppConfig = TransportAppConfig.Load();
				result = true;
			}
			catch (ConfigurationErrorsException ex)
			{
				result = false;
				exceptionMessage = ex.Message;
				ExTraceGlobals.GeneralTracer.TraceError(0L, string.Format("Failed to load configuration file. {0}.", ex.Message));
			}
			return result;
		}

		public static void StopService(string reason, bool canRetry, bool retryAlways, bool failServiceWithException)
		{
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Components StopService");
			Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_StopService, null, new object[]
			{
				reason
			});
			if (Components.stopServiceHandler != null)
			{
				Components.stopServiceHandler(reason, canRetry, retryAlways, failServiceWithException);
			}
		}

		public static string OnUnhandledException(Exception e)
		{
			if (Components.rootComponent != null)
			{
				return Components.rootComponent.OnUnhandledException(e);
			}
			return null;
		}

		public static string CurrentStateRepresentation()
		{
			string result;
			if (Components.ShuttingDown)
			{
				result = string.Format("Transport is shutting down: Following is the current state of the components: {0}", Components.rootComponent.CurrentState);
			}
			else
			{
				result = string.Format("Transport: Active = {0}, Paused={1}", Components.IsActive, Components.IsPaused);
			}
			return result;
		}

		public static void HandleConnection(Socket clientConnection)
		{
			if (Components.smtpInComponent != null)
			{
				Components.SmtpInComponent.HandleConnection(clientConnection);
				return;
			}
			clientConnection.Close();
		}

		public void Start(Components.StopServiceHandler stopServiceHandler, bool passiveRole, bool serviceControlled, bool selfListening, bool paused)
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Start called");
			Components.serviceControlled = serviceControlled;
			if (Components.smtpInComponent != null)
			{
				Components.smtpInComponent.SelfListening = selfListening;
			}
			Components.stopServiceHandler = stopServiceHandler;
			Components.paused = paused;
			int availableMemoryInMBytes = Components.GetAvailableMemoryInMBytes();
			this.RegisterForDeletedObjectNotificationsInDatacenter();
			if (availableMemoryInMBytes < Components.TransportAppConfig.WorkerProcess.FreeMemoryRequiredToStartInMbytes)
			{
				string text = string.Format("TotalMemoryAvailableInMB:{0} MemoryRequiredToStartServiceInMB:{1} {2}", availableMemoryInMBytes, Components.TransportAppConfig.WorkerProcess.FreeMemoryRequiredToStartInMbytes, Components.GetProcessMemoryUsage());
				ExWatson.SendGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.012", Assembly.GetExecutingAssembly().GetName().Name, "OutOfMemoryException", "Startup", "Startup", "Component.Start", text);
				Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NotEnoughMemoryToStartService, null, new object[]
				{
					text
				});
				Components.StopService(string.Format("Memory Available {0}MB less than required {1}MB", availableMemoryInMBytes, Components.TransportAppConfig.WorkerProcess.FreeMemoryRequiredToStartInMbytes), true, true, false);
			}
			if (!passiveRole)
			{
				this.ProtectedActivate();
			}
		}

		public void Stop()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug<int>((long)this.GetHashCode(), "Stop {0}: initiated", Environment.TickCount);
			if (Interlocked.Exchange(ref Components.busyExit, 1) != 0)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Already working on exit procedure");
				return;
			}
			DisposeTracker.Suppressed = true;
			lock (Components.SyncRoot)
			{
				if (Components.IsActive)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<int>((long)this.GetHashCode(), "Stop {0}: signal background", Environment.TickCount);
					if (Components.bootScanner != null)
					{
						Components.BootScanner.Pause();
					}
					TransportFacades.Stop();
					Components.rootComponent.Stop();
					Components.rootComponent.Unload();
				}
			}
		}

		public void Pause()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Pause called");
			lock (Components.SyncRoot)
			{
				Components.paused = true;
				if (Components.resourceManagerComponent != null)
				{
					Components.ResourceManagerComponent.ResourceManager.RefreshComponentsState();
				}
			}
		}

		public void Continue()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Continue called");
			lock (Components.SyncRoot)
			{
				Components.paused = false;
				if (Components.IsActive && !Components.ShuttingDown)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Initiate Continue");
					if (Components.resourceManagerComponent != null)
					{
						Components.ResourceManagerComponent.ResourceManager.RefreshComponentsState();
					}
				}
			}
		}

		public void Retire()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Retire called");
			if (Interlocked.Exchange(ref Components.busyExit, 1) != 0)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Already working on exit procedure");
				return;
			}
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Exit procedure started");
			lock (Components.SyncRoot)
			{
				if (Components.IsActive)
				{
					if (Components.bootScanner != null)
					{
						Components.BootScanner.Pause();
					}
					if (Components.smtpInComponent != null)
					{
						Components.SmtpInComponent.RejectCommands();
					}
					if (Components.pickupComponent != null)
					{
						Components.PickupComponent.Stop();
					}
					if (Components.storeDriver != null)
					{
						Components.StoreDriver.Retire();
					}
					if (Components.storeDriverDelivery != null)
					{
						Components.StoreDriverDelivery.Retire();
					}
					if (Components.storeDriverSubmission != null)
					{
						Components.StoreDriverSubmission.Retire();
					}
					if (Components.categorizerComponent != null)
					{
						Components.CategorizerComponent.Retire();
					}
					Components.retireTimer = new Timer(new TimerCallback(this.RetireCompleted), null, 15000, -1);
				}
				else
				{
					ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Retire signalled, components are not activated.");
					Environment.Exit(0);
				}
			}
		}

		public void ProtectedActivate()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(new ADOperation(this.Activate), 0);
			if (!adoperationResult.Succeeded)
			{
				string text = "Failed to read configuration. The Transport Service will be stopped.";
				ExTraceGlobals.ConfigurationTracer.TraceError(0L, text);
				Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ActivationFailed, null, new object[]
				{
					adoperationResult.Exception.ToString()
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Warning, false);
				Components.StopService(Strings.ActivationFailed, adoperationResult.ErrorCode == ADOperationErrorCode.RetryableError, false, false);
			}
		}

		public void Activate()
		{
			ICategorizerComponentFacade categorizerComponentFacade2;
			if (Components.categorizerComponent == null)
			{
				ICategorizerComponentFacade categorizerComponentFacade = new NullCategorizer();
				categorizerComponentFacade2 = categorizerComponentFacade;
			}
			else
			{
				categorizerComponentFacade2 = Components.CategorizerComponent;
			}
			ICategorizerComponentFacade categorizerComponentFacade3 = categorizerComponentFacade2;
			TransportFacades.Initialize(Components.Dns, categorizerComponentFacade3, Components.shadowRedundancyComponent, Components.ConfigChanged, new NewMailItemDelegate(TransportMailItem.NewAgentMailItem), new EnsureSecurityAttributesDelegate(MultilevelAuth.EnsureSecurityAttributesByAgent), new TrackReceiveByAgentDelegate(MessageTrackingLog.TrackReceiveByAgent), new TrackRecipientAddByAgentDelegate(MessageTrackingLog.TrackRecipientAddByAgent), new ReadHistoryFromMailItemByAgentDelegate(History.ReadFromMailItemByAgent), new ReadHistoryFromRecipientByAgentDelegate(History.ReadFromRecipientByAgent), new CreateAndSubmitApprovalInitiationForTransportRulesDelegate(ApprovalInitiation.CreateAndSubmitApprovalInitiationForTransportRules));
			lock (Components.SyncRoot)
			{
				Stopwatch stopwatch = new Stopwatch();
				Stopwatch stopwatch2 = new Stopwatch();
				TimerCallback callback = delegate(object state)
				{
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ActivationSlow, null, new object[]
					{
						Components.rootComponent.CurrentState
					});
				};
				Timer timer = new Timer(callback, null, Components.maxActivateSecondsToLog, TimeSpan.FromMilliseconds(-1.0));
				try
				{
					Components.SetLoadTimeDependencies();
					stopwatch2.Start();
					Components.rootComponent.Load();
					stopwatch2.Stop();
					Components.SetRunTimeDependencies();
					stopwatch.Start();
					Components.rootComponent.Start(true, this.ServiceState);
					stopwatch.Stop();
					Components.active = true;
					if (Components.categorizerComponent != null)
					{
						Components.CategorizerComponent.DataAvail();
					}
					if (!Components.IsPaused)
					{
						this.Continue();
					}
				}
				catch (TransportComponentLoadFailedException ex)
				{
					ExTraceGlobals.ConfigurationTracer.TraceError(0L, "Failed to load components. The Transport Service will be stopped.");
					bool canRetry = Components.HasTransientException(ex);
					Components.StopService(ex.Message, canRetry, false, false);
				}
				finally
				{
					timer.Dispose();
					if (stopwatch2.Elapsed + stopwatch.Elapsed > Components.maxActivateSecondsToLog)
					{
						Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ActivationTiming, null, new object[]
						{
							stopwatch2.Elapsed.ToString(),
							stopwatch.Elapsed.ToString(),
							string.Format("\n{0}\n{1}", Components.rootComponent.LoadTimings, Components.rootComponent.StartTimings)
						});
					}
				}
			}
		}

		public void ConfigUpdate()
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Components ConfigUpdate");
			lock (Components.SyncRoot)
			{
				if (!Components.ShuttingDown && Components.IsActive)
				{
					EventHandler configChanged = Components.ConfigChanged;
					if (configChanged != null)
					{
						configChanged(null, null);
					}
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ConfigUpdateOccurred, null, new object[0]);
				}
			}
		}

		internal static void InstantiateComponentsForTest()
		{
			TransportAppConfig.IsMemberOfResolverConfiguration transportIsMemberOfResolverConfig = Components.TransportAppConfig.TransportIsMemberOfResolverConfig;
			TransportAppConfig.IsMemberOfResolverConfiguration mailboxRulesIsMemberOfResolverConfig = Components.TransportAppConfig.MailboxRulesIsMemberOfResolverConfig;
			IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver adAdapter = new IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver(transportIsMemberOfResolverConfig.DisableDynamicGroups);
			IsMemberOfResolverADAdapter<string>.LegacyDNResolver adAdapter2 = new IsMemberOfResolverADAdapter<string>.LegacyDNResolver(mailboxRulesIsMemberOfResolverConfig.DisableDynamicGroups);
			StorageFactory.SchemaToUse = StorageFactory.Schema.NullSchema;
			Components.agentComponent = (Components.agentComponent ?? new AgentComponent());
			Components.messagingDatabaseComponent = (Components.messagingDatabaseComponent ?? new MessagingDatabaseComponent());
			Components.bootScanner = (Components.bootScanner ?? Components.messagingDatabaseComponent.CreateBootScanner());
			Components.routingComponent = (Components.routingComponent ?? new RoutingComponent());
			Components.unhealthyTargetFilterComponent = (Components.unhealthyTargetFilterComponent ?? new UnhealthyTargetFilterComponent());
			Components.enhancedDnsComponent = (Components.enhancedDnsComponent ?? new EnhancedDns());
			Components.categorizerComponent = (Components.categorizerComponent ?? new CategorizerComponent());
			Components.proxyHubSelectorComponent = (Components.proxyHubSelectorComponent ?? new ProxyHubSelectorComponent());
			Components.certificateComponent = (Components.certificateComponent ?? new CertificateComponent());
			Components.configurationComponent = (Components.configurationComponent ?? new ConfigurationComponent());
			Components.deliveryAgentConnectionHandler = (Components.deliveryAgentConnectionHandler ?? new DeliveryAgentConnectionHandler());
			Components.dsnGenerator = (Components.dsnGenerator ?? new DsnGenerator());
			Components.messageResubmissionComponent = (Components.messageResubmissionComponent ?? new MessageResubmissionComponent());
			Components.mailboxRulesIsMemberOfResolverComponent = (Components.mailboxRulesIsMemberOfResolverComponent ?? new IsMemberOfResolverComponent<string>("MailboxRules", mailboxRulesIsMemberOfResolverConfig, adAdapter2));
			Components.messageThrottlingComponent = (Components.messageThrottlingComponent ?? new MessageThrottlingComponent());
			Components.orarGenerator = (Components.orarGenerator ?? new OrarGenerator());
			Components.pickupComponent = (Components.pickupComponent ?? new PickupComponent(new Components.TransportPickupSubmissionHandler()));
			Components.queueManager = (Components.queueManager ?? new QueueManager());
			Components.messageDepotComponent = (Components.messageDepotComponent ?? new MessageDepotComponent());
			Components.messageDepotQueueViewerComponent = (Components.messageDepotQueueViewerComponent ?? new MessageDepotQueueViewerComponent());
			Components.dsnSchedulerComponent = (Components.dsnSchedulerComponent ?? new DsnSchedulerComponent());
			Components.remoteDeliveryComponent = (Components.remoteDeliveryComponent ?? new RemoteDeliveryComponent());
			Components.resourceManagerComponent = (Components.resourceManagerComponent ?? new ResourceManagerComponent(ResourceManagerResources.All));
			Components.rmsClientManagerComponent = (Components.rmsClientManagerComponent ?? new RmsClientManagerComponent());
			Components.shadowRedundancyComponent = (Components.shadowRedundancyComponent ?? new ShadowRedundancyComponent());
			Components.smtpInComponent = (Components.smtpInComponent ?? new SmtpInComponent(false));
			Components.smtpOutConnectionHandler = (Components.smtpOutConnectionHandler ?? new SmtpOutConnectionHandler());
			Components.storeDriverLoaderComponent = (Components.storeDriverLoaderComponent ?? new Components.StoreDriverLoader());
			Components.systemCheckComponent = (Components.systemCheckComponent ?? new SystemCheckComponent());
			Components.transportIsMemberOfResolverComponent = (Components.transportIsMemberOfResolverComponent ?? new IsMemberOfResolverComponent<RoutingAddress>("Transport", transportIsMemberOfResolverConfig, adAdapter));
			Components.poisonMessageComponent = (Components.poisonMessageComponent ?? new PoisonMessage());
			Components.loggingComponent = (Components.loggingComponent ?? new Components.LoggingComponent(false, false, false, false, false));
			Components.queueQuotaComponent = (Components.queueQuotaComponent ?? new QueueQuotaComponent());
			Components.meteringComponent = (Components.meteringComponent ?? new MeteringComponent());
			Components.processingQuotaComponent = (Components.processingQuotaComponent ?? new ProcessingQuotaComponent());
		}

		private static void SetLoadTimeDependencies()
		{
			if (Components.systemCheckComponent != null)
			{
				Components.SystemCheckComponent.SetLoadTimeDependencies(new SystemCheckConfig(null), Components.TransportAppConfig, Components.Configuration);
			}
			if (Components.smtpInComponent != null)
			{
				Components.SmtpInComponent.SetLoadTimeDependencies(Components.TransportAppConfig, Components.Configuration);
			}
			if (Components.smtpOutConnectionHandler != null)
			{
				Components.SmtpOutConnectionHandler.SetLoadTimeDependencies(Components.RoutingComponent.MailRouter, Components.TransportAppConfig, Components.Configuration, Components.SmtpInComponent, Components.Logging);
			}
			if (Components.routingComponent != null)
			{
				Components.routingComponent.SetLoadTimeDependencies(Components.TransportAppConfig, Components.Configuration);
			}
			if (Components.proxyHubSelectorComponent != null)
			{
				Components.proxyHubSelectorComponent.SetLoadTimeDependencies(Components.RoutingComponent.MailRouter, Components.Configuration);
			}
			if (Components.transportAppConfig != null)
			{
				Components.TransportAppConfig.QueueDatabase.SetLoadTimeDependencies(Components.Configuration);
			}
			if (Components.messagingDatabaseComponent != null)
			{
				Components.messagingDatabaseComponent.SetLoadTimeDependencies(Components.TransportAppConfig.QueueDatabase);
			}
			if (Components.shadowRedundancyComponent != null)
			{
				Components.shadowRedundancyComponent.SetLoadTimeDependencies(Components.MessagingDatabase.Database, Components.bootScanner);
			}
			if (Components.messageDepotComponent != null)
			{
				Components.messageDepotComponent.SetLoadTimeDependencies(new MessageDepotConfig(null));
			}
			if (Components.messageDepotQueueViewerComponent != null)
			{
				Components.messageDepotQueueViewerComponent.SetLoadTimeDependencies(Components.messageDepotComponent, Components.TransportAppConfig.QueueConfiguration);
			}
			if (Components.processingSchedulerComponent != null)
			{
				Components.processingSchedulerComponent.SetLoadTimeDependencies(Components.Configuration, Components.messageDepotComponent, Components.categorizerAdapterComponent, Components.messagingDatabaseComponent);
			}
			if (Components.bootScanner != null)
			{
				Components.bootScanner.SetLoadTimeDependencies(Components.EventLogger, Components.MessagingDatabase.Database, Components.ShadowRedundancyComponent, Components.PoisonMessageComponent, Components.CategorizerComponent, Components.QueueManager, Components.TransportAppConfig.BootLoader);
			}
			if (Components.messageResubmissionComponent != null && Components.messagingDatabaseComponent != null && Components.shadowRedundancyComponent != null)
			{
				Components.messageResubmissionComponent.SetLoadTimeDependencies(Components.TransportAppConfig.MessageResubmission, Components.messagingDatabaseComponent.Database);
			}
			if (Components.processingQuotaComponent != null || Components.messageDepotComponent != null)
			{
				Components.categorizer.SetLoadTimeDependencies(Components.ProcessingQuotaComponent, Components.messageDepotComponent);
				Components.processingQuotaComponent.SetLoadTimeDependencies(Components.TransportAppConfig.ProcessingQuota);
			}
			if (Components.dsnSchedulerComponent != null)
			{
				Components.dsnSchedulerComponent.SetLoadTimeDependencies(Components.messageDepotComponent, Components.DsnGenerator, Components.orarGenerator, new MessageTrackingLogWrapper(), Components.configurationComponent);
			}
			if (Components.meteringComponent != null)
			{
				Components.meteringComponent.SetLoadtimeDependencies(new MeteringConfig(), ExTraceGlobals.QueuingTracer);
			}
		}

		private static void SetRunTimeDependencies()
		{
			ICategorizer categorizer = (Components.categorizer != null) ? Components.categorizer : new NullCategorizer();
			if (Components.queueQuotaComponent != null)
			{
				ICountTracker<MeteredEntity, MeteredCount> metering = null;
				IMeteringComponent meteringComponent;
				if (Components.TryGetMeteringComponent(out meteringComponent))
				{
					metering = meteringComponent.Metering;
				}
				QueueQuotaConfig queueQuotaConfig = new QueueQuotaConfig(Components.TransportAppConfig.FlowControlLog, Components.TransportAppConfig.QueueConfiguration);
				Components.queueQuotaComponent.SetRunTimeDependencies(queueQuotaConfig, Components.loggingComponent.FlowControlLog, new QueueQuotaComponentPerformanceCountersWrapper(queueQuotaConfig.RecentPerfCounterTrackingInterval, queueQuotaConfig.RecentPerfCounterTrackingBucketSize), Components.processingQuotaComponent, SubmitMessageQueue.Instance, Components.RemoteDeliveryComponent, metering);
			}
			if (Components.smtpInComponent != null)
			{
				Components.SmtpInComponent.SetRunTimeDependencies(Components.AgentComponent, Components.RoutingComponent.MailRouter, (Components.proxyHubSelectorComponent != null) ? Components.proxyHubSelectorComponent.ProxyHubSelector : null, Components.EnhancedDns, categorizer, Components.CertificateComponent.Cache, Components.CertificateComponent.Validator, Components.TransportIsMemberOfResolverComponent.IsMemberOfResolver, Components.MessagingDatabase.Database, Components.MessageThrottlingComponent.MessageThrottlingManager, (Components.shadowRedundancyComponent != null) ? Components.shadowRedundancyComponent.ShadowRedundancyManager : null, Components.SmtpOutConnectionHandler, Components.queueQuotaComponent);
			}
			if (Components.smtpOutConnectionHandler != null)
			{
				Components.SmtpOutConnectionHandler.SetRunTimeDependencies(Components.EnhancedDns, Components.UnhealthyTargetFilterComponent, Components.CertificateComponent.Cache, Components.CertificateComponent.Validator, (Components.shadowRedundancyComponent != null) ? Components.shadowRedundancyComponent.ShadowRedundancyManager : null);
			}
			if (Components.routingComponent != null)
			{
				Components.routingComponent.SetRunTimeDependencies(Components.shadowRedundancyComponent, Components.UnhealthyTargetFilterComponent, Components.categorizerComponent);
			}
			if (Components.enhancedDnsComponent != null)
			{
				Components.enhancedDnsComponent.SetRunTimeDependencies(Components.RoutingComponent.MailRouter);
			}
			if (Components.messageResubmissionComponent != null && Components.shadowRedundancyComponent != null && Components.shadowRedundancyComponent.ShadowRedundancyManager != null)
			{
				Components.messageResubmissionComponent.SetRunTimeDependencies(Components.shadowRedundancyComponent.ShadowRedundancyManager.PrimaryServerInfos);
			}
		}

		private static Exception FaultInjectionCallback(string exceptionType)
		{
			Exception ex = null;
			LocalizedString value = new LocalizedString("Fault injection.");
			if (exceptionType != null && exceptionType.Equals("System.ArgumentException"))
			{
				ex = new ArgumentException("Test Fault Exception");
			}
			if (ex == null)
			{
				ex = new Exception(value);
			}
			return ex;
		}

		private static bool HasTransientException(Exception exception)
		{
			while (exception != null)
			{
				if (exception is TransientException)
				{
					return true;
				}
				exception = exception.InnerException;
			}
			return false;
		}

		private static void LoadStoreDriverAssembly()
		{
			AssemblyName name = Assembly.GetExecutingAssembly().GetName();
			string assemblyString = name.FullName.Replace(name.Name, "Microsoft.Exchange.StoreDriver");
			Components.storeDriverAssembly = Assembly.Load(assemblyString);
		}

		public static string GetProcessMemoryUsage()
		{
			try
			{
				IEnumerable<string> instanceNames = new PerformanceCounterCategory("Process").GetInstanceNames();
				List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>();
				foreach (string text in instanceNames)
				{
					using (PerformanceCounter performanceCounter = new PerformanceCounter("Process", "Working Set", text, true))
					{
						list.Add(new KeyValuePair<string, float>(text, performanceCounter.NextValue()));
					}
				}
				return string.Format("TopProcesses @{0} :\n {1}", DateTime.UtcNow, string.Join("\n", from t in (from k in list
				orderby k.Value descending
				select k).Take(21)
				select string.Format("{0}:{1:N0}", t.Key, t.Value)));
			}
			catch (InvalidOperationException)
			{
			}
			catch (Win32Exception)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return "Unable to query performance counters to get the top processes";
		}

		private static int GetAvailableMemoryInMBytes()
		{
			try
			{
				NativeMethods.MemoryStatusEx memoryStatusEx;
				if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
				{
					ulong num = memoryStatusEx.AvailPhys / 1048576UL;
					return (int)num;
				}
			}
			catch (Win32Exception)
			{
			}
			return int.MaxValue;
		}

		private void RetireCompleted(object obj)
		{
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Retire completed");
			if (Components.smtpInComponent != null)
			{
				Components.SmtpInComponent.RejectSubmits();
			}
			Components.rootComponent.Stop();
			Components.rootComponent.Unload();
			ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Exit");
			Environment.Exit(0);
		}

		private void RegisterForDeletedObjectNotificationsInDatacenter()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(new ADOperation(TransportADNotificationAdapter.Instance.RegisterForEdgeTransportEvents), 0);
			if (!adoperationResult.Succeeded)
			{
				string text = "Failed to read configuration. The Transport Service will be stopped.";
				ExTraceGlobals.ConfigurationTracer.TraceError(0L, text);
				Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ActivationFailed, null, new object[]
				{
					adoperationResult.Exception.ToString()
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Warning, false);
				Components.StopService(Strings.ActivationFailed, adoperationResult.ErrorCode == ADOperationErrorCode.RetryableError, false, false);
			}
		}

		public const string SmtpPrincipalName = "SMTP";

		private const int RetirePeriodSec = 15;

		private static TransportAppConfig transportAppConfig;

		private static TimeSpan maxActivateSecondsToLog = TimeSpan.FromSeconds(15.0);

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());

		private static SequentialTransportComponent rootComponent;

		private static SequentialTransportComponent databaseComponents;

		private static SmtpOutConnectionHandler smtpOutConnectionHandler;

		private static ISmtpInComponent smtpInComponent;

		private static MessagingDatabaseComponent messagingDatabaseComponent;

		private static CategorizerComponent categorizerComponent;

		private static CategorizerAdapterComponent categorizerAdapterComponent;

		private static SchedulerAdapterComponent schedulerAdapterComponent;

		private static Components.LoggingComponent loggingComponent;

		private static ICategorizer categorizer;

		private static IRoutingComponent routingComponent;

		private static UnhealthyTargetFilterComponent unhealthyTargetFilterComponent;

		private static EnhancedDns enhancedDnsComponent;

		private static IProxyHubSelectorComponent proxyHubSelectorComponent;

		private static ITransportConfiguration configurationComponent;

		private static AgentComponent agentComponent;

		private static OrarGenerator orarGenerator;

		private static DsnGenerator dsnGenerator;

		private static ClassificationConfig classificationConfig = new ClassificationConfig();

		private static PickupComponent pickupComponent;

		private static ResourceManagerComponent resourceManagerComponent;

		private static ResourceThrottlingComponent resourceThrottlingComponent;

		private static RmsClientManagerComponent rmsClientManagerComponent;

		private static RemoteDeliveryComponent remoteDeliveryComponent;

		private static DeliveryAgentConnectionHandler deliveryAgentConnectionHandler;

		private static QueueManager queueManager;

		private static IProcessingSchedulerComponent processingSchedulerComponent;

		private static IMessageDepotComponent messageDepotComponent;

		private static IMessageDepotQueueViewerComponent messageDepotQueueViewerComponent;

		private static DsnSchedulerComponent dsnSchedulerComponent;

		private static MessageResubmissionComponent messageResubmissionComponent;

		private static ShadowRedundancyComponent shadowRedundancyComponent;

		private static MessageThrottlingComponent messageThrottlingComponent;

		private static PoisonMessage poisonMessageComponent;

		private static IsMemberOfResolverComponent<RoutingAddress> transportIsMemberOfResolverComponent;

		private static IsMemberOfResolverComponent<string> mailboxRulesIsMemberOfResolverComponent;

		private static IStoreDriver storeDriver;

		private static IStoreDriverDelivery storeDriverDelivery;

		private static IStoreDriverSubmission storeDriverSubmission;

		private static Components.StoreDriverLoader storeDriverLoaderComponent;

		private static IStartableTransportComponent aggregator;

		private static IQueueQuotaComponent queueQuotaComponent;

		private static IMeteringComponent meteringComponent;

		private static IProcessingQuotaComponent processingQuotaComponent;

		private static Assembly storeDriverAssembly;

		private static IBootLoader bootScanner;

		private static IPFilterAdminServer adminIPFilterServer;

		private static QueueViewerServer queueViewerServer;

		private static RpcServerWrapper offlineRmsServer;

		private static MessageResubmissionRpcServerImpl resubmissionServer;

		private static CertificateComponent certificateComponent;

		private static IPerfCountersLoader perfCountersLoader;

		private static SystemCheckComponent systemCheckComponent;

		private static int instanceId = Process.GetCurrentProcess().Id;

		private static int busyExit;

		private static bool active;

		private static bool paused;

		private static Timer retireTimer;

		private static bool serviceControlled;

		private static Components.StopServiceHandler stopServiceHandler;

		private static object syncRoot = new object();

		private readonly string hostServiceName;

		private readonly bool stateManagementEnabled;

		public delegate void StopServiceHandler(string reason, bool canRetry, bool retryAlways, bool failServiceWithException);

		internal class ServicePrincipalNameRegistrar : ITransportComponent
		{
			public void Load()
			{
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (!current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid) && !current.User.IsWellKnown(WellKnownSidType.LocalSystemSid))
					{
						return;
					}
				}
				bool flag;
				int num = LsaPolicy.GetDomainMembershipStatus(out flag);
				if (num != 0)
				{
					flag = true;
				}
				if (flag)
				{
					num = ServicePrincipalName.RegisterServiceClass("SmtpSvc");
					if (num != 0)
					{
						Win32Exception ex = new Win32Exception(num);
						Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SpnRegisterFailure, null, new object[]
						{
							"SmtpSvc",
							ex.Message
						});
					}
					num = ServicePrincipalName.RegisterServiceClass("SMTP");
					if (num != 0)
					{
						Win32Exception ex2 = new Win32Exception(num);
						Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SpnRegisterFailure, null, new object[]
						{
							"SMTP",
							ex2.Message
						});
					}
					return;
				}
			}

			public void Unload()
			{
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}
		}

		internal class RpcServerComponent : IStartableTransportComponent, ITransportComponent
		{
			public string CurrentState
			{
				get
				{
					return null;
				}
			}

			public void Load()
			{
			}

			public void Start(bool initiallyPaused, ServiceState targetRunningState)
			{
				bool isLocalOnly = !Components.IsBridgehead;
				Components.adminIPFilterServer = (IPFilterAdminServer)RpcServerBase.RegisterServer(typeof(IPFilterAdminServer), Components.Configuration.LocalServer.TransportServerSecurity, 131112, isLocalOnly);
				Components.queueViewerServer = (QueueViewerServer)RpcServerBase.RegisterServer(typeof(QueueViewerServer), Components.Configuration.LocalServer.TransportServerSecurity, 131220, isLocalOnly);
				if (Components.IsBridgehead)
				{
					Components.offlineRmsServer = (RpcServerWrapper)RpcServerBase.RegisterServer(typeof(RpcServerWrapper), Components.Configuration.LocalServer.TransportServerSecurity, 131220, false);
					ObjectSecurity localSystemSecurity = this.GetLocalSystemSecurity();
					ObjectSecurity serverAdminSecurity = this.GetServerAdminSecurity();
					Components.resubmissionServer = (MessageResubmissionRpcServerImpl)RpcServerBase.RegisterServer(typeof(MessageResubmissionRpcServerImpl), serverAdminSecurity, 1, false, (uint)Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions);
					Components.resubmissionServer.LocalSystemSecurityDescriptor = localSystemSecurity;
				}
				Components.Configuration.LocalServerChanged += Components.RpcServerComponent.ConfigUpdate;
			}

			public void Stop()
			{
				Components.Configuration.LocalServerChanged -= Components.RpcServerComponent.ConfigUpdate;
				RpcServerBase.StopServer(IPFilterRpcServer.RpcIntfHandle);
				RpcServerBase.StopServer(QueueViewerRpcServer.RpcIntfHandle);
				if (Components.IsBridgehead)
				{
					RpcServerBase.StopServer(OfflineRmsRpcServer.RpcIntfHandle);
					RpcServerBase.StopServer(MailSubmissionServiceRpcServer.RpcIntfHandle);
				}
			}

			public void Pause()
			{
			}

			public void Continue()
			{
			}

			public void Unload()
			{
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}

			private static void ConfigUpdate(TransportServerConfiguration server)
			{
				Components.adminIPFilterServer.SecurityDescriptor = server.TransportServerSecurity;
				Components.queueViewerServer.SecurityDescriptor = server.TransportServerSecurity;
			}

			private ObjectSecurity GetServerAdminSecurity()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2901, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Core\\start.cs");
				Server server = null;
				try
				{
					server = topologyConfigurationSession.FindLocalServer();
				}
				catch (LocalServerNotFoundException arg)
				{
					ExTraceGlobals.GeneralTracer.TraceError<LocalServerNotFoundException>(0L, "FindLocalServer failed: {0}", arg);
					return null;
				}
				RawSecurityDescriptor rawSecurityDescriptor = server.ReadSecurityDescriptor();
				if (rawSecurityDescriptor == null)
				{
					return null;
				}
				FileSecurity fileSecurity = new FileSecurity();
				byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array, 0);
				fileSecurity.SetSecurityDescriptorBinaryForm(array);
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2929, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Core\\start.cs");
				SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
				FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
				fileSecurity.SetAccessRule(fileSystemAccessRule);
				SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
				fileSystemAccessRule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
				fileSecurity.AddAccessRule(fileSystemAccessRule);
				return fileSecurity;
			}

			private ObjectSecurity GetLocalSystemSecurity()
			{
				FileSecurity fileSecurity = new FileSecurity();
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 2957, "GetLocalSystemSecurity", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Core\\start.cs");
				SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
				FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
				fileSecurity.SetAccessRule(fileSystemAccessRule);
				SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
				fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
				fileSecurity.AddAccessRule(fileSystemAccessRule);
				fileSecurity.SetOwner(securityIdentifier);
				return fileSecurity;
			}
		}

		internal class LoggingComponent : ITransportComponent
		{
			public LoggingComponent(bool startMessageTrackingLog, bool startQueueLogging, bool startTransportWlmLog, bool startFlowControlLog, bool startDnsLog) : this(startMessageTrackingLog, startQueueLogging, startTransportWlmLog, startFlowControlLog, startDnsLog, null)
			{
			}

			public LoggingComponent(bool startMessageTrackingLog, bool startQueueLogging, bool startTransportWlmLog, bool startFlowControlLog, bool startDnsLog, string messageTrackingLogPrefix)
			{
				this.startMessageTrackingLog = startMessageTrackingLog;
				this.startQueueLogging = startQueueLogging;
				this.messageTrackingLogPrefix = messageTrackingLogPrefix;
				this.startTransportWlmLog = startTransportWlmLog;
				this.startFlowControlLog = startFlowControlLog;
				this.startDnsLog = startDnsLog;
			}

			public ProtocolLog SmtpSendLog
			{
				get
				{
					return this.smtpSendLog;
				}
			}

			public FlowControlLog FlowControlLog
			{
				get
				{
					return this.flowControlLog;
				}
			}

			public void Load()
			{
				if (this.startMessageTrackingLog)
				{
					if (string.IsNullOrEmpty(this.messageTrackingLogPrefix))
					{
						MessageTrackingLog.Start();
					}
					else
					{
						MessageTrackingLog.Start(this.messageTrackingLogPrefix);
					}
					MessageTrackingLog.Configure(Components.Configuration.LocalServer.TransportServer);
				}
				this.smtpSendLog.Configure(Components.Configuration.LocalServer.TransportServer.SendProtocolLogPath, Components.Configuration.LocalServer.TransportServer.SendProtocolLogMaxAge, Components.Configuration.LocalServer.TransportServer.SendProtocolLogMaxDirectorySize, Components.Configuration.LocalServer.TransportServer.SendProtocolLogMaxFileSize, Components.Configuration.AppConfig.Logging.SmtpSendLogBufferSize, Components.Configuration.AppConfig.Logging.SmtpSendLogFlushInterval, Components.Configuration.AppConfig.Logging.SmtpSendLogAsyncInterval);
				Components.Configuration.LocalServerChanged += this.ConfigUpdate;
				ConnectionLog.Start();
				ConnectionLog.TransportStart(Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions, Components.Configuration.LocalServer.MaxConcurrentMailboxDeliveries, Components.Configuration.LocalServer.TransportServer.MaxOutboundConnections.ToString());
				if (this.systemProbeLoggingEnabled)
				{
					Components.LoggingComponent.StartSystemProbe();
				}
				if (this.startQueueLogging)
				{
					QueueLog.Start();
				}
				if (this.startTransportWlmLog)
				{
					TransportWlmLog.Start();
				}
				if (this.startFlowControlLog)
				{
					this.flowControlLog = new FlowControlLog();
					Server transportServer = Components.Configuration.LocalServer.TransportServer;
					if (!transportServer.FlowControlLogEnabled && (transportServer.FlowControlLogPath == null || string.IsNullOrEmpty(transportServer.FlowControlLogPath.PathName)))
					{
						ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Flow Control Log path was set to null, Flow Control Log is disabled");
						Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_FlowControlLogPathIsNull, null, new object[0]);
					}
					else
					{
						this.flowControlLog.Configure(Components.TransportAppConfig.FlowControlLog, Components.Configuration.LocalServer.TransportServer);
					}
				}
				if (this.startDnsLog && Components.Configuration.LocalServer.TransportServer.DnsLogEnabled)
				{
					string dnsLogPath = (Components.Configuration.LocalServer.TransportServer.DnsLogPath == null) ? null : Components.Configuration.LocalServer.TransportServer.DnsLogPath.PathName;
					DnsLog.Start(dnsLogPath, Components.Configuration.LocalServer.TransportServer.DnsLogMaxAge, (long)Components.Configuration.LocalServer.TransportServer.DnsLogMaxDirectorySize.Value.ToBytes(), (long)Components.Configuration.LocalServer.TransportServer.DnsLogMaxFileSize.Value.ToBytes());
				}
			}

			public void Unload()
			{
				Components.Configuration.LocalServerChanged -= this.ConfigUpdate;
				if (this.startMessageTrackingLog)
				{
					MessageTrackingLog.Stop();
				}
				if (this.systemProbeLoggingEnabled)
				{
					SystemProbe.Stop();
				}
				this.smtpSendLog.Close();
				ConnectionLog.TransportStop();
				ConnectionLog.Stop();
				QueueLog.Stop();
				if (this.startTransportWlmLog)
				{
					TransportWlmLog.Stop();
				}
				if (this.startFlowControlLog)
				{
					this.flowControlLog.Stop();
				}
				DnsLog.Stop();
			}

			public string OnUnhandledException(Exception e)
			{
				ConnectionLog.FlushBuffer();
				TransportWlmLog.FlushBuffer();
				return null;
			}

			private void ConfigUpdate(TransportServerConfiguration server)
			{
				if (this.startMessageTrackingLog)
				{
					MessageTrackingLog.Configure(server.TransportServer);
				}
				this.smtpSendLog.Configure(server.TransportServer.SendProtocolLogPath, server.TransportServer.SendProtocolLogMaxAge, server.TransportServer.SendProtocolLogMaxDirectorySize, server.TransportServer.SendProtocolLogMaxFileSize, Components.Configuration.AppConfig.Logging.SmtpSendLogBufferSize, Components.Configuration.AppConfig.Logging.SmtpSendLogFlushInterval, Components.Configuration.AppConfig.Logging.SmtpSendLogAsyncInterval);
			}

			private static void StartSystemProbe()
			{
				try
				{
					SystemProbe.Start("SYSPRB", Components.configurationComponent.ProcessTransportRole.ToString());
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "System probe started successfully.");
					SystemProbe.ActivityId = CombGuidGenerator.NewGuid();
					SystemProbe.TracePass("Transport", "System probe started successfully.", new object[0]);
					SystemProbe.ActivityId = Guid.Empty;
				}
				catch (LogException ex)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Failed to initialize system probe. {0}", ex.Message);
				}
			}

			private readonly bool startMessageTrackingLog;

			private readonly bool startQueueLogging;

			private readonly string messageTrackingLogPrefix;

			private readonly bool startTransportWlmLog;

			private readonly bool startFlowControlLog;

			private readonly bool startDnsLog;

			private readonly bool systemProbeLoggingEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Transport.SystemProbeLogging.Enabled;

			private ProtocolLog smtpSendLog = new ProtocolLog("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version, "SMTP Send Protocol Log", "SEND", "SmtpSendProtocolLogs");

			private FlowControlLog flowControlLog;
		}

		internal class PerfCountersLoader : ITransportComponent, IDiagnosable, IPerfCountersLoader
		{
			public PerfCountersLoader(bool initSecureMailPerfCounters)
			{
				this.initSecureMailPerfCounters = initSecureMailPerfCounters;
			}

			public void Load()
			{
				Components.PerfCountersLoader.InitializeADRecipientCachePerfCounters();
				if (this.initSecureMailPerfCounters)
				{
					Utils.InitPerfCounters();
				}
				LatencyTracker.Start(Components.TransportAppConfig.LatencyTracker, Components.Configuration.ProcessTransportRole);
			}

			public void Unload()
			{
				ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Logging the number of messages left in the queues");
				ProcessTransportRole processTransportRole = Components.configurationComponent.ProcessTransportRole;
				QueuingPerfCountersInstance queuingPerfCountersInstance = (processTransportRole == ProcessTransportRole.FrontEnd) ? null : QueueManager.GetTotalPerfCounters();
				bool flag = queuingPerfCountersInstance != null && (processTransportRole == ProcessTransportRole.Hub || processTransportRole == ProcessTransportRole.Edge);
				bool flag2 = ShadowRedundancyManager.PerfCounters != null && ShadowRedundancyManager.PerfCounters.IsValid(ShadowRedundancyCounterId.RedundantMessageDiscardEvents);
				if (flag || flag2)
				{
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_QueuingStatusAtShutdown, null, new object[]
					{
						flag ? queuingPerfCountersInstance.SubmissionQueueLength.RawValue.ToString() : string.Empty,
						flag ? queuingPerfCountersInstance.InternalAggregateDeliveryQueueLength.RawValue.ToString() : string.Empty,
						flag ? queuingPerfCountersInstance.ExternalAggregateDeliveryQueueLength.RawValue.ToString() : string.Empty,
						flag ? queuingPerfCountersInstance.UnreachableQueueLength.RawValue.ToString() : string.Empty,
						flag ? queuingPerfCountersInstance.PoisonQueueLength.RawValue.ToString() : string.Empty,
						flag ? queuingPerfCountersInstance.AggregateShadowQueueLength.RawValue.ToString() : string.Empty,
						flag2 ? ShadowRedundancyManager.PerfCounters.RedundantMessageDiscardEvents.ToString() : string.Empty
					});
				}
				ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Stopping latency tracking");
				LatencyTracker.Stop();
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}

			private static void InitializeADRecipientCachePerfCounters()
			{
				if (!Components.IsBridgehead)
				{
					return;
				}
				try
				{
					ADRecipientCache<TransportMiniRecipient>.InitializePerfCounters(Process.GetCurrentProcess().ProcessName);
				}
				catch (InvalidOperationException ex)
				{
					string text = string.Format("Failed to initialize AD recipient cache performance counters: {0}", ex);
					ExTraceGlobals.GeneralTracer.TraceError(0L, text);
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ADRecipientCachePerfCountersLoadFailure, null, new object[]
					{
						ex.ToString()
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Error, false);
				}
			}

			public void AddCounterToGetExchangeDiagnostics(Type counterType, string counterName)
			{
				this.otherCounters[counterType] = counterName;
			}

			private bool GetPerfCounterInfoUsingReflection(string argument, Type classType, XElement perfCountersElement, bool showVerbose, string counterName)
			{
				bool result = false;
				string text = string.IsNullOrEmpty(counterName) ? classType.Name : counterName;
				XElement xelement = new XElement(text);
				perfCountersElement.Add(xelement);
				if (showVerbose || argument.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
				{
					MethodInfo method = classType.GetMethod("GetPerfCounterInfo");
					if (method != null)
					{
						method.Invoke(classType, new object[]
						{
							xelement
						});
						result = true;
					}
					else
					{
						xelement.Add(string.Format("{0} does not have GetPerfCounterInfo method defined", classType.Name));
					}
				}
				return result;
			}

			public string GetDiagnosticComponentName()
			{
				return "PerfCounters";
			}

			public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
			{
				XElement xelement = new XElement(this.GetDiagnosticComponentName());
				bool showVerbose = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
				bool flag = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
				bool flag2 = false;
				foreach (Type classType in Components.PerfCountersLoader.TransportPerfCounters)
				{
					flag2 |= this.GetPerfCounterInfoUsingReflection(parameters.Argument, classType, xelement, showVerbose, null);
				}
				foreach (KeyValuePair<Type, string> keyValuePair in this.otherCounters)
				{
					flag2 |= this.GetPerfCounterInfoUsingReflection(parameters.Argument, keyValuePair.Key, xelement, showVerbose, keyValuePair.Value);
				}
				if (!flag2 || flag)
				{
					XElement xelement2 = new XElement("Example1");
					xelement2.SetValue("$xml=[xml](Get-ExchangeDiagnosticInfo -Process edgetransport -component perfcounters -Argument verbose -server ht001);$xml.Diagnostics.Components.PerfCounters");
					XElement xelement3 = new XElement("Example2");
					xelement3.SetValue("$xml=[xml](Get-ExchangeDiagnosticInfo -Process edgetransport -component perfcounters -argument QueuingPerfCounters,SmtpReceivePerfCounters -server ht001);$xml.Diagnostics.Components.PerfCounters.QueuingPerfCounters");
					xelement.Add(xelement2);
					xelement.Add(xelement3);
				}
				return xelement;
			}

			private static Type[] GetTransportPerfCounters()
			{
				return new Type[]
				{
					typeof(CertificateValidationResultCachePerfCounters),
					typeof(ConfigurationCachePerfCounters),
					typeof(DatabasePerfCounters),
					typeof(DeliveryAgentPerfCounters),
					typeof(DeliveryFailurePerfCounters),
					typeof(DsnGeneratorPerfCounters),
					typeof(LatencyTrackerEndToEndPerfCounters),
					typeof(LatencyTrackerPerfCounters),
					typeof(MessageResubmissionPerformanceCounters),
					typeof(OutboundIPPoolPerfCounters),
					typeof(PickupPerfCounters),
					typeof(SubmitHelperPerfCounters),
					typeof(ProxyHubSelectorPerfCounters),
					typeof(QueuingPerfCounters),
					typeof(ResolverPerfCounters),
					typeof(RoutingPerfCounters),
					typeof(SecureMailTransportPerfCounters),
					typeof(ShadowRedundancyPerfCounters),
					typeof(ShadowRedundancyInstancePerfCounters),
					typeof(SmtpAvailabilityPerfCounters),
					typeof(SmtpConnectionCachePerfCounters),
					typeof(SmtpProxyPerfCounters),
					typeof(SmtpReceiveFrontendPerfCounters),
					typeof(SmtpReceivePerfCounters),
					typeof(SmtpResponseSubCodePerfCounters),
					typeof(SmtpSendPerfCounters),
					typeof(MExCounters),
					typeof(MSExchangeADRecipientCache),
					typeof(IsMemberOfResolverPerfCounters),
					typeof(QueueQuotaComponentPerfCounters)
				};
			}

			private readonly bool initSecureMailPerfCounters;

			private static Type[] TransportPerfCounters = Components.PerfCountersLoader.GetTransportPerfCounters();

			private Dictionary<Type, string> otherCounters = new Dictionary<Type, string>();
		}

		internal class StoreDriverLoader : IStartableTransportComponent, ITransportComponent
		{
			public string CurrentState
			{
				get
				{
					if (Components.storeDriver != null)
					{
						return Components.storeDriver.CurrentState;
					}
					return null;
				}
			}

			public void Load()
			{
				if (!Components.IsBridgehead)
				{
					return;
				}
				Components.LoadStoreDriverAssembly();
				Type type = Components.storeDriverAssembly.GetType("Microsoft.Exchange.MailboxTransport.StoreDriver.StoreDriver", true);
				MethodInfo method = type.GetMethod("CreateStoreDriver", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (null == method)
				{
					throw new InvalidOperationException("CreateStoreDriver method was not found.");
				}
				object obj = method.Invoke(null, null);
				if (obj == null)
				{
					ExTraceGlobals.GeneralTracer.TraceError((long)this.GetHashCode(), "Failed to create the store driver object; Local delivery is disabled");
					return;
				}
				Components.storeDriver = (obj as IStoreDriver);
				this.mexEventsType = Components.storeDriverAssembly.GetType("Microsoft.Exchange.MailboxTransport.StoreDriver.MExEvents", true);
				MethodInfo method2 = this.mexEventsType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (null == method2)
				{
					throw new InvalidOperationException("MExEvents.Initialize method was not found.");
				}
				try
				{
					method2.Invoke(null, new object[]
					{
						Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config")
					});
				}
				catch (TargetInvocationException ex)
				{
					ExchangeConfigurationException ex2 = ex.InnerException as ExchangeConfigurationException;
					if (ex2 == null)
					{
						throw;
					}
					string text = "StoreDriver.MExEvents.Initialize threw ExchangeConfigurationException: shutting down service.";
					ExTraceGlobals.SchedulerTracer.TraceError((long)this.GetHashCode(), text);
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotStartAgents, null, new object[]
					{
						ex2.LocalizedString,
						ex2
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Error, false);
					bool canRetry = Components.HasTransientException(ex2);
					Components.StopService(ex2.Message, canRetry, false, false);
				}
			}

			public void Unload()
			{
				if (null != this.mexEventsType)
				{
					MethodInfo method = this.mexEventsType.GetMethod("Shutdown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == method)
					{
						throw new InvalidOperationException("MExEvents.Shutdown method was not found.");
					}
					method.Invoke(null, null);
				}
			}

			public string OnUnhandledException(Exception e)
			{
				if (Components.storeDriver != null)
				{
					Components.storeDriver.Pause();
				}
				return null;
			}

			public void Start(bool initiallyPaused, ServiceState targetRunningState)
			{
				this.targetRunningState = targetRunningState;
				if (Components.storeDriver != null)
				{
					Components.storeDriver.Start(initiallyPaused || !this.ShouldExecute());
				}
			}

			public void Stop()
			{
				if (Components.storeDriver != null)
				{
					Components.storeDriver.Stop();
				}
			}

			public void Pause()
			{
				if (Components.storeDriver != null)
				{
					Components.storeDriver.Pause();
				}
			}

			public void Continue()
			{
				if (Components.storeDriver != null && this.ShouldExecute())
				{
					Components.storeDriver.Continue();
				}
			}

			private bool ShouldExecute()
			{
				return this.targetRunningState == ServiceState.Active || this.targetRunningState == ServiceState.Draining;
			}

			private Type mexEventsType;

			private ServiceState targetRunningState;
		}

		internal class AggregatorLoader : IStartableTransportComponent, ITransportComponent
		{
			public string CurrentState
			{
				get
				{
					if (Components.aggregator != null)
					{
						return Components.aggregator.CurrentState;
					}
					return null;
				}
			}

			public void Load()
			{
				if (!Components.IsBridgehead)
				{
					return;
				}
				this.LoadTransportSyncAssemblies();
				Type type = Components.AggregatorLoader.transportSyncWorkerAssembly.GetType("Microsoft.Exchange.MailboxTransport.ContentAggregation.AggregationComponent", true);
				MethodInfo method = type.GetMethod("CreateAggregationComponent", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					throw new InvalidOperationException("CreateAggregationComponent method not found");
				}
				object obj = method.Invoke(null, null);
				if (obj == null)
				{
					ExTraceGlobals.GeneralTracer.TraceError((long)this.GetHashCode(), "Failed to create the aggregation component, no aggregator on this hub");
					return;
				}
				Components.aggregator = (obj as IStartableTransportComponent);
				if (Components.aggregator != null)
				{
					Components.aggregator.Load();
				}
			}

			public void Unload()
			{
				if (Components.aggregator != null)
				{
					Components.aggregator.Unload();
				}
			}

			public string OnUnhandledException(Exception e)
			{
				if (Components.aggregator != null)
				{
					return Components.aggregator.OnUnhandledException(e);
				}
				return null;
			}

			public void Start(bool initiallyPaused, ServiceState targetRunningState)
			{
				if (Components.aggregator != null)
				{
					Components.aggregator.Start(initiallyPaused, targetRunningState);
				}
			}

			public void Stop()
			{
				if (Components.aggregator != null)
				{
					Components.aggregator.Stop();
				}
			}

			public void Pause()
			{
				if (Components.aggregator != null)
				{
					Components.aggregator.Pause();
				}
			}

			public void Continue()
			{
				if (Components.aggregator != null)
				{
					Components.aggregator.Continue();
				}
			}

			private void LoadTransportSyncAssemblies()
			{
				AssemblyName assemblyName = null;
				if (Components.AggregatorLoader.transportSyncCommonAssembly == null)
				{
					lock (Components.AggregatorLoader.syncRoot)
					{
						if (Components.AggregatorLoader.transportSyncCommonAssembly == null)
						{
							assemblyName = Assembly.GetExecutingAssembly().GetName();
							string assemblyString = assemblyName.FullName.Replace(assemblyName.Name, "Microsoft.Exchange.Transport.Sync.Common");
							Components.AggregatorLoader.transportSyncCommonAssembly = Assembly.Load(assemblyString);
						}
					}
				}
				if (Components.AggregatorLoader.transportSyncWorkerAssembly == null)
				{
					lock (Components.AggregatorLoader.syncRoot)
					{
						if (Components.AggregatorLoader.transportSyncWorkerAssembly == null)
						{
							if (assemblyName == null)
							{
								assemblyName = Assembly.GetExecutingAssembly().GetName();
							}
							string assemblyString2 = assemblyName.FullName.Replace(assemblyName.Name, "Microsoft.Exchange.Transport.Sync.Worker");
							Components.AggregatorLoader.transportSyncWorkerAssembly = Assembly.Load(assemblyString2);
						}
					}
				}
			}

			private static object syncRoot = new object();

			private static Assembly transportSyncCommonAssembly;

			private static Assembly transportSyncWorkerAssembly;
		}

		internal class CategorizerMExRuntimeLoader : ITransportComponent, IDiagnosable
		{
			public void Load()
			{
				try
				{
					MExEvents.Initialize(Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config"));
				}
				catch (ExchangeConfigurationException ex)
				{
					string text = "StoreDriver.MExEvents.Initialize threw ExchangeConfigurationException: shutting down service.";
					ExTraceGlobals.SchedulerTracer.TraceError((long)this.GetHashCode(), text);
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotStartAgents, null, new object[]
					{
						ex.LocalizedString,
						ex
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Error, false);
					bool canRetry = Components.HasTransientException(ex);
					Components.StopService(ex.Message, canRetry, false, false);
				}
			}

			public void Unload()
			{
				MExEvents.Shutdown();
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}

			string IDiagnosable.GetDiagnosticComponentName()
			{
				return "RoutingAgents";
			}

			XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
			{
				XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
				xelement.Add(MExEvents.GetDiagnosticInfo(parameters));
				return xelement;
			}
		}

		internal class BootScannerMExRuntimeLoader : ITransportComponent, IDiagnosable
		{
			public void Load()
			{
				try
				{
					StorageAgentMExEvents.Initialize(Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config"));
				}
				catch (ExchangeConfigurationException ex)
				{
					string text = "StorageAgentMExEvents.Initialize threw ExchangeConfigurationException: shutting down service.";
					ExTraceGlobals.SchedulerTracer.TraceError((long)this.GetHashCode(), text);
					Components.eventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotStartAgents, null, new object[]
					{
						ex.LocalizedString,
						ex
					});
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Error, false);
					bool canRetry = Components.HasTransientException(ex);
					Components.StopService(ex.Message, canRetry, false, false);
				}
			}

			public void Unload()
			{
				StorageAgentMExEvents.Shutdown();
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}

			string IDiagnosable.GetDiagnosticComponentName()
			{
				return "StorageAgents";
			}

			XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
			{
				XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
				xelement.Add(StorageAgentMExEvents.GetDiagnosticInfo(parameters));
				return xelement;
			}
		}

		internal class TransportMailItemLoader : ITransportComponent
		{
			public void Load()
			{
				TransportMailItem.SetComponents(Components.Configuration, Components.ResourceManagerComponent.ResourceManager, Components.shadowRedundancyComponent, Components.MessagingDatabase.Database);
			}

			public void Unload()
			{
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}
		}

		internal class MicrosoftExchangeRecipientLoader : ITransportComponent
		{
			public void Load()
			{
				GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Start();
			}

			public void Unload()
			{
				GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Stop();
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}
		}

		internal class DirectTrustLoader : ITransportComponent
		{
			public void Load()
			{
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.DirectTrustCache.Enabled)
				{
					DirectTrust.Load();
				}
			}

			public void Unload()
			{
				DirectTrust.Unload();
			}

			public string OnUnhandledException(Exception e)
			{
				return null;
			}
		}

		internal class TransportPickupSubmissionHandler : IPickupSubmitHandler
		{
			public void OnSubmit(TransportMailItem item, MailDirectionality direction, PickupType pickupType)
			{
				if (direction != MailDirectionality.Undefined)
				{
					item.Directionality = direction;
				}
				item.CommitImmediate();
				Components.CategorizerComponent.EnqueueSubmittedMessage(item);
			}
		}

		private class ComponentNotAvailableException : Exception
		{
			internal ComponentNotAvailableException(string componentName) : base("component " + componentName + " is not available")
			{
			}
		}
	}
}
