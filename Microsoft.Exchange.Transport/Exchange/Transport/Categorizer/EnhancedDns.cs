using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EnhancedDns : Dns, ITransportComponent, IDiagnosable, IEnhancedDns
	{
		public SmtpSendConnectorConfig EnterpriseRelayConnector
		{
			get
			{
				return this.enterpriseRelayConnector;
			}
		}

		public SmtpSendConnectorConfig ClientProxyConnector
		{
			get
			{
				return this.clientProxyConnector;
			}
		}

		private static ExEventLog Logger
		{
			get
			{
				if (EnhancedDns.eventLogger == null)
				{
					ExEventLog value = new ExEventLog(ExTraceGlobals.RoutingTracer.Category, TransportEventLog.GetEventSource());
					Interlocked.CompareExchange<ExEventLog>(ref EnhancedDns.eventLogger, value, null);
				}
				return EnhancedDns.eventLogger;
			}
		}

		public static DnsStatus EndResolveToNextHop(IAsyncResult asyncResult, out EnhancedDnsTargetHost[] hosts, out IEnumerable<INextHopServer> destinationServers, out SmtpSendConnectorConfig destinationConnector, out SmtpSendConnectorConfig proxyConnector, out IPAddress reportingServer, out string diagnosticInfo)
		{
			diagnosticInfo = string.Empty;
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null || (!(lazyAsyncResult.AsyncObject is EnhancedDnsRequest) && !(lazyAsyncResult.AsyncObject is int)))
			{
				throw new ArgumentException("Incorrect IAsyncResult value");
			}
			int arg = (lazyAsyncResult.AsyncObject is EnhancedDnsRequest) ? (lazyAsyncResult.AsyncObject as EnhancedDnsRequest).RequestId : ((int)lazyAsyncResult.AsyncObject);
			if (lazyAsyncResult.Result is EnhancedDnsStatusResult)
			{
				EnhancedDnsStatusResult enhancedDnsStatusResult = (EnhancedDnsStatusResult)lazyAsyncResult.Result;
				ExTraceGlobals.RoutingTracer.TraceError<int, DnsStatus>(0L, "Request ID={0}: EndResolve with status '{1}'", arg, enhancedDnsStatusResult.Status);
				hosts = new EnhancedDnsTargetHost[0];
				destinationServers = enhancedDnsStatusResult.RequestContext.DestinationServers;
				destinationConnector = enhancedDnsStatusResult.RequestContext.DestinationConnector;
				proxyConnector = enhancedDnsStatusResult.RequestContext.ProxyConnector;
				reportingServer = enhancedDnsStatusResult.Server;
				diagnosticInfo = enhancedDnsStatusResult.DiagnosticInfo;
				return enhancedDnsStatusResult.Status;
			}
			EnhancedDnsHostsResult enhancedDnsHostsResult = (EnhancedDnsHostsResult)lazyAsyncResult.Result;
			hosts = enhancedDnsHostsResult.Hosts;
			destinationServers = enhancedDnsHostsResult.RequestContext.DestinationServers;
			destinationConnector = enhancedDnsHostsResult.RequestContext.DestinationConnector;
			proxyConnector = enhancedDnsHostsResult.RequestContext.ProxyConnector;
			reportingServer = IPAddress.None;
			ExTraceGlobals.RoutingTracer.TraceDebug<int, int>(0L, "Request ID={0}: EndResolve with {1} hosts", arg, hosts.Length);
			return (DnsStatus)lazyAsyncResult.ErrorCode;
		}

		public void SetRunTimeDependencies(IMailRouter router)
		{
			this.router = router;
		}

		public void Load()
		{
			this.requestCounter = 0;
			this.nextPollTcpipSettings = DateTime.UtcNow + EnhancedDns.TcpipPollInterval;
			NetworkChange.NetworkAddressChanged += this.HandleAddressChange;
			Components.Configuration.LocalServerChanged += this.HandleTransportServerConfigChange;
			if (!this.LoadConfiguration())
			{
				throw new TransportComponentLoadFailedException("Failed to load local IP address information");
			}
			Components.ConfigChanged += this.HandleConfigChange;
		}

		public void Unload()
		{
			NetworkChange.NetworkAddressChanged -= this.HandleAddressChange;
			Components.Configuration.LocalServerChanged -= this.HandleTransportServerConfigChange;
			Components.ConfigChanged -= this.HandleConfigChange;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void FlushCache()
		{
			DnsServerList serverList = this.internalDnsServerList;
			if (serverList != null)
			{
				serverList.FlushCache();
			}
			serverList = base.ServerList;
			if (serverList != null)
			{
				serverList.FlushCache();
			}
		}

		public IAsyncResult BeginResolveToNextHop(NextHopSolutionKey key, RiskLevel riskLevel, int outboundIPPool, AsyncCallback requestCallback, object stateObject)
		{
			if (this.router == null)
			{
				throw new InvalidOperationException("Router not set for Enhanced DNS");
			}
			this.CheckForTcpInterfaceChanges();
			int num = Interlocked.Increment(ref this.requestCounter);
			ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "New enhanced DNS request ID={0}: NextHopType='{1}'; NextHopDomain='{2}'; Connector='{3}'", new object[]
			{
				num,
				key.NextHopType,
				key.NextHopDomain,
				key.NextHopConnector
			});
			IEnumerable<INextHopServer> enumerable;
			SmtpSendConnectorConfig smtpSendConnectorConfig;
			if (!this.router.TryGetServersForNextHop(key, out enumerable, out smtpSendConnectorConfig))
			{
				return EnhancedDnsRequest.CompleteWithStatus(num, DnsStatus.ConfigChanged, default(EnhancedDnsRequestContext), requestCallback, stateObject);
			}
			IEnumerable<INextHopServer> enumerable2 = null;
			SmtpSendConnectorConfig smtpSendConnectorConfig2 = null;
			EnhancedDnsRequest.QueryType queryType;
			if (smtpSendConnectorConfig == null)
			{
				queryType = EnhancedDnsRequest.QueryType.AQuery;
				smtpSendConnectorConfig = this.enterpriseRelayConnector;
			}
			else if (!smtpSendConnectorConfig.FrontendProxyEnabled)
			{
				queryType = EnhancedDnsRequest.QueryType.MXQuery;
			}
			else
			{
				bool flag;
				if (!this.router.TryGetOutboundFrontendServers(out enumerable2, out flag))
				{
					return EnhancedDnsRequest.CompleteWithStatus(num, DnsStatus.NoOutboundFrontendServers, new EnhancedDnsRequestContext(smtpSendConnectorConfig), requestCallback, stateObject);
				}
				if (flag)
				{
					queryType = EnhancedDnsRequest.QueryType.MXQuery;
					smtpSendConnectorConfig2 = this.outboundProxyExternalConnector;
				}
				else
				{
					queryType = EnhancedDnsRequest.QueryType.AQuery;
					smtpSendConnectorConfig2 = this.outboundProxyInternalConnector;
				}
			}
			EnhancedDnsRequestContext requestContext = new EnhancedDnsRequestContext((enumerable2 == null) ? null : enumerable, smtpSendConnectorConfig, smtpSendConnectorConfig2);
			EnhancedDnsRequest enhancedDnsRequest = new EnhancedDnsRequest(num, enumerable2 ?? enumerable, queryType, key.NextHopType.DeliveryType, (smtpSendConnectorConfig2 != null) ? smtpSendConnectorConfig2 : smtpSendConnectorConfig, riskLevel, outboundIPPool, smtpSendConnectorConfig2 != null, requestContext);
			DnsServerList list = null;
			DnsQueryOptions options = DnsQueryOptions.None;
			this.GetDnsQuerySettings(num, smtpSendConnectorConfig2 ?? smtpSendConnectorConfig, out list, out options);
			return enhancedDnsRequest.Resolve(this, list, options, requestCallback, stateObject);
		}

		public IAsyncResult BeginResolveProxyNextHop(IEnumerable<INextHopServer> destinations, bool internalDestination, SmtpSendConnectorConfig sendConnector, SmtpOutProxyType proxyType, RiskLevel riskLevel, int outboundIPPool, AsyncCallback requestCallback, object stateObject)
		{
			this.CheckForTcpInterfaceChanges();
			int num = Interlocked.Increment(ref this.requestCounter);
			ExTraceGlobals.RoutingTracer.TraceDebug<int, bool, SmtpOutProxyType>((long)this.GetHashCode(), "New enhanced DNS request for proxy ID={0}: internalDestination='{1}', proxyType='{2}'", num, internalDestination, proxyType);
			SmtpSendConnectorConfig smtpSendConnectorConfig = sendConnector;
			EnhancedDnsRequest.QueryType queryType;
			switch (proxyType)
			{
			case SmtpOutProxyType.PerMessage:
				if (smtpSendConnectorConfig != null)
				{
					throw new ArgumentException("PerMessage proxy type cannot be used with a specified connector");
				}
				smtpSendConnectorConfig = (internalDestination ? this.perMessageProxyInternalConnector : this.perMessageProxyExternalConnector);
				queryType = EnhancedDnsRequest.QueryType.AQuery;
				break;
			case SmtpOutProxyType.Blind:
				if (smtpSendConnectorConfig == null)
				{
					throw new ArgumentException("Blind proxy type cannot be used without a specified connector");
				}
				queryType = (internalDestination ? EnhancedDnsRequest.QueryType.AQuery : EnhancedDnsRequest.QueryType.MXQuery);
				break;
			case SmtpOutProxyType.ShadowPeerToPeer:
				if (smtpSendConnectorConfig != null)
				{
					throw new ArgumentException("ShadowPeerToPeer proxy type cannot be used with a specified connector");
				}
				if (!internalDestination)
				{
					throw new ArgumentException("ShadowPeerToPeer must be internal destination");
				}
				smtpSendConnectorConfig = this.enterpriseRelayConnector;
				queryType = EnhancedDnsRequest.QueryType.AQuery;
				break;
			default:
				throw new InvalidOperationException("Illegal proxy type");
			}
			EnhancedDnsRequest enhancedDnsRequest = new EnhancedDnsRequest(num, destinations, queryType, DeliveryType.Undefined, smtpSendConnectorConfig, riskLevel, outboundIPPool, false, new EnhancedDnsRequestContext(smtpSendConnectorConfig));
			DnsServerList list = null;
			DnsQueryOptions options = DnsQueryOptions.None;
			this.GetDnsQuerySettings(num, smtpSendConnectorConfig, out list, out options);
			return enhancedDnsRequest.Resolve(this, list, options, requestCallback, stateObject);
		}

		public void HandleTransportServerConfigChange(TransportServerConfiguration args)
		{
			ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "Transport server config change detected");
			this.LoadConfiguration();
		}

		private static DnsQueryOptions GetDnsOptions(ProtocolOption protocolOption)
		{
			DnsQueryOptions dnsQueryOptions = DnsQueryOptions.None;
			switch (protocolOption)
			{
			case ProtocolOption.UseUdpOnly:
				dnsQueryOptions |= DnsQueryOptions.AcceptTruncatedResponse;
				break;
			case ProtocolOption.UseTcpOnly:
				dnsQueryOptions |= DnsQueryOptions.UseTcpOnly;
				break;
			}
			return dnsQueryOptions;
		}

		private static ADObjectId GetHomeRoutingGroup(Server transportServer)
		{
			if (transportServer.HomeRoutingGroup != null)
			{
				return transportServer.HomeRoutingGroup;
			}
			if (transportServer.Id.Parent != null && transportServer.Id.Parent.Parent != null)
			{
				return transportServer.Id.Parent.Parent.GetChildId(RoutingGroupsContainer.DefaultName).GetChildId(RoutingGroup.DefaultName);
			}
			return transportServer.Id.GetChildId(RoutingGroupsContainer.DefaultName).GetChildId(RoutingGroup.DefaultName);
		}

		private void HandleAddressChange(object sender, EventArgs e)
		{
			ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "Network address change detected");
			this.LoadConfiguration();
		}

		private void HandleConfigChange(object sender, EventArgs e)
		{
			ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "Config change detected");
			this.LoadConfiguration();
		}

		private bool LoadConfiguration()
		{
			Server transportServer = Components.Configuration.LocalServer.TransportServer;
			List<IPAddress> list = null;
			NetworkInformationException ex;
			if (!LocalComputer.TryGetIPAddresses(out list, out ex))
			{
				ExTraceGlobals.RoutingTracer.TraceError<NetworkInformationException>((long)this.GetHashCode(), "Failed GetLocalIPAddresses, {0}", ex);
				EnhancedDns.Logger.LogEvent(TransportEventLogConstants.Tuple_NetworkAdapterIPQueryFailed, null, new object[]
				{
					ex
				});
				return false;
			}
			IPAddress externalIPAddress = transportServer.ExternalIPAddress;
			if (externalIPAddress != null)
			{
				list.Add(externalIPAddress);
			}
			base.LocalIPAddresses = list;
			ExTraceGlobals.RoutingTracer.TraceDebug<List<IPAddress>>((long)this.GetHashCode(), "New local IP addresses: {0}", list);
			base.Options = EnhancedDns.GetDnsOptions(transportServer.ExternalDNSProtocolOption);
			MultiValuedProperty<IPAddress> externalDNSServers = transportServer.ExternalDNSServers;
			if (transportServer.ExternalDNSAdapterEnabled || MultiValuedPropertyBase.IsNullOrEmpty(externalDNSServers))
			{
				base.AdapterServerList(transportServer.ExternalDNSAdapterGuid, Components.TransportAppConfig.RemoteDelivery.ExcludeDnsServersFromLoopbackAdapters, Components.TransportAppConfig.RemoteDelivery.ExcludeIPv6SiteLocalDnsAddresses);
			}
			else
			{
				IPAddress[] array = new IPAddress[externalDNSServers.Count];
				externalDNSServers.CopyTo(array, 0);
				base.InitializeServerList(array);
			}
			MultiValuedProperty<IPAddress> multiValuedProperty = transportServer.InternalDNSServers;
			if (transportServer.InternalDNSAdapterEnabled || MultiValuedPropertyBase.IsNullOrEmpty(multiValuedProperty))
			{
				IPAddress[] adapterDnsServerList = DnsServerList.GetAdapterDnsServerList(transportServer.InternalDNSAdapterGuid, Components.TransportAppConfig.RemoteDelivery.ExcludeDnsServersFromLoopbackAdapters, Components.TransportAppConfig.RemoteDelivery.ExcludeIPv6SiteLocalDnsAddresses);
				if (adapterDnsServerList != null && adapterDnsServerList.Length != 0)
				{
					multiValuedProperty = adapterDnsServerList;
				}
				else
				{
					EnhancedDns.Logger.LogEvent(TransportEventLogConstants.Tuple_InvalidAdapterGuid, null, new object[]
					{
						transportServer.InternalDNSAdapterGuid
					});
					string notificationReason = string.Format("No DNS servers could be retrieved from network adapter {0}", transportServer.InternalDNSAdapterGuid);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Warning, false);
				}
			}
			DnsServerList dnsServerList = this.internalDnsServerList;
			IPAddress[] array2 = null;
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				array2 = new IPAddress[multiValuedProperty.Count];
				multiValuedProperty.CopyTo(array2, 0);
			}
			if (dnsServerList == null || !dnsServerList.IsAddressListIdentical(array2))
			{
				DnsServerList dnsServerList2 = new DnsServerList();
				dnsServerList2.Initialize(array2);
				DnsServerList dnsServerList3 = Interlocked.CompareExchange<DnsServerList>(ref this.internalDnsServerList, dnsServerList2, dnsServerList);
				if (dnsServerList3 == dnsServerList)
				{
					if (dnsServerList3 != null)
					{
						dnsServerList3.Dispose();
					}
				}
				else
				{
					dnsServerList2.Dispose();
				}
			}
			this.internalDnsOptions = EnhancedDns.GetDnsOptions(transportServer.InternalDNSProtocolOption);
			base.Timeout = Components.Configuration.AppConfig.RemoteDelivery.DnsRequestTimeout;
			base.QueryRetryInterval = Components.Configuration.AppConfig.RemoteDelivery.DnsQueryRetryInterval;
			if (!Components.Configuration.AppConfig.RemoteDelivery.DnsIpv6Enabled)
			{
				base.DefaultAddressFamily = AddressFamily.InterNetwork;
			}
			ADObjectId homeRoutingGroup = EnhancedDns.GetHomeRoutingGroup(transportServer);
			this.CreateEnterpriseRelayConnector(homeRoutingGroup);
			this.CreateProxyConnectors(homeRoutingGroup);
			ExTraceGlobals.RoutingTracer.TraceDebug<DnsServerList>((long)this.GetHashCode(), "New internal DNS servers: {0}", this.internalDnsServerList);
			ExTraceGlobals.RoutingTracer.TraceDebug<DnsQueryOptions>((long)this.GetHashCode(), "New internal DNS query options: {0}", this.internalDnsOptions);
			ExTraceGlobals.RoutingTracer.TraceDebug<DnsServerList>((long)this.GetHashCode(), "New external DNS servers: {0}", base.ServerList);
			ExTraceGlobals.RoutingTracer.TraceDebug<DnsQueryOptions>((long)this.GetHashCode(), "New external DNS query options: {0}", base.Options);
			ExTraceGlobals.RoutingTracer.TraceDebug<string>((long)this.GetHashCode(), "New timeout: {0}", base.Timeout.ToString());
			ExTraceGlobals.RoutingTracer.TraceDebug<string>((long)this.GetHashCode(), "New query retry interval: {0}", base.QueryRetryInterval.ToString());
			ExTraceGlobals.RoutingTracer.TraceDebug<AddressFamily>((long)this.GetHashCode(), "New default address family: {0}", base.DefaultAddressFamily);
			return true;
		}

		private void GetDnsQuerySettings(int requestId, SmtpSendConnectorConfig connector, out DnsServerList dnsServerList, out DnsQueryOptions dnsQueryOptions)
		{
			if (connector.UseExternalDNSServersEnabled)
			{
				dnsServerList = base.ServerList;
				dnsQueryOptions = base.Options;
				if (Components.TransportAppConfig.RemoteDelivery.DnsFaultTolerance == DnsFaultTolerance.Lenient)
				{
					dnsQueryOptions |= DnsQueryOptions.FailureTolerant;
				}
				else
				{
					dnsQueryOptions &= ~DnsQueryOptions.FailureTolerant;
				}
				ExTraceGlobals.RoutingTracer.TraceDebug<int>((long)this.GetHashCode(), "Request ID={0}: using external DNS", requestId);
				return;
			}
			dnsServerList = this.internalDnsServerList;
			dnsQueryOptions = this.internalDnsOptions;
		}

		private void CheckForTcpInterfaceChanges()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > this.nextPollTcpipSettings)
			{
				bool flag = false;
				lock (this)
				{
					if (utcNow > this.nextPollTcpipSettings)
					{
						this.nextPollTcpipSettings = utcNow + EnhancedDns.TcpipPollInterval;
						flag = true;
					}
				}
				if (flag && this.tcpInterfaceWatcher.IsChanged())
				{
					this.LoadConfiguration();
				}
			}
		}

		private void CreateProxyConnectors(ADObjectId homeRoutingGroup)
		{
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				this.perMessageProxyInternalConnector = new ProxySendConnector(Strings.MailboxProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, true, true, new TlsAuthLevel?(TlsAuthLevel.EncryptionOnly), null, false, 0, null, null);
				if (Components.TransportAppConfig.Routing.DisableExchangeServerAuth)
				{
					this.perMessageProxyInternalConnector.SmartHostAuthMechanism = SmtpSendConnectorConfig.AuthMechanisms.None;
					return;
				}
			}
			else
			{
				if (ConfigurationComponent.IsFrontEndTransportProcess(Components.Configuration))
				{
					this.perMessageProxyExternalConnector = new ProxySendConnector(Strings.ExternalDestinationInboundProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, false, Components.TransportAppConfig.SmtpInboundProxyConfiguration.RequireTls, new TlsAuthLevel?(Components.TransportAppConfig.SmtpInboundProxyConfiguration.TlsAuthLevel), Components.TransportAppConfig.SmtpInboundProxyConfiguration.TlsDomain, Components.TransportAppConfig.SmtpInboundProxyConfiguration.UseExternalDnsServers, 0, null, Components.TransportAppConfig.SmtpInboundProxyConfiguration.ExternalCertificateSubject);
					if (Components.Configuration.AppConfig.SmtpInboundProxyConfiguration.TreatProxyDestinationAsExternal)
					{
						this.perMessageProxyInternalConnector = this.perMessageProxyExternalConnector;
					}
					else
					{
						this.perMessageProxyInternalConnector = new ProxySendConnector(Strings.InternalDestinationInboundProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, true, true, new TlsAuthLevel?(TlsAuthLevel.EncryptionOnly), null, false, 0, null, null);
					}
					this.clientProxyConnector = new ProxySendConnector(Strings.ClientProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, true, true, new TlsAuthLevel?(TlsAuthLevel.EncryptionOnly), null, false, Components.TransportAppConfig.SmtpProxyConfiguration.ProxyPort, null, null);
					return;
				}
				if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub)
				{
					this.outboundProxyExternalConnector = new ProxySendConnector(Strings.ExternalDestinationOutboundProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, false, Components.TransportAppConfig.SmtpOutboundProxyConfiguration.RequireTls, new TlsAuthLevel?(Components.TransportAppConfig.SmtpOutboundProxyConfiguration.TlsAuthLevel), Components.TransportAppConfig.SmtpOutboundProxyConfiguration.TlsDomain, Components.TransportAppConfig.SmtpOutboundProxyConfiguration.UseExternalDnsServers, 0, null, Components.TransportAppConfig.SmtpOutboundProxyConfiguration.ExternalCertificateSubject);
					this.outboundProxyExternalConnector.ErrorPolicies = ErrorPolicies.DowngradeDnsFailures;
					if (Components.Configuration.AppConfig.SmtpOutboundProxyConfiguration.TreatProxyHopAsExternal)
					{
						this.outboundProxyInternalConnector = this.outboundProxyExternalConnector;
						return;
					}
					this.outboundProxyInternalConnector = new ProxySendConnector(Strings.InternalDestinationOutboundProxySendConnector, Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, true, true, new TlsAuthLevel?(TlsAuthLevel.EncryptionOnly), null, false, 717, null, null);
					this.outboundProxyInternalConnector.ErrorPolicies = ErrorPolicies.DowngradeDnsFailures;
				}
			}
		}

		private void CreateEnterpriseRelayConnector(ADObjectId homeRoutingGroup)
		{
			if (Components.IsBridgehead)
			{
				this.enterpriseRelayConnector = new EnterpriseRelaySendConnector(Components.Configuration.LocalServer.TransportServer, homeRoutingGroup, Components.TransportAppConfig.Routing.DisableExchangeServerAuth);
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "Dns";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			if (flag2)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, help."));
			}
			else if (flag)
			{
				XElement xelement2 = new XElement("DnsCache");
				xelement.Add(xelement2);
				xelement.SetAttributeValue("DefaultAddressFamily", base.DefaultAddressFamily);
				xelement.SetAttributeValue("DnsOptions", this.internalDnsOptions);
				xelement.SetAttributeValue("LocalIPAddresses", (base.LocalIPAddresses == null) ? string.Empty : string.Join<IPAddress>(",", base.LocalIPAddresses));
				xelement.SetAttributeValue("DnsServers", (this.internalDnsServerList.Addresses == null) ? string.Empty : string.Join<IPAddress>(",", this.internalDnsServerList.Addresses));
				this.internalDnsServerList.Cache.AddDiagnosticInfoTo(xelement2);
			}
			return xelement;
		}

		int IEnhancedDns.get_MaxDataPerRequest()
		{
			return base.MaxDataPerRequest;
		}

		void IEnhancedDns.set_MaxDataPerRequest(int A_1)
		{
			base.MaxDataPerRequest = A_1;
		}

		DnsServerList IEnhancedDns.get_ServerList()
		{
			return base.ServerList;
		}

		void IEnhancedDns.set_ServerList(DnsServerList A_1)
		{
			base.ServerList = A_1;
		}

		IEnumerable<IPAddress> IEnhancedDns.get_LocalIPAddresses()
		{
			return base.LocalIPAddresses;
		}

		void IEnhancedDns.set_LocalIPAddresses(IEnumerable<IPAddress> A_1)
		{
			base.LocalIPAddresses = A_1;
		}

		TimeSpan IEnhancedDns.get_Timeout()
		{
			return base.Timeout;
		}

		void IEnhancedDns.set_Timeout(TimeSpan A_1)
		{
			base.Timeout = A_1;
		}

		TimeSpan IEnhancedDns.get_QueryRetryInterval()
		{
			return base.QueryRetryInterval;
		}

		void IEnhancedDns.set_QueryRetryInterval(TimeSpan A_1)
		{
			base.QueryRetryInterval = A_1;
		}

		AddressFamily IEnhancedDns.get_DefaultAddressFamily()
		{
			return base.DefaultAddressFamily;
		}

		void IEnhancedDns.set_DefaultAddressFamily(AddressFamily A_1)
		{
			base.DefaultAddressFamily = A_1;
		}

		DnsQueryOptions IEnhancedDns.get_Options()
		{
			return base.Options;
		}

		void IEnhancedDns.set_Options(DnsQueryOptions A_1)
		{
			base.Options = A_1;
		}

		void IEnhancedDns.AdapterServerList(Guid A_1)
		{
			base.AdapterServerList(A_1);
		}

		void IEnhancedDns.AdapterServerList(Guid A_1, bool A_2, bool A_3)
		{
			base.AdapterServerList(A_1, A_2, A_3);
		}

		void IEnhancedDns.InitializeFromMachineServerList()
		{
			base.InitializeFromMachineServerList();
		}

		void IEnhancedDns.InitializeServerList(IPAddress[] A_1)
		{
			base.InitializeServerList(A_1);
		}

		IAsyncResult IEnhancedDns.BeginResolveToAddresses(string A_1, AddressFamily A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginResolveToAddresses(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginResolveToAddresses(string A_1, AddressFamily A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginResolveToAddresses(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginResolveToAddresses(string A_1, AddressFamily A_2, DnsServerList A_3, DnsQueryOptions A_4, AsyncCallback A_5, object A_6)
		{
			return base.BeginResolveToAddresses(A_1, A_2, A_3, A_4, A_5, A_6);
		}

		IAsyncResult IEnhancedDns.BeginResolveToAddresses(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginResolveToAddresses(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginResolveToMailServers(string A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginResolveToMailServers(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginResolveToMailServers(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginResolveToMailServers(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginResolveToMailServers(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginResolveToMailServers(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginResolveToMailServers(string A_1, bool A_2, AddressFamily A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginResolveToMailServers(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveTextRecords(string A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginRetrieveTextRecords(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveTextRecords(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginRetrieveTextRecords(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveTextRecords(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginRetrieveTextRecords(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveSoaRecords(string A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginRetrieveSoaRecords(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveSoaRecords(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginRetrieveSoaRecords(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveSoaRecords(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginRetrieveSoaRecords(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveCNameRecords(string A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginRetrieveCNameRecords(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveCNameRecords(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginRetrieveCNameRecords(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveCNameRecords(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginRetrieveCNameRecords(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginResolveToNames(IPAddress A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginResolveToNames(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginResolveToNames(IPAddress A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginResolveToNames(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginResolveToNames(IPAddress A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginResolveToNames(A_1, A_2, A_3, A_4, A_5);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveSrvRecords(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginRetrieveSrvRecords(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveNsRecords(string A_1, AsyncCallback A_2, object A_3)
		{
			return base.BeginRetrieveNsRecords(A_1, A_2, A_3);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveNsRecords(string A_1, DnsQueryOptions A_2, AsyncCallback A_3, object A_4)
		{
			return base.BeginRetrieveNsRecords(A_1, A_2, A_3, A_4);
		}

		IAsyncResult IEnhancedDns.BeginRetrieveNsRecords(string A_1, DnsServerList A_2, DnsQueryOptions A_3, AsyncCallback A_4, object A_5)
		{
			return base.BeginRetrieveNsRecords(A_1, A_2, A_3, A_4, A_5);
		}

		public const int OutboundProxyFrontendPort = 717;

		private static readonly TimeSpan TcpipPollInterval = TimeSpan.FromMinutes(1.0);

		private static ExEventLog eventLogger;

		private DnsQueryOptions internalDnsOptions;

		private DnsServerList internalDnsServerList;

		private IMailRouter router;

		private int requestCounter;

		private DateTime nextPollTcpipSettings;

		private RegistryWatcher tcpInterfaceWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces", true);

		private SmtpSendConnectorConfig enterpriseRelayConnector;

		private SmtpSendConnectorConfig perMessageProxyInternalConnector;

		private SmtpSendConnectorConfig perMessageProxyExternalConnector;

		private SmtpSendConnectorConfig outboundProxyInternalConnector;

		private SmtpSendConnectorConfig outboundProxyExternalConnector;

		private SmtpSendConnectorConfig clientProxyConnector;
	}
}
