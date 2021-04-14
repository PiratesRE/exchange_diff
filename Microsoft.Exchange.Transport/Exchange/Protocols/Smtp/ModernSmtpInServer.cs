using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ModernSmtpInServer : ISmtpInServer
	{
		public string Name
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public Version Version
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public TransportConfigContainer TransportSettings
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ITransportConfiguration Configuration
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public Server ServerConfiguration
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public bool IsBridgehead
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ICertificateValidator CertificateValidator
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IShadowRedundancyManager ShadowRedundancyManager
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ICategorizer Categorizer
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IInboundProxyDestinationTracker InboundProxyDestinationTracker
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IInboundProxyDestinationTracker InboundProxyAccountForestTracker
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ICertificateCache CertificateCache
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public SmtpProxyPerfCountersWrapper ClientProxyPerfCounters
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public SmtpProxyPerfCountersWrapper OutboundProxyPerfCounters
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public OutboundProxyBySourceTracker OutboundProxyBySourceTracker
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public SmtpOutConnectionHandler SmtpOutConnectionHandler
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ISmtpInMailItemStorage MailItemStorage
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IProxyHubSelector ProxyHubSelector
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public ISmtpReceiveConfiguration ReceiveConfiguration
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IPConnectionTable InboundTlsIPConnectionTable
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public IEventNotificationItem EventNotificationItem
		{
			get
			{
				throw ModernSmtpInServer.LegacyStackOnlyException;
			}
		}

		public void RemoveConnection(long id)
		{
			throw ModernSmtpInServer.LegacyStackOnlyException;
		}

		public ServiceState TargetRunningState
		{
			get
			{
				return this.serverState.ServiceState;
			}
			set
			{
				this.serverState.ServiceState = value;
			}
		}

		public void SetRejectState(bool rejectCommands, bool rejectMailSubmission, bool rejectMailFromInternet, SmtpResponse rejectionResponse)
		{
			this.serverState.SetRejectState(rejectCommands, rejectMailSubmission, rejectMailFromInternet, rejectionResponse);
		}

		public bool RejectCommands
		{
			get
			{
				return this.serverState.RejectCommands;
			}
		}

		public bool RejectSubmits
		{
			get
			{
				return this.serverState.RejectSubmits;
			}
		}

		public bool RejectMailFromInternet
		{
			get
			{
				return this.serverState.RejectMailFromInternet;
			}
		}

		public SmtpResponse RejectionSmtpResponse
		{
			get
			{
				return this.serverState.RejectionSmtpResponse;
			}
		}

		public DateTime CurrentTime
		{
			get
			{
				return this.serverState.CurrentTime;
			}
			set
			{
				this.serverState.CurrentTime = value;
			}
		}

		public void SetThrottleState(TimeSpan perMessageDelay, string diagnosticContext)
		{
			this.serverState.SetThrottleState(perMessageDelay, diagnosticContext);
		}

		public TimeSpan ThrottleDelay
		{
			get
			{
				return this.serverState.ThrottleDelay;
			}
		}

		public string ThrottleDelayContext
		{
			get
			{
				return this.serverState.ThrottleDelayContext;
			}
		}

		public bool Ipv6ReceiveConnectionThrottlingEnabled { get; private set; }

		public bool ReceiveTlsThrottlingEnabled { get; private set; }

		public string CurrentState
		{
			get
			{
				return string.Empty;
			}
		}

		public void SetRunTimeDependencies(IAgentRuntime agentRuntime, IMailRouter mailRouter, IProxyHubSelector proxyHubSelector, IEnhancedDns enhancedDns, ICategorizer categorizer, ICertificateCache certificateCache, ICertificateValidator certificateValidator, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, ISmtpInMailItemStorage mailItemStorage, SmtpOutConnectionHandler smtpOutConnectionHandler, IQueueQuotaComponent queueQuotaComponent)
		{
			ArgumentValidator.ThrowIfNull("agentRuntime", agentRuntime);
			ArgumentValidator.ThrowIfNull("categorizer", categorizer);
			ArgumentValidator.ThrowIfNull("certificateCache", certificateCache);
			ArgumentValidator.ThrowIfNull("certificateValidator", certificateValidator);
			ArgumentValidator.ThrowIfNull("memberOfResolver", memberOfResolver);
			ArgumentValidator.ThrowIfNull("messageThrottlingManager", messageThrottlingManager);
			ArgumentValidator.ThrowIfNull("mailItemStorage", mailItemStorage);
			this.serverState.SetRunTimeDependencies(agentRuntime, categorizer, certificateCache, certificateValidator, memberOfResolver, messageThrottlingManager, shadowRedundancyManager, mailItemStorage, queueQuotaComponent);
			this.receiveConnectorManager = this.CreateReceiveConnectorManager(this.serverState.SmtpConfiguration);
			this.OnTransportServerConfigurationChanged(this.configuration.LocalServer);
			this.OnReceiveConnectorsChanged(this.configuration.LocalReceiveConnectors);
			this.runtimeDependenciesSet = true;
		}

		public void SetLoadTimeDependencies(IProtocolLog protocolLogToUse, ITransportAppConfig transportAppConfigToUse, ITransportConfiguration transportConfigurationToUse)
		{
			ArgumentValidator.ThrowIfNull("protocolLogToUse", protocolLogToUse);
			ArgumentValidator.ThrowIfNull("transportAppConfigToUse", transportAppConfigToUse);
			ArgumentValidator.ThrowIfNull("transportConfigurationToUse", transportConfigurationToUse);
			this.Ipv6ReceiveConnectionThrottlingEnabled = transportAppConfigToUse.SmtpReceiveConfiguration.Ipv6ReceiveConnectionThrottlingEnabled;
			this.ReceiveTlsThrottlingEnabled = transportAppConfigToUse.SmtpReceiveConfiguration.ReceiveTlsThrottlingEnabled;
			this.configuration = transportConfigurationToUse;
			this.serverState.SetLoadTimeDependencies(protocolLogToUse, transportAppConfigToUse, transportConfigurationToUse);
			this.loadtimeDependenciesSet = true;
		}

		public void Load()
		{
			this.ThrowIfLoadTimeDependenciesNotSet();
			this.ConfigureProtocolLog(this.serverState.SmtpConfiguration.TransportConfiguration.LocalServer);
			this.configuration.LocalServerChanged += this.OnTransportServerConfigurationChanged;
			this.configuration.LocalReceiveConnectorsChanged += this.OnReceiveConnectorsChanged;
		}

		public void Unload()
		{
			this.ThrowIfLoadTimeDependenciesNotSet();
			this.configuration.LocalServerChanged -= this.OnTransportServerConfigurationChanged;
			this.configuration.LocalReceiveConnectorsChanged -= this.OnReceiveConnectorsChanged;
		}

		public void Initialize(TcpListener.HandleFailure failureDelegate = null, TcpListener.HandleConnection connectionHandler = null)
		{
			this.ThrowIfLoadTimeDependenciesNotSet();
			this.ThrowIfRunTimeDependenciesNotSet();
			this.OnTransportServerConfigurationChanged(this.configuration.LocalServer);
			this.tcpListener = this.CreateTcpListener(failureDelegate, connectionHandler);
			this.OnReceiveConnectorsChanged(this.configuration.LocalReceiveConnectors);
			this.tcpListener.StartListening(true);
		}

		public void Shutdown()
		{
			this.ThrowIfLoadTimeDependenciesNotSet();
			this.ThrowIfRunTimeDependenciesNotSet();
			this.ThrowIfShuttingDown();
			if (this.tcpListener != null)
			{
				this.tcpListener.ProcessStopping = true;
				this.tcpListener.StopListening();
				this.tcpListener.Shutdown();
				this.tcpListener = null;
			}
			this.cancellationTokenSource.Cancel();
			this.WaitForAllStateMachinesToTerminate(ModernSmtpInServer.ShutdownTimeout);
			this.serverState.ProtocolLog.Close();
		}

		public void NonGracefullyCloseTcpListener()
		{
			if (this.tcpListener != null)
			{
				this.tcpListener.StopListening();
				this.tcpListener = null;
			}
		}

		public INetworkConnection CreateNetworkConnection(Socket socket, int receiveBufferSize)
		{
			ArgumentValidator.ThrowIfNull("socket", socket);
			return new CancellableNetworkConnection(socket, this.cancellationTokenSource.Token, receiveBufferSize);
		}

		public bool HandleConnection(INetworkConnection connection)
		{
			ArgumentValidator.ThrowIfNull("connection", connection);
			this.ThrowIfLoadTimeDependenciesNotSet();
			this.ThrowIfRunTimeDependenciesNotSet();
			if (this.ShouldRejectConnectionOnFrontEndRole(connection))
			{
				return false;
			}
			SmtpReceiveConnectorStub smtpReceiveConnectorStub;
			if (!this.receiveConnectorManager.TryLookupIncomingConnection(connection.LocalEndPoint, connection.RemoteEndPoint, out smtpReceiveConnectorStub))
			{
				return false;
			}
			connection.MaxLineLength = 4000;
			connection.Timeout = (int)smtpReceiveConnectorStub.Connector.ConnectionInactivityTimeout.TotalSeconds;
			MailboxTransportSmtpInStateMachine mailboxTransportSmtpInStateMachine = new MailboxTransportSmtpInStateMachine(new SmtpInSessionState(this.serverState, connection, smtpReceiveConnectorStub));
			Task task = mailboxTransportSmtpInStateMachine.ExecuteAsync(this.cancellationTokenSource.Token);
			if (this.currentSessions.TryAdd(connection.ConnectionId, task))
			{
				task.ContinueWith(delegate(Task completedTask)
				{
					Task task2;
					if (!this.currentSessions.TryRemove(connection.ConnectionId, out task2))
					{
						this.serverState.Tracer.TraceDebug(connection.ConnectionId, "this.currentSessions.TryRemove() returned false");
					}
				});
			}
			else
			{
				this.serverState.Tracer.TraceDebug(connection.ConnectionId, "this.currentSessions.TryAdd() returned false");
			}
			return true;
		}

		public void AddDiagnosticInfo(DiagnosableParameters parameters, XElement element)
		{
		}

		protected virtual ReceiveConnectorManager CreateReceiveConnectorManager(ISmtpReceiveConfiguration config)
		{
			return new ReceiveConnectorManager(config);
		}

		protected virtual ITcpListener CreateTcpListener(TcpListener.HandleFailure failureDelegate, TcpListener.HandleConnection connectionHandler)
		{
			return new TcpListener(failureDelegate, connectionHandler, null, this.serverState.Tracer, this.serverState.EventLog, 1200, this.serverState.SmtpConfiguration.TransportConfiguration.ExclusiveAddressUse, this.serverState.SmtpConfiguration.TransportConfiguration.DisableHandleInheritance);
		}

		protected void WaitForAllStateMachinesToTerminate(TimeSpan timeout)
		{
			if (!Task.WaitAll(this.currentSessions.Values.ToArray<Task>(), timeout))
			{
				this.LogTimedOutWaitingForStateMachinesToTerminate(timeout);
			}
		}

		protected virtual void LogTimedOutWaitingForStateMachinesToTerminate(TimeSpan timeout)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Timed out waiting for all state machines to terminate (duration {0}", timeout);
			stringBuilder.AppendFormat("Number of sessions remaining: {0}", this.currentSessions.Count);
			Exception ex = new InvalidOperationException(stringBuilder.ToString());
			bool flag;
			ExWatson.SendThrottledGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.015", Assembly.GetExecutingAssembly().GetName().Name, ex.GetType().Name, ex.StackTrace, ex.GetHashCode().ToString(CultureInfo.InvariantCulture), ex.TargetSite.Name, "details", TimeSpan.FromHours(1.0), out flag);
		}

		private void OnTransportServerConfigurationChanged(TransportServerConfiguration config)
		{
			this.ConfigureProtocolLog(config.TransportServer);
			if (this.tcpListener != null)
			{
				this.tcpListener.MaxConnectionRate = config.TransportServer.MaxConnectionRatePerMinute;
			}
			if (this.receiveConnectorManager != null)
			{
				this.receiveConnectorManager.ApplyLocalServerConfiguration(config.TransportServer);
			}
		}

		private void OnReceiveConnectorsChanged(ReceiveConnectorConfiguration connectorConfig)
		{
			this.receiveConnectorManager.ApplyReceiveConnectors(connectorConfig.Connectors, this.serverState.SmtpConfiguration.TransportConfiguration.LocalServer);
			if (this.tcpListener != null)
			{
				this.tcpListener.SetBindings(this.receiveConnectorManager.Bindings.ToArray(), true);
			}
			this.serverState.EventLog.LogEvent(TransportEventLogConstants.Tuple_ConfiguredConnectors, null, null);
		}

		private bool ShouldRejectConnectionOnFrontEndRole(INetworkConnection connection)
		{
			return this.serverState.SmtpConfiguration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd && this.TargetRunningState == ServiceState.Inactive && !this.serverState.IsLocalAddress(connection.RemoteEndPoint.Address);
		}

		private void ConfigureProtocolLog(Server server)
		{
			this.serverState.ProtocolLog.Configure(server.ReceiveProtocolLogPath, server.ReceiveProtocolLogMaxAge, server.ReceiveProtocolLogMaxDirectorySize, server.ReceiveProtocolLogMaxFileSize, this.serverState.SmtpConfiguration.DiagnosticsConfiguration.SmtpRecvLogBufferSize, this.serverState.SmtpConfiguration.DiagnosticsConfiguration.SmtpRecvLogFlushInterval, this.serverState.SmtpConfiguration.DiagnosticsConfiguration.SmtpRecvLogAsyncInterval);
		}

		private void ThrowIfLoadTimeDependenciesNotSet()
		{
			if (!this.loadtimeDependenciesSet)
			{
				throw new InvalidOperationException("SetLoadTimeDependencies() must be invoked before other methods are usable");
			}
		}

		private void ThrowIfRunTimeDependenciesNotSet()
		{
			if (!this.runtimeDependenciesSet)
			{
				throw new InvalidOperationException("SetRunTimeDependencies() must be invoked before other methods are usable");
			}
		}

		private void ThrowIfShuttingDown()
		{
			if (this.cancellationTokenSource.Token.IsCancellationRequested)
			{
				throw new InvalidOperationException("Shutdown() can only be invoked once");
			}
		}

		private static readonly InvalidOperationException LegacyStackOnlyException = new InvalidOperationException("This method is not used by the new SMTP stack");

		private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(15.0);

		private readonly SmtpInServerState serverState = new SmtpInServerState();

		private readonly ConcurrentDictionary<long, Task> currentSessions = new ConcurrentDictionary<long, Task>();

		private bool runtimeDependenciesSet;

		private bool loadtimeDependenciesSet;

		private ITcpListener tcpListener;

		private ReceiveConnectorManager receiveConnectorManager;

		private ITransportConfiguration configuration;

		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
	}
}
