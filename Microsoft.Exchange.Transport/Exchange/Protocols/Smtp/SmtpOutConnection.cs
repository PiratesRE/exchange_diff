using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpOutConnection
	{
		public SmtpOutConnection(NextHopConnection connection, ProtocolLog protocolLog, IMailRouter mailRouter, EnhancedDns enhancedDns, UnhealthyTargetFilterComponent unhealthyTargetFilter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, ISmtpInComponent smtpInComponent, RiskLevel riskLevel, int outboundIPPool, int perHostConnectionAttemptCount, string connectionContextString) : this(connection, protocolLog, mailRouter, enhancedDns, unhealthyTargetFilter, certificateCache, certificateValidator, shadowRedundancyManager, transportAppConfig, transportConfiguration, smtpInComponent, riskLevel, outboundIPPool, 0, perHostConnectionAttemptCount, false, null, null, false, connectionContextString)
		{
		}

		public SmtpOutConnection(NextHopConnection connection, ProtocolLog protocolLog, IMailRouter mailRouter, EnhancedDns enhancedDns, UnhealthyTargetFilterComponent unhealthyTargetFilter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, ISmtpInComponent smtpInComponent, RiskLevel riskLevel, int outboundIPPool) : this(connection, protocolLog, mailRouter, enhancedDns, unhealthyTargetFilter, certificateCache, certificateValidator, shadowRedundancyManager, transportAppConfig, transportConfiguration, smtpInComponent, riskLevel, outboundIPPool, 0, 1, false, null, null, false, null)
		{
		}

		public SmtpOutConnection(NextHopConnection connection, ProtocolLog protocolLog, IMailRouter mailRouter, EnhancedDns enhancedDns, UnhealthyTargetFilterComponent unhealthyTargetFilter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, ISmtpInComponent smtpInComponent, RiskLevel riskLevel, int outboundIPPool, int fixedTotalConnectionAttemptCount, int perHostConnectionAttemptCount, bool clientProxy, TlsSendConfiguration tlsSendConfiguration, ISmtpInSession inSession, bool isShadowOut, string connectionContextString)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.nextHopConnection = connection;
			this.protocolLog = protocolLog;
			this.mailRouter = mailRouter;
			this.certificateCache = certificateCache;
			this.certificateValidator = certificateValidator;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
			this.smtpInComponent = smtpInComponent;
			this.fixedTotalConnectionAttemptCount = fixedTotalConnectionAttemptCount;
			this.perHostConnectionAttemptCount = perHostConnectionAttemptCount;
			this.ClientProxy = clientProxy;
			this.inSession = inSession;
			this.isShadowOut = isShadowOut;
			this.TlsConfig = tlsSendConfiguration;
			this.RiskLevel = riskLevel;
			this.OutboundIPPool = outboundIPPool;
			this.connectionContextString = connectionContextString;
			this.smtpOutTargetHostPicker = new SmtpOutTargetHostPicker(this.sessionId, this, this.nextHopConnection, enhancedDns, unhealthyTargetFilter.UnhealthyTargetIPAddressFilter, unhealthyTargetFilter.UnhealthyTargetFqdnFilter);
			this.unhealthyTargetFilter = unhealthyTargetFilter;
		}

		public bool IsBlindProxy
		{
			get
			{
				return this.inSession != null;
			}
		}

		public bool NextHopIsOutboundProxy
		{
			get
			{
				return this.outboundProxyContext != null;
			}
		}

		public string SmtpHostName
		{
			get
			{
				return this.smtpOutTargetHostPicker.SmtpHostName;
			}
		}

		public string SmtpHost
		{
			get
			{
				return this.smtpOutTargetHostPicker.SmtpHost;
			}
		}

		public ulong BytesSent
		{
			get
			{
				return this.bytesSent;
			}
			set
			{
				this.bytesSent = value;
			}
		}

		public ulong MessagesSent
		{
			get
			{
				return this.messagesSent;
			}
			set
			{
				this.messagesSent = value;
			}
		}

		public ulong DiscardIdsReceived
		{
			get
			{
				return this.discardIdsReceived;
			}
			set
			{
				this.discardIdsReceived = value;
			}
		}

		public SmtpSendConnectorConfig Connector
		{
			get
			{
				return this.sendConnector;
			}
		}

		public SmtpSendPerfCountersInstance SmtpSendPerformanceCounters
		{
			get
			{
				return this.smtpSendPerformanceCounters;
			}
		}

		public int TotalTargets
		{
			get
			{
				return this.smtpOutTargetHostPicker.TotalTargets;
			}
		}

		public ProtocolLog ProtocolLog
		{
			get
			{
				return this.protocolLog;
			}
		}

		internal TlsSendConfiguration TlsConfig { get; private set; }

		internal RiskLevel RiskLevel { get; private set; }

		internal int OutboundIPPool { get; private set; }

		public void DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		public void SetSendConnector(SmtpSendConnectorConfig connector, SmtpSendConnectorConfig outboundProxyDestinationConnector, IEnumerable<INextHopServer> outboundProxyDestinations)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SetSendConnector);
			this.sendConnector = connector;
			if (outboundProxyDestinationConnector != null)
			{
				this.SetOutboundProxyContext(outboundProxyDestinations, outboundProxyDestinationConnector);
			}
			bool flag = outboundProxyDestinationConnector == null;
			if (this.createSmtpSendPerfCounters)
			{
				string connectorName = (outboundProxyDestinationConnector != null) ? outboundProxyDestinationConnector.Name : this.sendConnector.Name;
				this.smtpSendPerformanceCounters = SmtpOutConnectionHandler.GetSmtpSendPerfCounterInstance(connectorName);
			}
			if (this.TlsConfig == null)
			{
				if (flag)
				{
					this.TlsConfig = new TlsSendConfiguration(this.sendConnector, this.nextHopConnection.Key.TlsAuthLevel, this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.Key.NextHopTlsDomain);
					return;
				}
				this.TlsConfig = new TlsSendConfiguration(this.sendConnector, null, null, null);
			}
		}

		public void UpdateOnSuccessfulOutboundProxySetup(IEnumerable<INextHopServer> remainingDestinations, bool shouldSkipTls)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.UpdateOnSuccessfulOutboundProxySetup);
			if (!this.NextHopIsOutboundProxy)
			{
				throw new InvalidOperationException("Should only be called if the next hop is outbound proxy");
			}
			this.outboundProxyContext.ProxyDestinations = remainingDestinations;
			this.outboundProxyContext.ProxyTlsConfiguration.ShouldSkipTls = shouldSkipTls;
		}

		public void GetOutboundProxyDestinationSettings(out IEnumerable<INextHopServer> destinations, out SmtpSendConnectorConfig sendConnector, out TlsSendConfiguration tlsSendConfiguration, out RiskLevel riskLevel, out int outboundIPPool)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.GetOutboundProxyDestinationSettings);
			if (!this.NextHopIsOutboundProxy)
			{
				throw new InvalidOperationException("Should only be called if the next hop is outbound proxy");
			}
			destinations = this.outboundProxyContext.ProxyDestinations;
			sendConnector = this.outboundProxyContext.ProxySendConnector;
			tlsSendConfiguration = this.outboundProxyContext.ProxyTlsConfiguration;
			riskLevel = this.outboundProxyContext.ProxyRiskLevel;
			outboundIPPool = this.outboundProxyContext.OutboundIPPool;
		}

		public bool TryGetRemainingSmtpTargets(out IEnumerable<INextHopServer> remainingTargets)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.TryGetRemainingSmtpTargets);
			return this.smtpOutTargetHostPicker.TryGetRemainingSmtpTargets(out remainingTargets);
		}

		public void UpdateSession(NetworkConnection nc)
		{
			this.nextHopConnection.ConnectionAttemptSucceeded();
			this.smtpOutSession.ConnectionCompleted(nc);
		}

		public void Shutdown()
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.Shutdown);
			if (this.smtpOutSession != null)
			{
				this.smtpOutSession.ShutdownConnection();
			}
		}

		public void Connect()
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.Connect);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<NextHopType, string, Guid>((long)this.GetHashCode(), "Resolving next hop '{0}':'{1}':'{2}' in Enhanced DNS.", this.nextHopConnection.Key.NextHopType, this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.Key.NextHopConnector);
			int[] activeCountsPerPriority;
			int[] retryCountsPerPriority;
			this.nextHopConnection.GetQueueCountsOnlyForIndividualPriorities(out activeCountsPerPriority, out retryCountsPerPriority);
			ConnectionLog.SmtpConnectionStart(this.sessionId, this.nextHopConnection.Key, this.nextHopConnection.ActiveQueueLength, activeCountsPerPriority, retryCountsPerPriority, null);
			this.smtpOutTargetHostPicker.ResolveToNextHopAndConnect();
		}

		public void ConnectToPerMessageProxyDestinations(IEnumerable<INextHopServer> destinations, bool internalDestination)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.ConnectToPerMessageProxyDestinations);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<NextHopType, string, Guid>((long)this.GetHashCode(), "Resolving proxy next hop '{0}':'{1}':'{2}' in Enhanced DNS.", this.nextHopConnection.Key.NextHopType, this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.Key.NextHopConnector);
			ConnectionLog.SmtpConnectionStart(this.sessionId, this.nextHopConnection.Key, this.nextHopConnection.ActiveQueueLength, null, null, this.connectionContextString);
			this.smtpOutTargetHostPicker.ResolveProxyNextHopAndConnect(destinations, internalDestination, SmtpOutProxyType.PerMessage);
		}

		public void ConnectToBlindProxyDestinations(IEnumerable<INextHopServer> destinations, bool internalDestinations, SmtpSendConnectorConfig connector)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.ConnectToBlindProxyDestinations);
			if (this.inSession == null)
			{
				throw new InvalidOperationException("InSession needs to be set for blind proxy");
			}
			this.createSmtpSendPerfCounters = false;
			ExTraceGlobals.SmtpSendTracer.TraceDebug<NextHopType, string, Guid>((long)this.GetHashCode(), "Resolving proxy next hop '{0}':'{1}':'{2}' in Enhanced DNS.", this.nextHopConnection.Key.NextHopType, this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.Key.NextHopConnector);
			ConnectionLog.SmtpConnectionStart(this.sessionId, this.nextHopConnection.Key, this.connectionContextString);
			this.smtpOutTargetHostPicker.ResolveProxyNextHopAndConnect(destinations, internalDestinations, connector);
		}

		public void ConnectToShadowDestinations(IEnumerable<INextHopServer> destinations)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.ConnectToShadowDestinations);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Initiating resolution and connection to shadow next hop");
			ConnectionLog.SmtpConnectionStart(this.sessionId, this.nextHopConnection.Key, this.nextHopConnection.ActiveQueueLength, null, null, null);
			this.smtpOutTargetHostPicker.ResolveProxyNextHopAndConnect(destinations, true, SmtpOutProxyType.ShadowPeerToPeer);
		}

		public void FailoverConnection(SmtpResponse response, bool retryWithoutStartTls, SessionSetupFailureReason failoverReason, bool nextHopWasProxyingBlindly)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.FailoverConnection);
			ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Attempting to failover connection to an alternate IP address for domain '{0}':'{1}':'{2}'.retryWithoutStartTls={3}, failoverReason={4}, nexthopWasProxyingBlindly={5}", new object[]
			{
				this.nextHopConnection.Key.NextHopType,
				this.nextHopConnection.Key.NextHopDomain,
				this.nextHopConnection.Key.NextHopConnector,
				retryWithoutStartTls,
				failoverReason,
				nextHopWasProxyingBlindly
			});
			if (this.failoverResponse.Equals(SmtpResponse.Empty))
			{
				this.failoverResponse = response;
				this.failoverReason = failoverReason;
			}
			this.lastFailoverReason = failoverReason;
			this.UpdateSmtpSendFailurePerfCounter(failoverReason);
			this.nextHopConnection.NotifyConnectionFailedOver(this.smtpOutTargetHostPicker.SmtpHostName, response, failoverReason);
			this.ConnectToNextHost(retryWithoutStartTls, nextHopWasProxyingBlindly);
		}

		public void FailoverConnection(SmtpResponse response, bool retryWithoutStartTls, SessionSetupFailureReason failoverReason)
		{
			this.FailoverConnection(response, retryWithoutStartTls, failoverReason, false);
		}

		public void ConnectToNextHost()
		{
			this.ConnectToNextHost(false, false);
		}

		public void UpdateSmtpSendFailurePerfCounter(SessionSetupFailureReason failureReason)
		{
			if (this.SmtpSendPerformanceCounters != null)
			{
				switch (failureReason)
				{
				case SessionSetupFailureReason.None:
				case SessionSetupFailureReason.UserLookupFailure:
				case SessionSetupFailureReason.Shutdown:
					break;
				case SessionSetupFailureReason.DnsLookupFailure:
					this.smtpSendPerformanceCounters.DnsErrors.Increment();
					return;
				case SessionSetupFailureReason.ConnectionFailure:
					this.SmtpSendPerformanceCounters.ConnectionFailures.Increment();
					return;
				case SessionSetupFailureReason.ProtocolError:
					this.SmtpSendPerformanceCounters.ProtocolErrors.Increment();
					return;
				case SessionSetupFailureReason.SocketError:
					this.SmtpSendPerformanceCounters.SocketErrors.Increment();
					return;
				default:
					throw new InvalidOperationException("Invalid session failure reason");
				}
			}
		}

		internal void AckConnectionForResubmitWithoutHighAvailability(SmtpResponse smtpResponse, string reason)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.AckConnectionForResubmitWithoutHighAvailability);
			if (this.nextHopConnection == null)
			{
				throw new InvalidOperationException("Connection has already been acked!");
			}
			AckStatus ackStatus = AckStatus.Resubmit;
			string text = ackStatus.ToString();
			if (reason != null)
			{
				text = text + " : " + reason;
			}
			ConnectionLog.SmtpConnectionStop(this.sessionId, this.nextHopConnection.Key.NextHopDomain, text, this.messagesSent, this.bytesSent, this.discardIdsReceived);
			this.nextHopConnection.AckConnection(MessageTrackingSource.QUEUE, null, ackStatus, smtpResponse, null, null, true);
			this.nextHopConnection = null;
		}

		internal void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, string reason, SessionSetupFailureReason failureReason)
		{
			this.AckConnection(ackStatus, smtpResponse, details, reason, failureReason, true);
		}

		internal void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, string reason, SessionSetupFailureReason failureReason, bool updateFailureCounters)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.AckConnection);
			if (this.nextHopConnection == null)
			{
				throw new InvalidOperationException("Connection has already been acked!");
			}
			string text = string.Empty;
			if (ackStatus != AckStatus.Success)
			{
				text = ackStatus.ToString();
				if (reason != null)
				{
					text = text + " : " + reason;
				}
			}
			ConnectionLog.SmtpConnectionStop(this.sessionId, this.nextHopConnection.Key.NextHopDomain, text, this.messagesSent, this.bytesSent, this.discardIdsReceived);
			if (updateFailureCounters)
			{
				this.UpdateSmtpSendFailurePerfCounter(failureReason);
			}
			this.nextHopConnection.AckConnection(ackStatus, smtpResponse, details, failureReason);
			this.nextHopConnection = null;
		}

		internal void RemoveConnection()
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.RemoveConnection);
			if (this.connectionSucceededToNextHop)
			{
				this.smtpOutTargetHostPicker.ConnectionDisconnected();
				this.connectionSucceededToNextHop = false;
			}
			SmtpOutConnectionHandler.RemoveConnection(this.sessionId);
		}

		internal bool AddConnection()
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.AddConnection);
			if (!this.connectionAdded)
			{
				if (!SmtpOutConnectionHandler.AddConnection(this.sessionId, this))
				{
					return false;
				}
				this.connectionAdded = true;
			}
			return true;
		}

		internal string GetSessionAndConnectionInfo()
		{
			if (this.smtpOutSession != null)
			{
				return this.smtpOutSession.GetConnectionInfo();
			}
			return string.Empty;
		}

		internal void RoutingTableUpdate()
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.RoutingTableUpdate);
			this.smtpOutTargetHostPicker.RoutingTableUpdate();
		}

		internal void NextHopResolutionFailed(DnsStatus status, IPAddress reportingServer, string diagnosticInfo)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.NextHopResolutionFailed);
			bool flag = false;
			bool flag2 = false;
			SmtpSendConnectorConfig connector = this.Connector;
			if (connector == null && this.nextHopConnection.Key.NextHopType.IsSmtpConnectorDeliveryType)
			{
				this.mailRouter.TryGetLocalSendConnector<SmtpSendConnectorConfig>(this.nextHopConnection.Key.NextHopConnector, out connector);
			}
			SmtpResponse actualError = SmtpResponse.Empty;
			switch (status)
			{
			case DnsStatus.Success:
				throw new InvalidOperationException("NextHopResolutionFailed status=Success");
			case DnsStatus.InfoNoRecords:
				break;
			case DnsStatus.InfoDomainNonexistent:
			{
				SmtpResponse smtpResponse;
				if (this.NextHopIsOutboundProxy)
				{
					smtpResponse = AckReason.DnsNonExistentDomainForOutboundFrontend;
					ConnectionLog.SmtpHostResolutionFailedForOutboundProxyFrontend(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Non-existent domain", diagnosticInfo);
				}
				else
				{
					smtpResponse = AckReason.DnsNonExistentDomain;
					ConnectionLog.SmtpHostResolutionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Non-existent domain", diagnosticInfo);
				}
				if (!SmtpOutConnection.IsDnsConnectorDelivery(this.nextHopConnection.Key.NextHopType.DeliveryType, connector, this.transportConfiguration))
				{
					flag = true;
					actualError = smtpResponse;
					goto IL_4E2;
				}
				if (connector != null && (connector.ErrorPolicies & ErrorPolicies.DowngradeDnsFailures) != ErrorPolicies.Default)
				{
					if (this.NextHopIsOutboundProxy)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
					actualError = smtpResponse;
					goto IL_4E2;
				}
				this.AckConnectionWithDNSError(AckStatus.Fail, smtpResponse, "The domain name does not exist. Please correct the address and try again.");
				goto IL_4E2;
			}
			case DnsStatus.InfoMxLoopback:
			{
				SmtpResponse smtpResponse;
				if (this.NextHopIsOutboundProxy)
				{
					smtpResponse = AckReason.DnsMxLoopbackForOutboundFrontend;
					ConnectionLog.SmtpHostResolutionFailedForOutboundProxyFrontend(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Mail would loop back to itself", diagnosticInfo);
				}
				else
				{
					smtpResponse = AckReason.DnsMxLoopback;
					ConnectionLog.SmtpHostResolutionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Mail would loop back to itself", diagnosticInfo);
				}
				if (!SmtpOutConnection.IsDnsConnectorDelivery(this.nextHopConnection.Key.NextHopType.DeliveryType, connector, this.transportConfiguration))
				{
					flag = true;
					actualError = smtpResponse;
					goto IL_4E2;
				}
				if (connector != null && (connector.ErrorPolicies & ErrorPolicies.DowngradeDnsFailures) != ErrorPolicies.Default)
				{
					if (this.NextHopIsOutboundProxy)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
					actualError = smtpResponse;
					goto IL_4E2;
				}
				this.AckConnectionWithDNSError(AckStatus.Fail, smtpResponse, "The domain name has misconfigured records registered in DNS. The records are configured in a loop.");
				goto IL_4E2;
			}
			case DnsStatus.ErrorInvalidData:
			{
				SmtpResponse smtpResponse;
				if (this.NextHopIsOutboundProxy)
				{
					smtpResponse = AckReason.DnsInvalidDataOutboundFrontend;
					ConnectionLog.SmtpHostResolutionFailedForOutboundProxyFrontend(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Invalid DNS data returned", diagnosticInfo);
				}
				else
				{
					smtpResponse = AckReason.DnsInvalidData;
					ConnectionLog.SmtpHostResolutionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "Invalid DNS data returned", diagnosticInfo);
				}
				if (!SmtpOutConnection.IsDnsConnectorDelivery(this.nextHopConnection.Key.NextHopType.DeliveryType, connector, this.transportConfiguration))
				{
					flag = true;
					actualError = smtpResponse;
					goto IL_4E2;
				}
				if (connector != null && (connector.ErrorPolicies & ErrorPolicies.DowngradeDnsFailures) != ErrorPolicies.Default)
				{
					if (this.NextHopIsOutboundProxy)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
					actualError = smtpResponse;
					goto IL_4E2;
				}
				this.AckConnectionWithDNSError(AckStatus.Fail, smtpResponse, "A DNS server returned corrupt information when resolving this domain");
				goto IL_4E2;
			}
			default:
				if (status == DnsStatus.ConfigChanged)
				{
					string eventText = string.Format("The DNS query for '{0}':'{1}':'{2}' failed with error: {3}. {4}", new object[]
					{
						this.nextHopConnection.Key.NextHopType,
						this.nextHopConnection.Key.NextHopDomain,
						this.nextHopConnection.Key.NextHopConnector,
						status,
						(this.nextHopConnection.Key.NextHopType == NextHopType.Heartbeat) ? string.Empty : "The messages in this queue are being resubmitted to the categorizer."
					});
					this.AckConnectionWithDNSError(AckStatus.Resubmit, SmtpResponse.DnsConfigChangedSmtpResponse, eventText);
					goto IL_4E2;
				}
				if (status == DnsStatus.NoOutboundFrontendServers)
				{
					string eventText = string.Format("Connector {0} has the FrontendProxyEnabled option set to true but there are no suitable Frontend Transport servers in the local AD site or app.config.", (connector == null) ? this.nextHopConnection.Key.NextHopConnector.ToString() : connector.Name);
					this.AckConnectionWithDNSError(AckStatus.Retry, AckReason.NoOutboundFrontendServers, eventText);
					goto IL_4E2;
				}
				break;
			}
			if (this.NextHopIsOutboundProxy)
			{
				actualError = new SmtpResponse("451", "4.4.0", new string[]
				{
					"DNS query for the outbound proxy frontend server failed with error " + status
				});
				ConnectionLog.SmtpHostResolutionFailedForOutboundProxyFrontend(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "DNS server returned " + status.ToString(), diagnosticInfo);
			}
			else
			{
				actualError = new SmtpResponse("451", "4.4.0", new string[]
				{
					"DNS query failed with error " + status
				});
				ConnectionLog.SmtpHostResolutionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, reportingServer, "DNS server returned " + status.ToString(), diagnosticInfo);
			}
			flag = true;
			IL_4E2:
			if (flag || flag2)
			{
				string eventText2 = string.Format("The DNS query for {0} '{1}':'{2}':'{3}' failed with error {4}: {5}", new object[]
				{
					this.NextHopIsOutboundProxy ? "the outbound frontend servers to proxy" : string.Empty,
					this.nextHopConnection.Key.NextHopType,
					this.nextHopConnection.Key.NextHopDomain,
					this.nextHopConnection.Key.NextHopConnector,
					flag2 ? "but was ignored as per connector configuration" : string.Empty,
					status
				});
				this.AckConnectionWithDNSError(AckStatus.Retry, SmtpOutConnection.DnsQueryFailedResponse(actualError), eventText2);
			}
		}

		public void UpdateServerLatency(TimeSpan latency)
		{
			this.unhealthyTargetFilter.UpdateServerLatency(this.smtpOutTargetHostPicker.CurrentSmtpTarget, latency);
		}

		public TimeSpan GetDelayForCurrentTarget(IPEndPoint remoteEndPoint)
		{
			return this.unhealthyTargetFilter.GetServerLatency(new IPAddressPortPair(remoteEndPoint.Address, (ushort)remoteEndPoint.Port));
		}

		private static SmtpResponse DnsQueryFailedResponse(SmtpResponse actualError)
		{
			if (actualError.Equals(SmtpResponse.Empty) || actualError.StatusText == null)
			{
				return SmtpResponse.DnsQueryFailedResponseDefault;
			}
			return new SmtpResponse("451", "4.4.0", new string[]
			{
				"DNS query failed. The error was: " + actualError.StatusText[0]
			});
		}

		private static void OnConnectComplete(IAsyncResult asyncResult)
		{
			SmtpOutConnection smtpOutConnection = (SmtpOutConnection)asyncResult.AsyncState;
			smtpOutConnection.ConnectComplete(asyncResult);
		}

		private static bool IsProxySessionSetupProtocolFailure(SessionSetupFailureReason failoverReason, SmtpResponse failoverResponse)
		{
			return failoverReason == SessionSetupFailureReason.ProtocolError && (failoverResponse.Equals(SmtpResponse.ProxySessionProtocolSetupPermanentFailure) || failoverResponse.Equals(SmtpResponse.ProxySessionProtocolSetupTransientFailure)) && failoverResponse.StatusText.Length > 0 && failoverResponse.StatusText[0].StartsWith("Proxy session setup failed on Frontend with ", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsDnsConnectorDeliveryOnHub(DeliveryType deliveryType, ProcessTransportRole processTransportRole)
		{
			return processTransportRole == ProcessTransportRole.Hub && deliveryType == DeliveryType.DnsConnectorDelivery;
		}

		private static bool IsDnsConnectorDeliveryOnEdge(DeliveryType deliveryType, ProcessTransportRole processTransportRole)
		{
			return processTransportRole == ProcessTransportRole.Edge && deliveryType == DeliveryType.DnsConnectorDelivery;
		}

		private static bool IsDnsConnectorDeliveryOnFrontend(DeliveryType deliveryType, SmtpSendConnectorConfig connector, ITransportConfiguration transportConfiguration)
		{
			return ConfigurationComponent.IsFrontEndTransportProcess(transportConfiguration) && connector.DNSRoutingEnabled && deliveryType == DeliveryType.Undefined;
		}

		private static bool IsDnsConnectorDelivery(DeliveryType deliveryType, SmtpSendConnectorConfig connector, ITransportConfiguration transportConfiguration)
		{
			return SmtpOutConnection.IsDnsConnectorDeliveryOnHub(deliveryType, transportConfiguration.ProcessTransportRole) || SmtpOutConnection.IsDnsConnectorDeliveryOnEdge(deliveryType, transportConfiguration.ProcessTransportRole) || SmtpOutConnection.IsDnsConnectorDeliveryOnFrontend(deliveryType, connector, transportConfiguration);
		}

		private void SetOutboundProxyContext(IEnumerable<INextHopServer> destinations, SmtpSendConnectorConfig sendConnector)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SetOutboundProxyContext);
			if (destinations == null)
			{
				throw new ArgumentNullException("destinations");
			}
			if (sendConnector == null)
			{
				throw new ArgumentNullException("sendConnector");
			}
			TlsSendConfiguration proxyTlsConfiguration = new TlsSendConfiguration(sendConnector, this.nextHopConnection.Key.TlsAuthLevel, this.nextHopConnection.Key.NextHopDomain, this.nextHopConnection.Key.NextHopTlsDomain);
			this.outboundProxyContext = new SmtpOutConnection.OutboundProxyContext(destinations, proxyTlsConfiguration, sendConnector, this.nextHopConnection.Key.RiskLevel, this.nextHopConnection.Key.OutboundIPPool);
		}

		private bool TryUsingCachedSmtpOutSession(IPEndPoint endPoint)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.TryUsingCachedSmtpOutSession);
			if (this.nextHopConnection == null || this.nextHopConnection.Key.NextHopType == NextHopType.Heartbeat)
			{
				return false;
			}
			NextHopSolutionKey nextHopKey;
			IPEndPoint remoteEndPoint;
			if (this.NextHopIsOutboundProxy)
			{
				nextHopKey = SmtpOutSessionCache.OutboundFrontendCacheKey;
				remoteEndPoint = SmtpOutSessionCache.OutboundFrontendIPEndpointCacheKey;
			}
			else
			{
				nextHopKey = this.nextHopConnection.Key;
				remoteEndPoint = endPoint;
			}
			SmtpOutSession smtpOutSession;
			string logMessage;
			if (!SmtpOutConnectionHandler.SessionCache.TryGetValue(nextHopKey, remoteEndPoint, out smtpOutSession, out logMessage))
			{
				return false;
			}
			smtpOutSession.ResetSession(this, this.nextHopConnection);
			ConnectionLog.SmtpConnectionStopDueToCacheHit(this.sessionId, smtpOutSession.SessionId, this.nextHopConnection.Key.NextHopDomain);
			int[] activeCountsPerPriority;
			int[] retryCountsPerPriority;
			this.nextHopConnection.GetQueueCountsOnlyForIndividualPriorities(out activeCountsPerPriority, out retryCountsPerPriority);
			ConnectionLog.SmtpConnectionStartCacheHit(smtpOutSession.SessionId, this.sessionId, this.nextHopConnection.Key, this.nextHopConnection.ActiveQueueLength, activeCountsPerPriority, retryCountsPerPriority, logMessage);
			if (!SmtpOutConnectionHandler.ReplaceConnectionID(this.sessionId, smtpOutSession.SessionId))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Already shutting down");
				smtpOutSession.ShutdownConnection();
				this.RemoveConnection();
				return false;
			}
			this.sessionId = smtpOutSession.SessionId;
			this.smtpOutSession = smtpOutSession;
			this.smtpOutTargetHostPicker.UpdateSessionId(this.sessionId);
			this.nextHopConnection.ConnectionAttemptSucceeded();
			this.smtpOutSession.PrepareForNextMessageOnCachedSession();
			return true;
		}

		private void ConnectToNextHost(bool retryWithoutStartTls, bool nextHopWasProxyingBlindly)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.ConnectToNextHost);
			if (this.connectionSucceededToNextHop)
			{
				this.smtpOutTargetHostPicker.ConnectionDisconnected();
				this.connectionSucceededToNextHop = false;
			}
			for (;;)
			{
				SmtpOutTargetHostPicker.SmtpTarget smtpTarget = null;
				if (nextHopWasProxyingBlindly)
				{
					if (this.outboundProxyContext.ProxyDestinations != null && this.outboundProxyContext.ProxyDestinations.GetEnumerator().MoveNext())
					{
						smtpTarget = this.smtpOutTargetHostPicker.CurrentSmtpTarget;
					}
				}
				else if (this.smtpOutTargetHostPicker.CurrentSmtpTarget != null && this.connectionsAttemptedToCurrentTarget < this.perHostConnectionAttemptCount)
				{
					smtpTarget = this.smtpOutTargetHostPicker.CurrentSmtpTarget;
				}
				else
				{
					this.connectionsAttemptedToCurrentTarget = 0;
					smtpTarget = this.smtpOutTargetHostPicker.GetNextTargetToConnect();
				}
				if (smtpTarget == null || (this.fixedTotalConnectionAttemptCount > 0 && this.totalConnectionsAttempted >= this.fixedTotalConnectionAttemptCount))
				{
					if (this.totalConnectionsAttempted < this.fixedTotalConnectionAttemptCount)
					{
						this.smtpOutTargetHostPicker.StartOverForRetry();
					}
					else
					{
						this.HandleConnectionToAllTargetsFailed(retryWithoutStartTls);
						this.connectionsAttemptedToCurrentTarget = 0;
						if (!retryWithoutStartTls)
						{
							break;
						}
					}
				}
				else
				{
					this.totalConnectionsAttempted++;
					this.connectionsAttemptedToCurrentTarget++;
					IPEndPoint ipendPoint = new IPEndPoint(smtpTarget.Address, (int)smtpTarget.Port);
					ExTraceGlobals.SmtpSendTracer.TraceDebug<string, IPEndPoint>((long)this.GetHashCode(), "Initiating connection to remote domain {0} to {1}", smtpTarget.TargetHostName, ipendPoint);
					this.currentTargetEndpoint = ipendPoint;
					if (Components.ShuttingDown)
					{
						goto Block_11;
					}
					if (this.connectionAdded && this.smtpOutSession != null)
					{
						ulong num = (this.inSession == null) ? SessionId.GetNextSessionId() : this.inSession.SessionId;
						ConnectionLog.SmtpConnectionStop(this.sessionId, this.nextHopConnection.Key.NextHopDomain, "Attempting next target", this.messagesSent, this.bytesSent, this.discardIdsReceived);
						if (!SmtpOutConnectionHandler.ReplaceConnectionID(this.sessionId, num))
						{
							goto Block_15;
						}
						ConnectionLog.SmtpConnectionFailover(num, this.sessionId, this.nextHopConnection.Key.NextHopDomain, this.lastFailoverReason);
						this.sessionId = num;
						int[] activeCountsPerPriority;
						int[] retryCountsPerPriority;
						this.nextHopConnection.GetQueueCountsOnlyForIndividualPriorities(out activeCountsPerPriority, out retryCountsPerPriority);
						ConnectionLog.SmtpConnectionStart(this.sessionId, this.nextHopConnection.Key, this.nextHopConnection.TotalQueueLength, activeCountsPerPriority, retryCountsPerPriority, null);
						this.smtpOutTargetHostPicker.UpdateSessionId(num);
					}
					if (!this.IsBlindProxy && this.TryUsingCachedSmtpOutSession(ipendPoint))
					{
						return;
					}
					if (!this.TryInitializeSmtpOutSession(ipendPoint))
					{
						return;
					}
					if (this.smtpOutTargetHostPicker.TryMarkCurrentSmtpTargetInConnectingState() && this.TryBeginConnectToNextHop(ipendPoint))
					{
						return;
					}
					if (this.failoverResponse.Equals(SmtpResponse.Empty))
					{
						this.failoverResponse = SmtpResponse.UnableToConnect;
						this.failoverReason = SessionSetupFailureReason.ConnectionFailure;
					}
					this.lastFailoverReason = SessionSetupFailureReason.ConnectionFailure;
					this.UpdateSmtpSendFailurePerfCounter(SessionSetupFailureReason.ConnectionFailure);
					this.nextHopConnection.NotifyConnectionFailedOver(this.smtpOutTargetHostPicker.SmtpHostName, SmtpResponse.Empty, SessionSetupFailureReason.ConnectionFailure);
				}
			}
			return;
			Block_11:
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Already shutting down");
			this.AckConnection(AckStatus.Retry, SmtpResponse.ServiceUnavailable, null, "Transport Service Shutting down", SessionSetupFailureReason.Shutdown);
			this.RemoveConnection();
			return;
			Block_15:
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Already shutting down");
			this.RemoveConnection();
		}

		private void HandleConnectionToAllTargetsFailed(bool retryWithoutStartTls)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.HandleConnectionToAllTargetsFailed);
			if (retryWithoutStartTls)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "STARTTLS negotiation failed. No more hosts to connect to. Retrying without STARTTLS");
				this.smtpOutTargetHostPicker.StartOverForRetry();
				this.totalConnectionsAttempted = 0;
				this.TlsConfig.ShouldSkipTls = true;
				return;
			}
			string reason = null;
			if (this.failoverResponse.StatusText != null)
			{
				reason = string.Concat(this.failoverResponse.StatusText);
			}
			if (this.nextHopConnection.Key.IsLocalDeliveryGroupRelay)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "No more hosts connect to for high availability routing, acking connection as resubmit");
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Primary target IP address responded with: \"{0}.\" Attempted failover to alternate host, but that did not succeed. Either there are no alternate hosts, or delivery failed to all alternate hosts. Queue will be resubmitted for routing for MAPI delivery.", this.failoverResponse);
				if (this.currentTargetEndpoint != null)
				{
					stringBuilder.AppendFormat(" The last endpoint attempted was {0}:{1}", this.currentTargetEndpoint.Address, this.currentTargetEndpoint.Port.ToString());
				}
				this.AckConnectionForResubmitWithoutHighAvailability(new SmtpResponse("451", "4.4.0", new string[]
				{
					stringBuilder.ToString()
				}), reason);
			}
			else
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "No more hosts connect to, acking connection as retry");
				StringBuilder stringBuilder2 = new StringBuilder();
				string format;
				if (this.failoverReason == SessionSetupFailureReason.ProtocolError)
				{
					if (this.NextHopIsOutboundProxy)
					{
						format = "Primary outbound frontend IP address responded with:   \"{0}.\"   Attempted failover to alternate frontend address, but that did not succeed. Either there are no alternate frontend addresses, or delivery failed to all alternate frontend addresses.";
					}
					else
					{
						format = "Primary target IP address responded with: \"{0}.\" Attempted failover to alternate host, but that did not succeed. Either there are no alternate hosts, or delivery failed to all alternate hosts.";
					}
				}
				else if (this.NextHopIsOutboundProxy)
				{
					format = "Error encountered while communicating with primary outbound frontend IP address:   \"{0}.\"   Attempted failover to alternate frontend, but that did not succeed. Either there are no alternate frontend addresses, or delivery failed to all alternate frontend addresses.";
				}
				else
				{
					format = "Error encountered while communicating with primary target IP address: \"{0}.\" Attempted failover to alternate host, but that did not succeed. Either there are no alternate hosts, or delivery failed to all alternate hosts.";
				}
				string statusCode;
				string enhancedStatusCode;
				if (this.failoverReason == SessionSetupFailureReason.SocketError)
				{
					statusCode = SmtpResponse.SocketError.StatusCode;
					enhancedStatusCode = SmtpResponse.SocketError.EnhancedStatusCode;
					stringBuilder2.AppendFormat(format, (this.socketErrorDetails != null) ? this.socketErrorDetails : this.failoverResponse.ToString());
				}
				else
				{
					statusCode = SmtpResponse.ConnectionFailover.StatusCode;
					enhancedStatusCode = SmtpResponse.ConnectionFailover.EnhancedStatusCode;
					stringBuilder2.AppendFormat(format, this.failoverResponse.ToString());
				}
				if (this.currentTargetEndpoint != null)
				{
					stringBuilder2.AppendFormat(" The last endpoint attempted was {0}:{1}", this.currentTargetEndpoint.Address, this.currentTargetEndpoint.Port.ToString());
				}
				SmtpResponse smtpResponse = new SmtpResponse(statusCode, enhancedStatusCode, new string[]
				{
					stringBuilder2.ToString()
				});
				SessionSetupFailureReason failureReason = (this.lastFailoverReason == SessionSetupFailureReason.None) ? SessionSetupFailureReason.ConnectionFailure : this.lastFailoverReason;
				AckDetails details;
				if (!this.NextHopIsOutboundProxy)
				{
					details = ((this.smtpOutSession.AckDetails != null) ? this.smtpOutSession.AckDetails : new AckDetails((this.currentTargetEndpoint != null) ? this.currentTargetEndpoint : null, this.nextHopConnection.Key.NextHopDomain, this.sessionId.ToString(), string.Empty, (this.smtpOutSession.LocalEndPoint != null) ? this.smtpOutSession.LocalEndPoint.Address : null));
				}
				else
				{
					bool flag = SmtpOutConnection.IsProxySessionSetupProtocolFailure(this.failoverReason, this.failoverResponse);
					details = new AckDetails(flag ? null : this.currentTargetEndpoint, flag ? this.nextHopConnection.Key.NextHopDomain : ((this.smtpOutSession.AckDetails != null) ? this.smtpOutSession.AckDetails.RemoteHostName : null), this.sessionId.ToString(), string.Empty, null);
				}
				this.AckConnection(AckStatus.Retry, smtpResponse, details, reason, failureReason, false);
			}
			this.RemoveConnection();
		}

		private bool TryInitializeSmtpOutSession(IPEndPoint endPoint)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.TryInitializeSmtpOutSession);
			bool result;
			try
			{
				bool flag = false;
				SmtpResponse smtpResponse = SmtpResponse.Empty;
				if (this.TlsConfig.TlsDomains == null || this.TlsConfig.TlsDomains.Count == 0)
				{
					if (this.TlsConfig.TlsAuthLevel != null && this.TlsConfig.TlsAuthLevel.Value.Equals(RequiredTlsAuthLevel.DomainValidation))
					{
						flag = true;
						smtpResponse = SmtpResponse.TlsDomainRequired;
					}
				}
				else if (this.TlsConfig.TlsAuthLevel == null || !this.TlsConfig.TlsAuthLevel.Value.Equals(RequiredTlsAuthLevel.DomainValidation))
				{
					flag = true;
					smtpResponse = SmtpResponse.IncorrectTlsAuthLevel;
				}
				if (flag)
				{
					this.UpdateSmtpSendFailurePerfCounter(SessionSetupFailureReason.ProtocolError);
					this.nextHopConnection.AckConnection(AckStatus.Retry, smtpResponse, null, SessionSetupFailureReason.ProtocolError);
					this.RemoveConnection();
					result = false;
				}
				else
				{
					if (ConfigurationComponent.IsFrontEndTransportProcess(this.transportConfiguration))
					{
						if (this.IsBlindProxy)
						{
							this.smtpOutSession = new SmtpOutProxySession(this.inSession, this, this.sessionId, endPoint, this.protocolLog, this.Connector.ProtocolLoggingLevel, this.certificateCache, this.certificateValidator, this.transportConfiguration, this.transportAppConfig, this.connectionContextString);
						}
						else
						{
							this.smtpOutSession = new InboundProxySmtpOutSession(this.sessionId, this, this.nextHopConnection, endPoint, this.protocolLog, this.Connector.ProtocolLoggingLevel, this.mailRouter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, ((InboundProxyNextHopConnection)this.nextHopConnection).ProxyLayer);
						}
					}
					else if (this.isShadowOut)
					{
						this.smtpOutSession = new ShadowSmtpOutSession(this.sessionId, this, this.nextHopConnection, endPoint, this.protocolLog, this.Connector.ProtocolLoggingLevel, this.mailRouter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, ((ShadowPeerNextHopConnection)this.nextHopConnection).ProxyLayer);
					}
					else if (this.IsBlindProxy)
					{
						this.smtpOutSession = new SmtpOutProxySession(this.inSession, this, this.sessionId, endPoint, this.protocolLog, this.Connector.ProtocolLoggingLevel, this.certificateCache, this.certificateValidator, this.transportConfiguration, this.transportAppConfig, this.connectionContextString);
					}
					else
					{
						ProtocolLoggingLevel loggingLevel = (this.outboundProxyContext == null) ? this.Connector.ProtocolLoggingLevel : this.outboundProxyContext.ProxySendConnector.ProtocolLoggingLevel;
						this.smtpOutSession = new SmtpOutSession(this.sessionId, this, this.nextHopConnection, endPoint, this.protocolLog, loggingLevel, this.mailRouter, this.certificateCache, this.certificateValidator, this.shadowRedundancyManager, this.transportAppConfig, this.transportConfiguration, this.smtpInComponent.TargetRunningState == ServiceState.Inactive);
					}
					result = true;
				}
			}
			catch (TlsCertificateNameNotFoundException arg)
			{
				ConnectionLog.SmtpConnectionAborted(this.sessionId, this.nextHopConnection.Key.NextHopDomain, endPoint.Address);
				SmtpResponse smtpResponse2 = new SmtpResponse("454", "4.7.5", new string[]
				{
					"The certificate specified in TlsCertificateName of the SendConnector could not be found."
				});
				this.nextHopConnection.AckConnection(AckStatus.Retry, smtpResponse2, null);
				this.RemoveConnection();
				ExTraceGlobals.SmtpSendTracer.TraceError<TlsCertificateNameNotFoundException>((long)this.GetHashCode(), "TlsCertificateNameException occurred while trying to handle a new SMTP outbound connection. Exception test: {0}", arg);
				result = false;
			}
			catch (LocalizedException arg2)
			{
				ConnectionLog.SmtpConnectionAborted(this.sessionId, this.nextHopConnection.Key.NextHopDomain, endPoint.Address);
				this.nextHopConnection.AckConnection(AckStatus.Retry, SmtpResponse.UnexpectedExceptionHandlingNewOutboundConnection, null);
				this.RemoveConnection();
				ExTraceGlobals.SmtpSendTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "Unexpected exception occurred when trying to handle a new SMTP outbound connection. Exception text: {0}", arg2);
				result = false;
			}
			return result;
		}

		private bool TryBeginConnectToNextHop(IPEndPoint endPoint)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.TryBeginConnectToNextHop);
			bool result;
			try
			{
				this.socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				if (!Components.IsBridgehead && endPoint.AddressFamily == this.sendConnector.SourceIPAddress.AddressFamily)
				{
					try
					{
						this.socket.Bind(new IPEndPoint(this.sendConnector.SourceIPAddress, 0));
					}
					catch (SocketException ex)
					{
						if (ex.SocketErrorCode != SocketError.AddressNotAvailable)
						{
							throw;
						}
						SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SendConnectorInvalidSourceIPAddress, null, new object[]
						{
							this.sendConnector.SourceIPAddress,
							this.sendConnector.Name
						});
						string notificationReason = string.Format("Failed to bind to SourceIPAddress '{0}' configured on send connector '{1}'.", this.sendConnector.SourceIPAddress, this.sendConnector.Name);
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Warning, false);
					}
				}
				this.socket.BeginConnect(endPoint, SmtpOutConnection.onConnectComplete, this);
				result = true;
			}
			catch (SocketException socketException)
			{
				this.smtpOutTargetHostPicker.HandleSocketError(socketException);
				this.socket.Close();
				this.socket = null;
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendConnectionError, null, new object[]
				{
					this.Connector.Name,
					endPoint
				});
				result = false;
			}
			return result;
		}

		private void ConnectComplete(IAsyncResult asyncResult)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.ConnectComplete);
			try
			{
				this.socket.EndConnect(asyncResult);
				NetworkConnection networkConnection = new NetworkConnection(this.socket, 4096)
				{
					ClientTlsProtocols = SchannelProtocols.Zero
				};
				this.socket = null;
				ExTraceGlobals.SmtpSendTracer.TraceDebug<long, IPEndPoint>((long)this.GetHashCode(), "Connection Completed. Connection ID : {0}, Remote End Point {1}", networkConnection.ConnectionId, networkConnection.RemoteEndPoint);
				if (SmtpOutConnectionHandler.UpdateConnection(this.sessionId, networkConnection))
				{
					ConnectionLog.SmtpConnected(this.sessionId, this.nextHopConnection.Key.NextHopDomain, this.smtpOutTargetHostPicker.CurrentIpAddress);
					this.smtpOutTargetHostPicker.ConnectionSucceeded();
					this.connectionSucceededToNextHop = true;
					this.smtpOutSession.StartUsingConnection();
				}
				else
				{
					networkConnection.Dispose();
					this.smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Server shutdown");
					ConnectionLog.SmtpConnectionAborted(this.sessionId, this.nextHopConnection.Key.NextHopDomain, this.smtpOutTargetHostPicker.CurrentIpAddress);
					this.RemoveConnection();
				}
			}
			catch (SocketException ex)
			{
				this.smtpOutTargetHostPicker.HandleSocketError(ex);
				ExTraceGlobals.SmtpSendTracer.TraceError<SocketException>((long)this.GetHashCode(), "Failed to connect, SocketException:{0}", ex);
				this.socket.Close();
				this.socket = null;
				this.socketErrorDetails = string.Format(CultureInfo.InvariantCulture, "Failed to connect. Winsock error code: {0}, Win32 error code: {1}", new object[]
				{
					ex.ErrorCode,
					ex.NativeErrorCode
				});
				string context = string.Format(CultureInfo.InvariantCulture, "{0}, Error Message: {1}", new object[]
				{
					this.socketErrorDetails,
					ex.Message
				});
				this.smtpOutSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, context);
				this.smtpOutSession.FailoverConnection(SmtpResponse.UnableToConnect, SessionSetupFailureReason.SocketError);
			}
		}

		private void AckConnectionWithDNSError(AckStatus ackStatus, SmtpResponse response, string eventText)
		{
			this.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.AckConnectionWithDnsError);
			switch (ackStatus)
			{
			case AckStatus.Retry:
			case AckStatus.Fail:
				break;
			default:
				if (ackStatus != AckStatus.Resubmit)
				{
					throw new ArgumentException("AckConnectionWithDNSError should only be used for retry or failure.", "ackStatus");
				}
				break;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus, string>((long)this.GetHashCode(), "Acking Connection due to DNS error. Status -> {0} : {1}", ackStatus, eventText);
			SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendDnsConnectionFailure, null, new object[]
			{
				this.nextHopConnection.Key.NextHopConnector.ToString(),
				eventText
			});
			ConnectionLog.SmtpConnectionStop(this.sessionId, this.nextHopConnection.Key.NextHopDomain, eventText, 0UL, 0UL, 0UL);
			this.UpdateSmtpSendFailurePerfCounter(SessionSetupFailureReason.DnsLookupFailure);
			this.nextHopConnection.AckConnection(ackStatus, response, null, SessionSetupFailureReason.DnsLookupFailure);
			this.RemoveConnection();
			this.nextHopConnection = null;
		}

		public const ushort MailboxTransportDeliveryTargetPort = 475;

		public const ushort ColocatedFrontEndAndHubTargetPort = 2525;

		private const string ProtocolFailureResponseFormat = "Primary target IP address responded with: \"{0}.\" Attempted failover to alternate host, but that did not succeed. Either there are no alternate hosts, or delivery failed to all alternate hosts.";

		private const string FrontendProtocolFailureResponseFormat = "Primary outbound frontend IP address responded with:   \"{0}.\"   Attempted failover to alternate frontend address, but that did not succeed. Either there are no alternate frontend addresses, or delivery failed to all alternate frontend addresses.";

		private const string NonProtocolFailureResponseFormat = "Error encountered while communicating with primary target IP address: \"{0}.\" Attempted failover to alternate host, but that did not succeed. Either there are no alternate hosts, or delivery failed to all alternate hosts.";

		private const string FrontendNonProtocolFailureResponseFormat = "Error encountered while communicating with primary outbound frontend IP address:   \"{0}.\"   Attempted failover to alternate frontend, but that did not succeed. Either there are no alternate frontend addresses, or delivery failed to all alternate frontend addresses.";

		private const int NumberOfBreadcrumbs = 64;

		public static ExEventLog Events = new ExEventLog(ExTraceGlobals.SmtpSendTracer.Category, TransportEventLog.GetEventSource());

		public readonly bool ClientProxy;

		private static readonly AsyncCallback onConnectComplete = new AsyncCallback(SmtpOutConnection.OnConnectComplete);

		private readonly int fixedTotalConnectionAttemptCount;

		private readonly int perHostConnectionAttemptCount;

		private readonly ISmtpInSession inSession;

		private readonly bool isShadowOut;

		private readonly string connectionContextString;

		private Breadcrumbs<SmtpOutConnection.SmtpOutConnectionBreadcrumbs> breadcrumbs = new Breadcrumbs<SmtpOutConnection.SmtpOutConnectionBreadcrumbs>(64);

		private SmtpOutTargetHostPicker smtpOutTargetHostPicker;

		private bool connectionSucceededToNextHop;

		private NextHopConnection nextHopConnection;

		private SmtpResponse failoverResponse;

		private SessionSetupFailureReason failoverReason;

		private ProtocolLog protocolLog;

		private SmtpSendConnectorConfig sendConnector;

		private ISmtpOutSession smtpOutSession;

		private Socket socket;

		private ulong bytesSent;

		private ulong messagesSent;

		private ulong discardIdsReceived;

		private ulong sessionId = SessionId.GetNextSessionId();

		private SmtpSendPerfCountersInstance smtpSendPerformanceCounters;

		private int totalConnectionsAttempted;

		private int connectionsAttemptedToCurrentTarget;

		private bool connectionAdded;

		private IMailRouter mailRouter;

		private CertificateCache certificateCache;

		private CertificateValidator certificateValidator;

		private ShadowRedundancyManager shadowRedundancyManager;

		private TransportAppConfig transportAppConfig;

		private ITransportConfiguration transportConfiguration;

		private ISmtpInComponent smtpInComponent;

		private SmtpOutConnection.OutboundProxyContext outboundProxyContext;

		private IPEndPoint currentTargetEndpoint;

		private bool createSmtpSendPerfCounters = true;

		private SessionSetupFailureReason lastFailoverReason;

		private readonly UnhealthyTargetFilterComponent unhealthyTargetFilter;

		private string socketErrorDetails;

		public enum SmtpOutConnectionBreadcrumbs
		{
			EMPTY,
			AckConnection,
			AckConnectionForResubmitWithoutHighAvailability,
			AckConnectionWithDnsError,
			AddConnection,
			Connect,
			ConnectComplete,
			ConnectToBlindProxyDestinations,
			ConnectToNextHost,
			ConnectToPerMessageProxyDestinations,
			ConnectToShadowDestinations,
			FailoverConnection,
			GetOutboundProxyDestinationSettings,
			HandleConnectionToAllTargetsFailed,
			NextHopResolutionFailed,
			RemoveConnection,
			RoutingTableUpdate,
			SetOutboundProxyContext,
			SetSendConnector,
			Shutdown,
			TryBeginConnectToNextHop,
			TryGetRemainingSmtpTargets,
			TryInitializeSmtpOutSession,
			TryUsingCachedSmtpOutSession,
			UpdateOnSuccessfulOutboundProxySetup,
			SthpConnectAfterDns,
			SthpConnectionDisconnected,
			SthpConnectionSucceeded,
			SthpGetNextTargetToConnect,
			SthpHandleSocketError,
			SthpResolveProxyNextHopAndConnect,
			SthpResolveToNextHopAndConnect,
			SthpStartOverForRetry,
			SthpTryMarkCurrentSmtpTargetInConnectingState,
			SthpUpdateSessionId
		}

		private class OutboundProxyContext
		{
			public OutboundProxyContext(IEnumerable<INextHopServer> proxyDestinations, TlsSendConfiguration proxyTlsConfiguration, SmtpSendConnectorConfig proxySendConnector, RiskLevel proxyRiskLevel, int proxyOutboundIPPool)
			{
				if (proxyDestinations == null)
				{
					throw new ArgumentNullException("proxyDestinations");
				}
				if (proxyTlsConfiguration == null)
				{
					throw new ArgumentNullException("proxyTlsConfiguration");
				}
				if (proxySendConnector == null)
				{
					throw new ArgumentNullException("proxySendConnector");
				}
				this.ProxyDestinations = proxyDestinations;
				this.ProxyTlsConfiguration = proxyTlsConfiguration;
				this.proxySendConnector = proxySendConnector;
				this.proxyRiskLevel = proxyRiskLevel;
				this.proxyOutboundIPPool = proxyOutboundIPPool;
			}

			public SmtpSendConnectorConfig ProxySendConnector
			{
				get
				{
					return this.proxySendConnector;
				}
			}

			public RiskLevel ProxyRiskLevel
			{
				get
				{
					return this.proxyRiskLevel;
				}
			}

			public int OutboundIPPool
			{
				get
				{
					return this.proxyOutboundIPPool;
				}
			}

			public IEnumerable<INextHopServer> ProxyDestinations;

			public TlsSendConfiguration ProxyTlsConfiguration;

			private readonly SmtpSendConnectorConfig proxySendConnector;

			private readonly RiskLevel proxyRiskLevel;

			private readonly int proxyOutboundIPPool;
		}
	}
}
