using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpOutConnectionHandler : ISmtpOutConnectionHandler, IStartableTransportComponent, ITransportComponent, IDiagnosable
	{
		public static SmtpOutSessionCache SessionCache
		{
			get
			{
				return SmtpOutConnectionHandler.sessionCache;
			}
		}

		public string CurrentState
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(512);
				stringBuilder.AppendLine("Connections=" + SmtpOutConnectionHandler.connections.Count.ToString());
				lock (SmtpOutConnectionHandler.SyncRoot)
				{
					foreach (SmtpOutConnection smtpOutConnection in SmtpOutConnectionHandler.connections.Values)
					{
						stringBuilder.Append(smtpOutConnection.GetSessionAndConnectionInfo());
					}
				}
				return stringBuilder.ToString();
			}
		}

		private static object SyncRoot
		{
			get
			{
				return SmtpOutConnectionHandler.connections;
			}
		}

		public BufferCache BufferCache
		{
			get
			{
				return this.bufferCache;
			}
		}

		public static bool AddConnection(ulong id, SmtpOutConnection smtpOutConnection)
		{
			bool result;
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				if (Components.ShuttingDown)
				{
					result = false;
				}
				else
				{
					TransportHelpers.AttemptAddToDictionary<ulong, SmtpOutConnection>(SmtpOutConnectionHandler.connections, id, smtpOutConnection, null);
					result = true;
				}
			}
			return result;
		}

		public static bool UpdateConnection(ulong id, NetworkConnection nc)
		{
			bool result;
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				if (Components.ShuttingDown)
				{
					result = false;
				}
				else
				{
					SmtpOutConnection smtpOutConnection = SmtpOutConnectionHandler.connections[id];
					smtpOutConnection.UpdateSession(nc);
					result = true;
				}
			}
			return result;
		}

		public static bool ReplaceConnectionID(ulong previousID, ulong newID)
		{
			bool result;
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				if (Components.ShuttingDown)
				{
					result = false;
				}
				else
				{
					SmtpOutConnection valueToAdd = SmtpOutConnectionHandler.connections[previousID];
					SmtpOutConnectionHandler.connections.Remove(previousID);
					TransportHelpers.AttemptAddToDictionary<ulong, SmtpOutConnection>(SmtpOutConnectionHandler.connections, newID, valueToAdd, null);
					result = true;
				}
			}
			return result;
		}

		public static void RemoveConnection(ulong id)
		{
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				if (!SmtpOutConnectionHandler.connections.Remove(id))
				{
					throw new ArgumentException(string.Format("Session {0} not found", id), "id");
				}
			}
		}

		public void SetRunTimeDependencies(EnhancedDns enhancedDns, UnhealthyTargetFilterComponent unhealthyTargetFilter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager)
		{
			this.enhancedDns = enhancedDns;
			this.unhealthyTargetFilter = unhealthyTargetFilter;
			this.certificateCache = certificateCache;
			this.certificateValidator = certificateValidator;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.runTimeDependenciesSet = true;
		}

		public void SetLoadTimeDependencies(IMailRouter mailRouter, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, ISmtpInComponent smtpInComponent, Components.LoggingComponent loggingComponent)
		{
			this.mailRouter = mailRouter;
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
			this.smtpInComponent = smtpInComponent;
			this.loggingComponent = loggingComponent;
			this.loadTimeDependenciesSet = true;
		}

		public void Load()
		{
			if (!this.loadTimeDependenciesSet)
			{
				throw new InvalidOperationException("load-time dependencies should be set before calling Load()");
			}
			this.Configure();
			Components.ConfigChanged += this.ConfigUpdate;
			this.mailRouter.RoutingTablesChanged += this.SendConnectorsUpdate;
			this.SendConnectorsUpdate(this.mailRouter, DateTime.MinValue, true);
			this.AddPerInMemoryConnectorPerfCounters();
		}

		public void Unload()
		{
			if (SmtpOutConnectionHandler.connections.Count > 0)
			{
				throw new InvalidOperationException(string.Format("We should not have any smtp out connections; we have {0}.", SmtpOutConnectionHandler.connections.Count));
			}
			Components.ConfigChanged -= this.ConfigUpdate;
			this.mailRouter.RoutingTablesChanged -= this.SendConnectorsUpdate;
		}

		public string OnUnhandledException(Exception e)
		{
			this.FlushProtocolLog();
			return null;
		}

		public void FlushProtocolLog()
		{
			if (this.loggingComponent != null)
			{
				this.loggingComponent.SmtpSendLog.Flush();
			}
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			if (!this.runTimeDependenciesSet)
			{
				throw new InvalidOperationException("run-time dependencies should be set before calling Start()");
			}
			TlsCredentialCache.Initialize(1000, 3);
		}

		public void Stop()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug(0L, "Shutdown called");
			TlsCredentialCache.Shutdown();
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				foreach (SmtpOutConnection smtpOutConnection in SmtpOutConnectionHandler.connections.Values)
				{
					smtpOutConnection.Shutdown();
				}
			}
			SmtpOutConnectionHandler.SessionCache.RemoveAll(ConnectionCacheRemovalType.ShutDown);
			while (SmtpOutConnectionHandler.connections.Count > 0)
			{
				Thread.Sleep(1000);
			}
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "SmtpOut";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			if (flag2)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, help."));
			}
			else
			{
				UnhealthyTargetFilterComponent unhealthyTargetFilterComponent = this.unhealthyTargetFilter;
				XElement xelement2 = new XElement("UnhealthyTargetFilter");
				xelement.Add(xelement2);
				XElement xelement3 = new XElement("Configuration");
				XElement xelement4 = new XElement("UnhealthyTargetFqdnFilter");
				XElement xelement5 = new XElement("UnhealthyTargetIpAddressFilter");
				XElement xelement6 = new XElement("InboundProxyCache");
				xelement2.Add(xelement3);
				xelement2.Add(xelement4);
				xelement2.Add(xelement5);
				xelement2.Add(xelement6);
				UnhealthyTargetFilterConfiguration unhealthyTargetFilterConfiguration = (unhealthyTargetFilterComponent != null) ? unhealthyTargetFilterComponent.UnhealthyTargetFilterConfiguration : null;
				UnhealthyTargetFilter<IPAddressPortPair> unhealthyTargetFilter = (unhealthyTargetFilterComponent != null) ? unhealthyTargetFilterComponent.UnhealthyTargetIPAddressFilter : null;
				UnhealthyTargetFilter<FqdnPortPair> unhealthyTargetFilter2 = (unhealthyTargetFilterComponent != null) ? unhealthyTargetFilterComponent.UnhealthyTargetFqdnFilter : null;
				if (unhealthyTargetFilterConfiguration != null)
				{
					unhealthyTargetFilterConfiguration.AddDiagnosticInfoTo(xelement3);
				}
				if (unhealthyTargetFilter2 != null)
				{
					unhealthyTargetFilter2.AddDiagnosticInfoTo(xelement4, flag);
				}
				if (unhealthyTargetFilter != null)
				{
					unhealthyTargetFilter.AddDiagnosticInfoTo(xelement5, flag);
				}
				this.bufferCache.AddDiagnosticInfoTo(xelement6, flag);
				if (SmtpOutConnectionHandler.sessionCache != null)
				{
					XElement xelement7 = new XElement("SmtpOutSessionCache");
					SmtpOutConnectionHandler.sessionCache.AddDiagnosticInfoTo(xelement7, flag);
					xelement2.Add(xelement7);
				}
			}
			return xelement;
		}

		public void HandleConnection(NextHopConnection connection)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Initiating new outbound connection");
			SmtpOutConnection smtpOutConnection = new SmtpOutConnection(connection, this.loggingComponent.SmtpSendLog, this.mailRouter, this.enhancedDns, this.unhealthyTargetFilter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, this.smtpInComponent, RiskLevel.Normal, 0);
			smtpOutConnection.Connect();
		}

		public void HandleProxyConnection(NextHopConnection connection, IEnumerable<INextHopServer> proxyDestinations, bool internalDestination, string connectionContextString)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Initiating new outbound connection for {0} proxy destination(s)", internalDestination ? "internal" : "external");
			SmtpOutConnection smtpOutConnection = new SmtpOutConnection(connection, this.loggingComponent.SmtpSendLog, this.mailRouter, this.enhancedDns, this.unhealthyTargetFilter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, this.smtpInComponent, RiskLevel.Normal, 0, this.transportAppConfig.SmtpInboundProxyConfiguration.PerHostConnectionAttempts, connectionContextString);
			smtpOutConnection.ConnectToPerMessageProxyDestinations(proxyDestinations, internalDestination);
		}

		public SmtpOutConnection NewBlindProxyConnection(NextHopConnection connection, IEnumerable<INextHopServer> proxyDestinations, bool clientProxy, SmtpSendConnectorConfig connector, TlsSendConfiguration tlsSendConfiguration, RiskLevel riskLevel, int outboundIPPool, int connectionAttempts, ISmtpInSession inSession, string connectionContextString)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Initiating new outbound connection for blind {0} proxy destination(s)", clientProxy ? "client" : "outbound");
			return new SmtpOutConnection(connection, this.loggingComponent.SmtpSendLog, this.mailRouter, this.enhancedDns, this.unhealthyTargetFilter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, this.smtpInComponent, riskLevel, outboundIPPool, connectionAttempts, 1, clientProxy, tlsSendConfiguration, inSession, false, connectionContextString);
		}

		public void HandleShadowConnection(NextHopConnection connection, IEnumerable<INextHopServer> shadowHubs)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Initiating new outbound shadow connection");
			SmtpOutConnection smtpOutConnection = new SmtpOutConnection(connection, this.loggingComponent.SmtpSendLog, this.mailRouter, this.enhancedDns, this.unhealthyTargetFilter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, this.smtpInComponent, RiskLevel.Normal, 0, 0, 1, false, null, null, true, null);
			smtpOutConnection.ConnectToShadowDestinations(shadowHubs);
		}

		internal static SmtpSendPerfCountersInstance GetSmtpSendPerfCounterInstance(string connectorName)
		{
			SmtpSendPerfCounters.SetCategoryName(SmtpOutConnectionHandler.perfCounterCategoryMap[Components.Configuration.ProcessTransportRole]);
			return SmtpSendPerfCounters.GetInstance(connectorName);
		}

		private static void AddPerConnectorPerfCounters(IList<SmtpSendConnectorConfig> sendConnectorCollection)
		{
			foreach (SmtpSendConnectorConfig smtpSendConnectorConfig in sendConnectorCollection)
			{
				SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(smtpSendConnectorConfig.Name);
			}
		}

		private void AddPerInMemoryConnectorPerfCounters()
		{
			if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.Hub)
			{
				SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(Strings.IntraorgSendConnectorName);
				return;
			}
			if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd)
			{
				SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(Strings.ExternalDestinationInboundProxySendConnector);
				SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(Strings.InternalDestinationInboundProxySendConnector);
				return;
			}
			if (this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery || this.transportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission)
			{
				SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(Strings.MailboxProxySendConnector);
			}
		}

		private void SendConnectorsUpdate(IMailRouter routing, DateTime newRoutingTablesTimestamp, bool routesChanged)
		{
			if (routing == null)
			{
				return;
			}
			if (this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.Hub && this.transportConfiguration.ProcessTransportRole != ProcessTransportRole.Edge)
			{
				return;
			}
			IList<SmtpSendConnectorConfig> localSendConnectors = routing.GetLocalSendConnectors<SmtpSendConnectorConfig>();
			SmtpOutConnectionHandler.AddPerConnectorPerfCounters(localSendConnectors);
			SmtpOutConnectionHandler.SessionCache.RemoveAll(ConnectionCacheRemovalType.ConfigChange);
			lock (SmtpOutConnectionHandler.SyncRoot)
			{
				if (newRoutingTablesTimestamp > SmtpOutConnectionHandler.currentRoutingTablesTimestamp)
				{
					if (SmtpOutConnectionHandler.currentRoutingTablesTimestamp != DateTime.MinValue)
					{
						ExTraceGlobals.SmtpSendTracer.TraceDebug(0L, "Routing table changed, notify connections for DNS update.");
						foreach (SmtpOutConnection smtpOutConnection in SmtpOutConnectionHandler.connections.Values)
						{
							smtpOutConnection.RoutingTableUpdate();
						}
					}
					if (newRoutingTablesTimestamp != DateTime.MaxValue)
					{
						SmtpOutConnectionHandler.currentRoutingTablesTimestamp = newRoutingTablesTimestamp;
					}
				}
			}
		}

		private void ConfigUpdate(object source, EventArgs args)
		{
			this.SendConnectorsUpdate(this.mailRouter, DateTime.MaxValue, false);
		}

		private void Configure()
		{
			int maxEntriesForOutboundProxy = 0;
			int maxEntriesForNonOutboundProxy = 0;
			TimeSpan connectionTimeoutForOutboundProxy = this.transportAppConfig.ConnectionCacheConfig.ConnectionTimeoutForOutboundProxy;
			TimeSpan connectionInactivityTimeout = this.transportAppConfig.ConnectionCacheConfig.ConnectionInactivityTimeout;
			if (this.transportAppConfig.ConnectionCacheConfig.EnableConnectionCache)
			{
				maxEntriesForOutboundProxy = this.transportAppConfig.ConnectionCacheConfig.ConnectionCacheMaxNumberOfEntriesForOutboundProxy;
				maxEntriesForNonOutboundProxy = this.transportAppConfig.ConnectionCacheConfig.ConnectionCacheMaxNumberOfEntriesForNonOutboundProxy;
			}
			SmtpOutConnectionHandler.sessionCache = new SmtpOutSessionCache(maxEntriesForOutboundProxy, maxEntriesForNonOutboundProxy, connectionTimeoutForOutboundProxy, connectionInactivityTimeout, new SmtpOutSessionCache.ConnectionCachePerfCounters(this.transportConfiguration.ProcessTransportRole, "OutboundProxyConnectionCache"), new SmtpOutSessionCache.ConnectionCachePerfCounters(this.transportConfiguration.ProcessTransportRole, "NonOutboundProxyConnectionCache"));
		}

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.SmtpSendTracer.Category, TransportEventLog.GetEventSource());

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport SmtpSend"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport SmtpSend"
			},
			{
				ProcessTransportRole.FrontEnd,
				"MSExchangeFrontEndTransport SmtpSend"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery SmtpSend"
			},
			{
				ProcessTransportRole.MailboxSubmission,
				"MSExchange Submission SmtpSend"
			}
		};

		private static Dictionary<ulong, SmtpOutConnection> connections = new Dictionary<ulong, SmtpOutConnection>();

		private static SmtpOutSessionCache sessionCache;

		private static DateTime currentRoutingTablesTimestamp = DateTime.MinValue;

		private Components.LoggingComponent loggingComponent;

		private IMailRouter mailRouter;

		private EnhancedDns enhancedDns;

		private UnhealthyTargetFilterComponent unhealthyTargetFilter;

		private CertificateCache certificateCache;

		private CertificateValidator certificateValidator;

		private ShadowRedundancyManager shadowRedundancyManager;

		private TransportAppConfig transportAppConfig;

		private ITransportConfiguration transportConfiguration;

		private ISmtpInComponent smtpInComponent;

		private bool loadTimeDependenciesSet;

		private bool runTimeDependenciesSet;

		private BufferCache bufferCache = new BufferCache(1000);
	}
}
