using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
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
	internal class SmtpInServer : ISmtpInServer
	{
		public SmtpInServer()
		{
			this.currentTime = DateTime.UtcNow;
			this.configUpdateLock = new ReaderWriterLockSlim();
			this.ThrottleDelay = TimeSpan.Zero;
			this.eventLogger = new ExEventLog(ExTraceGlobals.SmtpReceiveTracer.Category, TransportEventLog.GetEventSource());
		}

		public IInboundProxyDestinationTracker InboundProxyDestinationTracker
		{
			get
			{
				return this.inboundProxyDestinationTracker;
			}
		}

		public IInboundProxyDestinationTracker InboundProxyAccountForestTracker
		{
			get
			{
				return this.inboundProxyAccountForestTracker;
			}
		}

		public ICategorizer Categorizer
		{
			get
			{
				return this.categorizer;
			}
		}

		public ISmtpInMailItemStorage MailItemStorage
		{
			get
			{
				return this.mailItemStorage;
			}
		}

		public ICertificateCache CertificateCache
		{
			get
			{
				return this.certificateCache;
			}
		}

		public ICertificateValidator CertificateValidator
		{
			get
			{
				return this.certificateValidator;
			}
		}

		public void SetRejectState(bool rejectCommands, bool rejectMailSubmission, bool rejectMailFromInternet, SmtpResponse rejectionResponse)
		{
			this.RejectCommands = rejectCommands;
			this.RejectSubmits = rejectMailSubmission;
			this.RejectMailFromInternet = rejectMailFromInternet;
			this.RejectionSmtpResponse = rejectionResponse;
		}

		public bool RejectCommands { get; private set; }

		public bool RejectSubmits { get; private set; }

		public bool RejectMailFromInternet { get; private set; }

		public SmtpResponse RejectionSmtpResponse { get; private set; }

		public IShadowRedundancyManager ShadowRedundancyManager
		{
			get
			{
				return this.shadowRedundancyManager;
			}
		}

		public string Name
		{
			get
			{
				return SmtpReceiveServer.ServerName;
			}
		}

		public Version Version
		{
			get
			{
				return this.serverVersion;
			}
		}

		public ServiceState TargetRunningState
		{
			get
			{
				return this.targetRunningState;
			}
			set
			{
				this.targetRunningState = value;
			}
		}

		public DateTime CurrentTime
		{
			get
			{
				return this.currentTime;
			}
			set
			{
				this.currentTime = value;
			}
		}

		public ITransportConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		public bool IsBridgehead
		{
			get
			{
				return this.configuration.LocalServer.IsBridgehead;
			}
		}

		public TransportConfigContainer TransportSettings
		{
			get
			{
				return this.configuration.TransportSettings.TransportSettings;
			}
		}

		public Server ServerConfiguration
		{
			get
			{
				return this.configuration.LocalServer.TransportServer;
			}
		}

		public void SetThrottleState(TimeSpan perMessageDelay, string diagnosticContext)
		{
			this.ThrottleDelay = perMessageDelay;
			this.ThrottleDelayContext = diagnosticContext;
		}

		public TimeSpan ThrottleDelay { get; private set; }

		public string ThrottleDelayContext { get; private set; }

		public string CurrentState
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<ISmtpInSession> list = this.sessions.TakeSnapshot();
				int count = list.Count;
				stringBuilder.Append("SessionCount=");
				stringBuilder.AppendLine(count.ToString(CultureInfo.InvariantCulture));
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append("Session[");
					stringBuilder.Append(i.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append("]: ");
					ISmtpInSession smtpInSession = list[i];
					Breadcrumbs<SmtpInSessionBreadcrumbs> breadcrumbs = smtpInSession.Breadcrumbs;
					stringBuilder.Append("LastBcIndex=");
					stringBuilder.Append(breadcrumbs.LastFilledIndex.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append("; BcData=");
					foreach (SmtpInSessionBreadcrumbs smtpInSessionBreadcrumbs in breadcrumbs.BreadCrumb)
					{
						stringBuilder.Append(smtpInSessionBreadcrumbs);
						stringBuilder.Append(",");
					}
					stringBuilder.Append("; ");
					SmtpSession sessionSource = smtpInSession.SessionSource;
					if (sessionSource != null)
					{
						IExecutionControl executionControl = sessionSource.ExecutionControl;
						if (executionControl == null)
						{
							stringBuilder.Append("MExSession=null");
						}
						else
						{
							stringBuilder.Append("Agent=");
							stringBuilder.Append(executionControl.ExecutingAgentName);
						}
					}
					stringBuilder.AppendLine();
				}
				return stringBuilder.ToString();
			}
		}

		public SmtpOutConnectionHandler SmtpOutConnectionHandler
		{
			get
			{
				return this.smtpOutConnectionHandler;
			}
		}

		public SmtpProxyPerfCountersWrapper ClientProxyPerfCounters
		{
			get
			{
				return this.clientProxyPerfCounters;
			}
		}

		public SmtpProxyPerfCountersWrapper OutboundProxyPerfCounters
		{
			get
			{
				return this.outboundProxyPerfCounters;
			}
		}

		public OutboundProxyBySourceTracker OutboundProxyBySourceTracker
		{
			get
			{
				return this.outboundProxyBySourceTracker;
			}
		}

		public ISmtpReceiveConfiguration ReceiveConfiguration { get; private set; }

		public IProxyHubSelector ProxyHubSelector
		{
			get
			{
				return this.proxyHubSelector;
			}
		}

		public IPConnectionTable InboundTlsIPConnectionTable
		{
			get
			{
				return this.inboundTlsIPConnectionTable;
			}
		}

		public bool Ipv6ReceiveConnectionThrottlingEnabled
		{
			get
			{
				return this.ipv6ReceiveConnectionThrottlingEnabled;
			}
		}

		public bool ReceiveTlsThrottlingEnabled
		{
			get
			{
				return this.receiveTlsThrottlingEnabled;
			}
		}

		public IEventNotificationItem EventNotificationItem
		{
			get
			{
				return this.eventNotificationItem;
			}
		}

		private bool SelfListening
		{
			get
			{
				return this.tcpListener != null;
			}
		}

		public void SetRunTimeDependencies(IAgentRuntime agentRuntime, IMailRouter mailRouter, IProxyHubSelector proxyHubSelector, IEnhancedDns enhancedDns, ICategorizer categorizer, ICertificateCache certificateCache, ICertificateValidator certificateValidator, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, ISmtpInMailItemStorage mailItemStorage, SmtpOutConnectionHandler smtpOutConnectionHandler, IQueueQuotaComponent queueQuotaComponent)
		{
			if (enhancedDns == null)
			{
				throw new ArgumentNullException("enhancedDns");
			}
			this.agentRuntime = agentRuntime;
			this.mailRouter = mailRouter;
			this.enhancedDns = enhancedDns;
			this.categorizer = categorizer;
			this.certificateCache = certificateCache;
			this.certificateValidator = certificateValidator;
			this.memberOfResolver = memberOfResolver;
			this.messageThrottlingManager = messageThrottlingManager;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.mailItemStorage = mailItemStorage;
			this.serverVersion = this.ServerConfiguration.AdminDisplayVersion;
			this.queueQuotaComponent = queueQuotaComponent;
			if (this.shadowRedundancyManager != null)
			{
				this.shadowRedundancyManager.SetDelayedAckCompletedHandler(new DelayedAckItemHandler(SmtpInSession.DelayedAckCompletedCallback));
			}
			this.smtpOutConnectionHandler = smtpOutConnectionHandler;
			this.proxyHubSelector = proxyHubSelector;
			this.inboundTlsIPConnectionTable = new IPConnectionTable(this.configuration.LocalServer.TransportServer.MaxReceiveTlsRatePerMinute);
			this.ipv6ReceiveConnectionThrottlingEnabled = this.transportAppConfig.SmtpReceiveConfiguration.Ipv6ReceiveConnectionThrottlingEnabled;
			this.receiveTlsThrottlingEnabled = this.transportAppConfig.SmtpReceiveConfiguration.ReceiveTlsThrottlingEnabled;
		}

		public void SetLoadTimeDependencies(IProtocolLog protocolLog, ITransportAppConfig transportAppConfig, ITransportConfiguration configuration)
		{
			this.ReceiveConfiguration = SmtpReceiveConfiguration.Create(transportAppConfig, configuration);
			this.protocolLogBufferSize = transportAppConfig.Logging.SmtpRecvLogBufferSize;
			this.protocolLogStreamFlushInterval = transportAppConfig.Logging.SmtpRecvLogFlushInterval;
			this.protocolLogAsyncInterval = transportAppConfig.Logging.SmtpRecvLogAsyncInterval;
			this.protocolLog = protocolLog;
			this.transportAppConfig = transportAppConfig;
			this.configuration = configuration;
		}

		public void Load()
		{
			this.CreateBlindProxyPerfCounters();
			this.ConfigureProtocolLog(this.ServerConfiguration);
			this.ReconfigureTransportServer(this.configuration.LocalServer);
			this.ReconfigureConnectors(this.configuration.LocalReceiveConnectors);
			this.configuration.LocalServerChanged += this.ReconfigureTransportServer;
			this.configuration.LocalReceiveConnectorsChanged += this.ReconfigureConnectors;
		}

		public void Unload()
		{
			this.configuration.LocalServerChanged -= this.ReconfigureTransportServer;
			this.configuration.LocalReceiveConnectorsChanged -= this.ReconfigureConnectors;
		}

		public void NonGracefullyCloseTcpListener()
		{
			if (this.tcpListener != null)
			{
				this.tcpListener.StopListening();
			}
		}

		public void Initialize(TcpListener.HandleFailure failureDelegate = null, TcpListener.HandleConnection connectionHandler = null)
		{
			this.ReconfigureTransportServer(this.configuration.LocalServer);
			if (failureDelegate != null && connectionHandler != null)
			{
				this.tcpListener = new TcpListener(failureDelegate, connectionHandler, null, ExTraceGlobals.SmtpReceiveTracer, this.eventLogger, this.maxConnectionRate, this.transportAppConfig.SmtpReceiveConfiguration.ExclusiveAddressUse, this.transportAppConfig.SmtpReceiveConfiguration.DisableHandleInheritance);
			}
			this.ReconfigureConnectors(this.configuration.LocalReceiveConnectors);
			if (this.SelfListening)
			{
				this.tcpListener.StartListening(true);
			}
		}

		public void Shutdown()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "Shutdown called");
			while (this.sessionsPendingCreation > 0)
			{
				Thread.Sleep(100);
			}
			if (this.SelfListening)
			{
				this.tcpListener.ProcessStopping = true;
			}
			this.sessions.StartShuttingDown();
			if (this.SelfListening)
			{
				this.tcpListener.StopListening();
				this.tcpListener.Shutdown();
				try
				{
					this.configUpdateLock.EnterWriteLock();
					this.tcpListener = null;
					if (this.bindings != null)
					{
						this.bindings.Clear();
					}
				}
				finally
				{
					try
					{
						this.configUpdateLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			this.sessions.ShutdownAllSessionsAndBlockUntilComplete(this.transportAppConfig.SmtpReceiveConfiguration.WaitForSmtpSessionsAtShutdown);
			this.protocolLog.Close();
		}

		public INetworkConnection CreateNetworkConnection(Socket socket, int receiveBufferSize)
		{
			ArgumentValidator.ThrowIfNull("socket", socket);
			return new NetworkConnection(socket, receiveBufferSize);
		}

		public bool HandleConnection(INetworkConnection connection)
		{
			try
			{
				Interlocked.Increment(ref this.sessionsPendingCreation);
				SmtpReceiveConnectorStub smtpReceiveConnectorStub = null;
				connection.MaxLineLength = 4000;
				if (this.connectorMap != null)
				{
					if (connection.LocalEndPoint.Address.Equals(IPAddress.Any) || connection.RemoteEndPoint.Address.Equals(IPAddress.Any))
					{
						return false;
					}
					try
					{
						this.configUpdateLock.EnterReadLock();
						smtpReceiveConnectorStub = this.connectorMap.Lookup(connection.LocalEndPoint.Address, connection.LocalEndPoint.Port, connection.RemoteEndPoint.Address);
					}
					finally
					{
						try
						{
							this.configUpdateLock.ExitReadLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				if (smtpReceiveConnectorStub == null || smtpReceiveConnectorStub.Connector == null)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "Mapped to connector: not found");
				}
				else
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string>((long)this.GetHashCode(), "Mapped to connector: Id={0}", smtpReceiveConnectorStub.Connector.Name);
				}
				ISmtpInSession smtpInSession = new SmtpInSession(connection, this, smtpReceiveConnectorStub, this.protocolLog, this.eventLogger, this.agentRuntime, this.mailRouter, this.enhancedDns, this.memberOfResolver, this.messageThrottlingManager, this.shadowRedundancyManager, this.transportAppConfig, this.configuration, this.queueQuotaComponent, this.authzAuthorization, this.smtpmessageContextBlob);
				if (this.sessions.TryAdd(connection.ConnectionId, smtpInSession))
				{
					smtpInSession.Start();
				}
				else
				{
					smtpInSession.Shutdown();
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.sessionsPendingCreation);
			}
			return true;
		}

		public void AddDiagnosticInfo(DiagnosableParameters parameters, XElement element)
		{
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag2)
			{
				element.Add(new XElement("help", "Supported arguments: verbose, help."));
				return;
			}
			if (this.inboundProxyDestinationTracker != null)
			{
				XElement xelement;
				if (this.inboundProxyDestinationTracker.TryGetDiagnosticInfo(parameters, out xelement) && xelement != null)
				{
					element.Add(xelement);
				}
				if (this.inboundProxyAccountForestTracker.TryGetDiagnosticInfo(parameters, out xelement) && xelement != null)
				{
					element.Add(xelement);
				}
			}
			List<ISmtpInSession> list = this.sessions.TakeSnapshot();
			if (flag)
			{
				XElement xelement2 = new XElement("Sessions");
				element.Add(xelement2);
				using (List<ISmtpInSession>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ISmtpInSession smtpInSession = enumerator.Current;
						XElement xelement3 = new XElement("Session");
						xelement2.Add(xelement3);
						xelement3.SetAttributeValue("LocalEndPoint", smtpInSession.NetworkConnection.LocalEndPoint);
						xelement3.SetAttributeValue("RemoteEndPoint", smtpInSession.NetworkConnection.RemoteEndPoint);
						xelement3.SetAttributeValue("HelloSmtpDomain", smtpInSession.HelloSmtpDomain ?? "null");
						xelement3.SetAttributeValue("SessionStartTime", smtpInSession.SessionStartTime);
						xelement3.SetAttributeValue("SessionId", smtpInSession.SessionId.ToString("X"));
						xelement3.SetAttributeValue("RemoteIdentityName", smtpInSession.RemoteIdentityName ?? "null");
						xelement3.SetAttributeValue("Breadcrumbs", smtpInSession.Breadcrumbs.ToString().Replace("\r\n", ";"));
					}
					return;
				}
			}
			XElement xelement4 = new XElement("Sessions");
			element.Add(xelement4);
			xelement4.SetAttributeValue("NumSessions", list.Count);
		}

		public void RemoveConnection(long id)
		{
			this.sessions.Remove(id);
		}

		private static SmtpProxyPerfCountersWrapper GetSmtpProxyPerfCounterInstance(string connectorName)
		{
			return new SmtpProxyPerfCountersWrapper(connectorName);
		}

		private static OutboundProxyBySourceTracker GetPerResourceForestOutboundProxyTrackerInstance()
		{
			return new OutboundProxyBySourceTracker(Components.TransportAppConfig.SmtpOutboundProxyConfiguration.ResourceForestMatchingDomains);
		}

		private void CreateBlindProxyPerfCounters()
		{
			if (this.Configuration.ProcessTransportRole == ProcessTransportRole.FrontEnd)
			{
				this.clientProxyPerfCounters = SmtpInServer.GetSmtpProxyPerfCounterInstance("Client submission proxy");
				this.outboundProxyPerfCounters = SmtpInServer.GetSmtpProxyPerfCounterInstance("Outbound proxy");
				this.outboundProxyBySourceTracker = SmtpInServer.GetPerResourceForestOutboundProxyTrackerInstance();
			}
		}

		private void ConfigureProtocolLog(Server serverConfig)
		{
			this.protocolLog.Configure(serverConfig.ReceiveProtocolLogPath, serverConfig.ReceiveProtocolLogMaxAge, serverConfig.ReceiveProtocolLogMaxDirectorySize, serverConfig.ReceiveProtocolLogMaxFileSize, this.protocolLogBufferSize, this.protocolLogStreamFlushInterval, this.protocolLogAsyncInterval);
		}

		private void TcpListenerSetBindings()
		{
			IPEndPoint[] newBindings;
			try
			{
				this.configUpdateLock.EnterReadLock();
				newBindings = ((this.bindings != null) ? this.bindings.ToArray() : null);
			}
			finally
			{
				try
				{
					this.configUpdateLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			this.tcpListener.SetBindings(newBindings, false);
		}

		private void ReconfigureTransportServer(TransportServerConfiguration config)
		{
			this.ConfigureProtocolLog(config.TransportServer);
			this.maxConnectionRate = config.TransportServer.MaxConnectionRatePerMinute;
			if (this.SelfListening)
			{
				this.tcpListener.MaxConnectionRate = this.maxConnectionRate;
			}
			if (this.mailboxDeliveryReceiveConnector != null)
			{
				this.mailboxDeliveryReceiveConnector.ApplyLocalServerConfiguration(config.TransportServer);
			}
		}

		private void ReconfigureConnectors(ReceiveConnectorConfiguration connectorConfig)
		{
			ServerRole serverRole;
			switch (this.configuration.ProcessTransportRole)
			{
			case ProcessTransportRole.Hub:
				serverRole = ServerRole.HubTransport;
				break;
			case ProcessTransportRole.Edge:
				serverRole = ServerRole.HubTransport;
				break;
			case ProcessTransportRole.FrontEnd:
				serverRole = ServerRole.FrontendTransport;
				break;
			case ProcessTransportRole.MailboxSubmission:
			case ProcessTransportRole.MailboxDelivery:
				serverRole = ServerRole.Mailbox;
				break;
			default:
				throw new InvalidOperationException("invalid value for ProcessTransportRole");
			}
			List<ReceiveConnector> list = new List<ReceiveConnector>();
			foreach (ReceiveConnector receiveConnector in connectorConfig.Connectors)
			{
				if ((receiveConnector.TransportRole & serverRole) > ServerRole.None)
				{
					list.Add(receiveConnector);
				}
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "Number of SMTP Receive Connectors that apply to this process ({0}) = {1}, out of a total of {2}", this.configuration.ProcessTransportRole.ToString(), list.Count, connectorConfig.Connectors.Count);
			if (this.configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				if (this.mailboxDeliveryReceiveConnector == null)
				{
					this.mailboxDeliveryReceiveConnector = new MailboxDeliveryReceiveConnector(string.Format(CultureInfo.InvariantCulture, "Default Mailbox Delivery {0}", new object[]
					{
						Environment.MachineName
					}), this.configuration.LocalServer.TransportServer, this.transportAppConfig.SmtpReceiveConfiguration.MailboxDeliveryAcceptAnonymousUsers);
				}
				list.Add(this.mailboxDeliveryReceiveConnector);
			}
			if (list.Count == 0)
			{
				try
				{
					this.configUpdateLock.EnterWriteLock();
					this.connectorMap = null;
					this.connectorStubList = null;
					this.bindings = null;
				}
				finally
				{
					try
					{
						this.configUpdateLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "The list of connectors is empty. All bindings are closed.");
			}
			else
			{
				SmtpInConnectorMap<SmtpReceiveConnectorStub> newConnectorMap;
				List<SmtpReceiveConnectorStub> newConnectorStubList;
				List<IPEndPoint> newBindings;
				this.ProcessReceiveConnectors(list, out newConnectorMap, out newConnectorStubList, out newBindings);
				this.ConfigureInboundProxyDestinationTrackers(list);
				this.UpdateReceiveConnectors(newConnectorMap, newConnectorStubList, newBindings);
			}
			if (this.SelfListening)
			{
				this.TcpListenerSetBindings();
			}
		}

		private void ConfigureInboundProxyDestinationTrackers(IEnumerable<ReceiveConnector> receiveConnectors)
		{
			ArgumentValidator.ThrowIfNull("receiveConnectors", receiveConnectors);
			TransportAppConfig.SmtpInboundProxyConfig smtpInboundProxyConfiguration = this.transportAppConfig.SmtpInboundProxyConfiguration;
			if (this.inboundProxyDestinationTracker == null)
			{
				this.inboundProxyDestinationTracker = new InboundProxyDestinationTracker(InboundProxyTrackerType.InboundProxyDestinationTracker, smtpInboundProxyConfiguration.InboundProxyDestinationTrackingEnabled, smtpInboundProxyConfiguration.RejectBasedOnInboundProxyDestinationTrackingEnabled, smtpInboundProxyConfiguration.PerDestinationConnectionPercentageThreshold, new InboundProxyDestinationTracker.TryGetDestinationConnectionThresholdDelegate(smtpInboundProxyConfiguration.TryGetDestinationConnectionThreshold), (string instanceName) => InboundProxyDestinationPerfCounters.GetInstance(instanceName).ConnectionsCurrent, (string instanceName) => InboundProxyDestinationPerfCounters.GetInstance(instanceName).ConnectionsTotal, new ExEventLogWrapper(this.eventLogger), smtpInboundProxyConfiguration.InboundProxyDestinationTrackerLogInterval, receiveConnectors);
			}
			else
			{
				this.inboundProxyDestinationTracker.UpdateReceiveConnectors(receiveConnectors);
			}
			if (this.inboundProxyAccountForestTracker == null)
			{
				this.inboundProxyAccountForestTracker = new InboundProxyDestinationTracker(InboundProxyTrackerType.InboundProxyAccountForestTracker, smtpInboundProxyConfiguration.InboundProxyAccountForestTrackingEnabled, smtpInboundProxyConfiguration.RejectBasedOnInboundProxyAccountForestTrackingEnabled, smtpInboundProxyConfiguration.PerAccountForestConnectionPercentageThreshold, new InboundProxyDestinationTracker.TryGetDestinationConnectionThresholdDelegate(smtpInboundProxyConfiguration.TryGetAccountForestConnectionThreshold), (string instanceName) => InboundProxyAccountForestPerfCounters.GetInstance(instanceName).ConnectionsCurrent, (string instanceName) => InboundProxyAccountForestPerfCounters.GetInstance(instanceName).ConnectionsTotal, new ExEventLogWrapper(this.eventLogger), smtpInboundProxyConfiguration.InboundProxyDestinationTrackerLogInterval, receiveConnectors);
				return;
			}
			this.inboundProxyAccountForestTracker.UpdateReceiveConnectors(receiveConnectors);
		}

		private void UpdateReceiveConnectors(SmtpInConnectorMap<SmtpReceiveConnectorStub> newConnectorMap, List<SmtpReceiveConnectorStub> newConnectorStubList, List<IPEndPoint> newBindings)
		{
			try
			{
				this.configUpdateLock.EnterWriteLock();
				List<SmtpReceiveConnectorStub> list = this.connectorStubList;
				this.connectorMap = newConnectorMap;
				this.connectorStubList = newConnectorStubList;
				this.bindings = newBindings;
				if (list != null)
				{
					foreach (SmtpReceiveConnectorStub smtpReceiveConnectorStub in list)
					{
						foreach (SmtpReceiveConnectorStub smtpReceiveConnectorStub2 in this.connectorStubList)
						{
							if (smtpReceiveConnectorStub.Connector.Identity.Equals(smtpReceiveConnectorStub2.Connector.Identity))
							{
								smtpReceiveConnectorStub2.ConnectionTable = smtpReceiveConnectorStub.ConnectionTable;
								break;
							}
						}
					}
				}
			}
			finally
			{
				try
				{
					this.configUpdateLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ConfiguredConnectors, null, null);
		}

		private void ProcessReceiveConnectors(List<ReceiveConnector> connectors, out SmtpInConnectorMap<SmtpReceiveConnectorStub> newConnectorMap, out List<SmtpReceiveConnectorStub> newConnectorStubList, out List<IPEndPoint> newBindings)
		{
			newConnectorMap = new SmtpInConnectorMap<SmtpReceiveConnectorStub>();
			newConnectorStubList = new List<SmtpReceiveConnectorStub>();
			newBindings = new List<IPEndPoint>();
			foreach (ReceiveConnector receiveConnector in connectors)
			{
				if (receiveConnector.Enabled)
				{
					foreach (IPBinding ipbinding in receiveConnector.Bindings)
					{
						newBindings.Add(new IPEndPoint(ipbinding.Address, ipbinding.Port));
					}
					SmtpReceiveConnectorStub smtpReceiveConnectorStub = new SmtpReceiveConnectorStub(receiveConnector, Util.CreateReceivePerfCounters(receiveConnector, this.configuration.ProcessTransportRole), Util.GetOrCreateAvailabilityPerfCounters(this.cachedAvailabilityPerfCounters, receiveConnector, this.configuration.ProcessTransportRole, this.transportAppConfig.SmtpAvailabilityConfiguration.SmtpAvailabilityMinConnectionsToMonitor));
					newConnectorMap.AddEntry(receiveConnector.Bindings.ToArray(), receiveConnector.RemoteIPRanges.ToArray(), smtpReceiveConnectorStub);
					newConnectorStubList.Add(smtpReceiveConnectorStub);
				}
			}
		}

		private ServiceState targetRunningState;

		private IPConnectionTable inboundTlsIPConnectionTable;

		private bool ipv6ReceiveConnectionThrottlingEnabled;

		private bool receiveTlsThrottlingEnabled;

		private MailboxDeliveryReceiveConnector mailboxDeliveryReceiveConnector;

		private readonly ExEventLog eventLogger;

		private IInboundProxyDestinationTracker inboundProxyDestinationTracker;

		private IInboundProxyDestinationTracker inboundProxyAccountForestTracker;

		private ICategorizer categorizer;

		private ISmtpInMailItemStorage mailItemStorage;

		private IProtocolLog protocolLog;

		private IMailRouter mailRouter;

		private IEnhancedDns enhancedDns;

		private ICertificateCache certificateCache;

		private ICertificateValidator certificateValidator;

		private IShadowRedundancyManager shadowRedundancyManager;

		private IMessageThrottlingManager messageThrottlingManager;

		private IIsMemberOfResolver<RoutingAddress> memberOfResolver;

		private ITransportAppConfig transportAppConfig;

		private ITransportConfiguration configuration;

		private IQueueQuotaComponent queueQuotaComponent;

		private IAgentRuntime agentRuntime;

		private DateTime currentTime;

		private Version serverVersion;

		private readonly SmtpSessions sessions = new SmtpSessions();

		private SmtpInConnectorMap<SmtpReceiveConnectorStub> connectorMap;

		private List<SmtpReceiveConnectorStub> connectorStubList;

		private readonly ReaderWriterLockSlim configUpdateLock;

		private TcpListener tcpListener;

		private List<IPEndPoint> bindings = new List<IPEndPoint>();

		private int maxConnectionRate = 1200;

		private int sessionsPendingCreation;

		private readonly ConcurrentDictionary<string, ISmtpAvailabilityPerfCounters> cachedAvailabilityPerfCounters = new ConcurrentDictionary<string, ISmtpAvailabilityPerfCounters>();

		private int protocolLogBufferSize;

		private TimeSpan protocolLogStreamFlushInterval;

		private TimeSpan protocolLogAsyncInterval;

		private SmtpOutConnectionHandler smtpOutConnectionHandler;

		private SmtpProxyPerfCountersWrapper clientProxyPerfCounters;

		private SmtpProxyPerfCountersWrapper outboundProxyPerfCounters;

		private OutboundProxyBySourceTracker outboundProxyBySourceTracker;

		private IProxyHubSelector proxyHubSelector;

		private readonly IAuthzAuthorization authzAuthorization = new AuthzAuthorizationWrapper();

		private readonly ISmtpMessageContextBlob smtpmessageContextBlob = new SmtpMessageContextBlobWrapper();

		private readonly IEventNotificationItem eventNotificationItem = new EventNotificationItemWrapper();
	}
}
