using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpOutTargetHostPicker
	{
		internal SmtpOutTargetHostPicker(ulong sessionId, SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection, EnhancedDns enhancedDns, UnhealthyTargetFilter<IPAddressPortPair> unhealthyTargetIpAddressFilter, UnhealthyTargetFilter<FqdnPortPair> unhealthyTargetFqdnFilter)
		{
			this.sessionId = sessionId;
			this.smtpOutConnection = smtpOutConnection;
			this.nextHopConnection = nextHopConnection;
			this.enhancedDns = enhancedDns;
			this.unhealthyTargetIpAddressFilter = unhealthyTargetIpAddressFilter;
			this.unhealthyTargetFqdnFilter = unhealthyTargetFqdnFilter;
		}

		internal string SmtpHostName
		{
			get
			{
				return this.currentSmtpTarget.TargetHostName ?? string.Empty;
			}
		}

		internal string SmtpHost
		{
			get
			{
				if (string.IsNullOrEmpty(this.SmtpHostName))
				{
					return this.currentSmtpTarget.Address.ToString();
				}
				return this.SmtpHostName;
			}
		}

		internal IPAddress CurrentIpAddress
		{
			get
			{
				return this.currentSmtpTarget.Address;
			}
		}

		internal SmtpOutTargetHostPicker.SmtpTarget CurrentSmtpTarget
		{
			get
			{
				return this.currentSmtpTarget;
			}
		}

		internal int TotalTargets
		{
			get
			{
				return this.totalTargets;
			}
		}

		private bool ShouldConnect
		{
			get
			{
				return this.smtpOutConnection.IsBlindProxy || this.nextHopConnection.GetNextMailItem() != null;
			}
		}

		internal void RoutingTableUpdate()
		{
			this.routingTableChange = true;
		}

		internal void UpdateSessionId(ulong sessionId)
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpUpdateSessionId);
			this.sessionId = sessionId;
		}

		internal void ResolveToNextHopAndConnect()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpResolveToNextHopAndConnect);
			this.routingTableChange = false;
			this.asyncResult = this.enhancedDns.BeginResolveToNextHop(this.nextHopConnection.Key, this.smtpOutConnection.RiskLevel, this.smtpOutConnection.OutboundIPPool, new AsyncCallback(this.ConnectAfterDNS), null);
		}

		internal void ResolveProxyNextHopAndConnect(IEnumerable<INextHopServer> destinations, bool internalDestination, SmtpOutProxyType proxyType)
		{
			this.ResolveProxyNextHopAndConnect(destinations, internalDestination, null, proxyType);
		}

		internal void ResolveProxyNextHopAndConnect(IEnumerable<INextHopServer> destinations, bool internalDestinations, SmtpSendConnectorConfig sendConnector)
		{
			this.ResolveProxyNextHopAndConnect(destinations, internalDestinations, sendConnector, SmtpOutProxyType.Blind);
		}

		internal void StartOverForRetry()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpStartOverForRetry);
			if (Components.ShuttingDown)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Already shutting down");
			}
			lock (this.dnsUpdateLock)
			{
				this.currentTargetIndex = 0;
				this.currentIpAddressIndex = -1;
			}
		}

		internal bool TryGetRemainingSmtpTargets(out IEnumerable<INextHopServer> remainingTargets)
		{
			List<INextHopServer> list = new List<INextHopServer>();
			remainingTargets = list;
			bool result;
			lock (this.dnsUpdateLock)
			{
				int num = this.currentIpAddressIndex;
				int num2 = this.currentTargetIndex;
				for (;;)
				{
					num++;
					if (num >= this.smtpTargetHosts[num2].IPAddresses.Count)
					{
						num2++;
						num = -1;
						if (num2 >= this.smtpTargetHosts.Length)
						{
							break;
						}
					}
					else
					{
						IPAddress ipaddress = this.smtpTargetHosts[num2].IPAddresses[num];
						if (Socket.OSSupportsIPv6 || ipaddress.AddressFamily != AddressFamily.InterNetworkV6)
						{
							list.Add(new SmtpOutTargetHostPicker.SmtpOutNextHopServer(ipaddress));
						}
					}
				}
				result = (list.Count != 0);
			}
			return result;
		}

		internal SmtpOutTargetHostPicker.SmtpTarget GetNextTargetToConnect()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpGetNextTargetToConnect);
			IPAddress ipaddress;
			string name;
			ushort port;
			lock (this.dnsUpdateLock)
			{
				for (;;)
				{
					this.currentIpAddressIndex++;
					if (this.currentIpAddressIndex >= this.smtpTargetHosts[this.currentTargetIndex].IPAddresses.Count)
					{
						this.currentTargetIndex++;
						this.currentIpAddressIndex = -1;
						if (this.currentTargetIndex >= this.smtpTargetHosts.Length)
						{
							break;
						}
					}
					else
					{
						ipaddress = this.smtpTargetHosts[this.currentTargetIndex].IPAddresses[this.currentIpAddressIndex];
						if (Socket.OSSupportsIPv6 || ipaddress.AddressFamily != AddressFamily.InterNetworkV6)
						{
							goto IL_A8;
						}
					}
				}
				return null;
				IL_A8:
				name = this.smtpTargetHosts[this.currentTargetIndex].Name;
				port = this.smtpTargetHosts[this.currentTargetIndex].Port;
			}
			this.currentSmtpTarget = new SmtpOutTargetHostPicker.SmtpTarget(ipaddress, name, port);
			return this.currentSmtpTarget;
		}

		internal bool TryMarkCurrentSmtpTargetInConnectingState()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpTryMarkCurrentSmtpTargetInConnectingState);
			string smtpHost = this.SmtpHost;
			IPAddress currentIpAddress = this.CurrentIpAddress;
			IPAddressPortPair key = new IPAddressPortPair(currentIpAddress, this.CurrentSmtpTarget.Port);
			bool flag = this.unhealthyTargetIpAddressFilter.TryMarkTargetInConnectingState(key);
			bool flag2 = flag && this.unhealthyTargetFqdnFilter.TryMarkTargetInConnectingState(new FqdnPortPair(smtpHost, this.CurrentSmtpTarget.Port));
			if (flag && !flag2)
			{
				this.unhealthyTargetIpAddressFilter.UnMarkTargetInConnectingState(key);
			}
			return flag && flag2;
		}

		internal void ConnectionSucceeded()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpConnectionSucceeded);
			string smtpHost = this.SmtpHost;
			IPAddress currentIpAddress = this.CurrentIpAddress;
			this.unhealthyTargetFqdnFilter.IncrementConnectionCountToTarget(new FqdnPortPair(smtpHost, this.CurrentSmtpTarget.Port));
			this.unhealthyTargetIpAddressFilter.IncrementConnectionCountToTarget(new IPAddressPortPair(currentIpAddress, this.CurrentSmtpTarget.Port));
		}

		internal void ConnectionDisconnected()
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpConnectionDisconnected);
			string smtpHost = this.SmtpHost;
			IPAddress currentIpAddress = this.CurrentIpAddress;
			this.unhealthyTargetFqdnFilter.DecrementConnectionCountToTarget(new FqdnPortPair(smtpHost, this.CurrentSmtpTarget.Port));
			this.unhealthyTargetIpAddressFilter.DecrementConnectionCountToTarget(new IPAddressPortPair(currentIpAddress, this.CurrentSmtpTarget.Port));
		}

		internal void HandleSocketError(SocketException socketException)
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpHandleSocketError);
			string smtpHost = this.SmtpHost;
			IPAddress currentIpAddress = this.CurrentIpAddress;
			int errorCode = socketException.ErrorCode;
			if (this.unhealthyTargetFqdnFilter.Enabled || this.unhealthyTargetIpAddressFilter.Enabled)
			{
				bool targetHostMarkedUnhealthy = false;
				int minValue = int.MinValue;
				int minValue2 = int.MinValue;
				ExDateTime minValue3 = ExDateTime.MinValue;
				ExDateTime ipAddressNextRetryTime;
				int currentIpAddressConnectionCount;
				int currentIpAddressFailureCount;
				bool flag = this.unhealthyTargetIpAddressFilter.TryMarkTargetUnhealthyIfNoConnectionOpen(new IPAddressPortPair(currentIpAddress, this.CurrentSmtpTarget.Port), out ipAddressNextRetryTime, out currentIpAddressConnectionCount, out currentIpAddressFailureCount);
				if (flag && this.currentIpAddressIndex == this.smtpTargetHosts[this.currentTargetIndex].IPAddresses.Count - 1)
				{
					targetHostMarkedUnhealthy = this.unhealthyTargetFqdnFilter.TryMarkTargetUnhealthyIfNoConnectionOpen(new FqdnPortPair(smtpHost, this.CurrentSmtpTarget.Port), out minValue3, out minValue, out minValue2);
				}
				else
				{
					this.unhealthyTargetFqdnFilter.UnMarkTargetInConnectingState(new FqdnPortPair(smtpHost, this.CurrentSmtpTarget.Port));
				}
				ConnectionLog.SmtpConnectionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, currentIpAddress, smtpHost, this.CurrentSmtpTarget.Port, flag, ipAddressNextRetryTime, currentIpAddressConnectionCount, currentIpAddressFailureCount, targetHostMarkedUnhealthy, minValue3, minValue, minValue2, socketException);
				return;
			}
			ConnectionLog.SmtpConnectionFailed(this.sessionId, this.nextHopConnection.Key.NextHopDomain, currentIpAddress, this.CurrentSmtpTarget.Port, socketException);
		}

		private void ResolveProxyNextHopAndConnect(IEnumerable<INextHopServer> destinations, bool internalDestination, SmtpSendConnectorConfig connector, SmtpOutProxyType proxyType)
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpResolveProxyNextHopAndConnect);
			this.routingTableChange = false;
			this.internalDestination = internalDestination;
			this.proxyDestinations = destinations;
			this.blindProxySendConnector = connector;
			this.proxyType = proxyType;
			this.asyncResult = this.enhancedDns.BeginResolveProxyNextHop(destinations, internalDestination, connector, proxyType, this.smtpOutConnection.RiskLevel, this.smtpOutConnection.OutboundIPPool, new AsyncCallback(this.ConnectAfterDNS), null);
		}

		private void ConnectAfterDNS(IAsyncResult ar)
		{
			this.smtpOutConnection.DropBreadcrumb(SmtpOutConnection.SmtpOutConnectionBreadcrumbs.SthpConnectAfterDns);
			EnhancedDnsTargetHost[] array;
			IEnumerable<INextHopServer> outboundProxyDestinations;
			SmtpSendConnectorConfig smtpSendConnectorConfig;
			SmtpSendConnectorConfig smtpSendConnectorConfig2;
			IPAddress reportingServer;
			string diagnosticInfo;
			DnsStatus dnsStatus = EnhancedDns.EndResolveToNextHop(ar, out array, out outboundProxyDestinations, out smtpSendConnectorConfig, out smtpSendConnectorConfig2, out reportingServer, out diagnosticInfo);
			this.asyncResult = null;
			if (!this.smtpOutConnection.AddConnection())
			{
				this.smtpOutConnection.AckConnection(AckStatus.Retry, SmtpResponse.Empty, null, "Connection not attempted because service is shutting down", SessionSetupFailureReason.Shutdown);
				return;
			}
			SmtpSendConnectorConfig smtpSendConnectorConfig3 = smtpSendConnectorConfig2 ?? smtpSendConnectorConfig;
			SmtpSendConnectorConfig smtpSendConnectorConfig4 = (smtpSendConnectorConfig2 != null) ? smtpSendConnectorConfig : null;
			if (smtpSendConnectorConfig3 != null)
			{
				this.smtpOutConnection.SetSendConnector(smtpSendConnectorConfig3, smtpSendConnectorConfig4, outboundProxyDestinations);
			}
			if (dnsStatus != DnsStatus.Success)
			{
				this.smtpOutConnection.NextHopResolutionFailed(dnsStatus, reportingServer, diagnosticInfo);
				return;
			}
			if (smtpSendConnectorConfig3 == null)
			{
				throw new InvalidOperationException("Successful resolution should return non-null connector");
			}
			if (array.Length == 0)
			{
				throw new InvalidOperationException("Successful resolution should return at least 1 target host");
			}
			lock (this.dnsUpdateLock)
			{
				this.smtpTargetHosts = array;
				this.currentTargetIndex = 0;
				this.currentIpAddressIndex = -1;
			}
			ConnectionLog.SmtpHostResolved(this.sessionId, this.nextHopConnection.Key.NextHopDomain, array, smtpSendConnectorConfig4 != null);
			if (!this.ShouldConnect)
			{
				this.smtpOutConnection.AckConnection(AckStatus.Pending, SmtpResponse.Empty, null, null, SessionSetupFailureReason.None);
				this.smtpOutConnection.RemoveConnection();
				return;
			}
			if (Components.ShuttingDown)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Already shutting down");
				this.smtpOutConnection.RemoveConnection();
				return;
			}
			if (this.routingTableChange)
			{
				if (Interlocked.Exchange(ref this.numberOfDnsQuery, 1) == 0)
				{
					if (this.proxyDestinations == null)
					{
						this.ResolveToNextHopAndConnect();
						return;
					}
					if (this.blindProxySendConnector == null)
					{
						this.ResolveProxyNextHopAndConnect(this.proxyDestinations, this.internalDestination, this.proxyType);
						return;
					}
					this.ResolveProxyNextHopAndConnect(this.proxyDestinations, this.internalDestination, this.blindProxySendConnector);
				}
				return;
			}
			Interlocked.Exchange(ref this.numberOfDnsQuery, 0);
			foreach (EnhancedDnsTargetHost enhancedDnsTargetHost in this.smtpTargetHosts)
			{
				this.totalTargets += enhancedDnsTargetHost.IPAddresses.Count;
			}
			this.smtpOutConnection.ConnectToNextHost();
		}

		private EnhancedDns enhancedDns;

		private UnhealthyTargetFilter<IPAddressPortPair> unhealthyTargetIpAddressFilter;

		private UnhealthyTargetFilter<FqdnPortPair> unhealthyTargetFqdnFilter;

		private SmtpOutConnection smtpOutConnection;

		private EnhancedDnsTargetHost[] smtpTargetHosts;

		private IEnumerable<INextHopServer> proxyDestinations;

		private SmtpSendConnectorConfig blindProxySendConnector;

		private SmtpOutProxyType proxyType;

		private bool internalDestination;

		private int currentTargetIndex;

		private int currentIpAddressIndex = -1;

		private ulong sessionId;

		private object dnsUpdateLock = new object();

		private int numberOfDnsQuery;

		private NextHopConnection nextHopConnection;

		private bool routingTableChange;

		private SmtpOutTargetHostPicker.SmtpTarget currentSmtpTarget;

		private IAsyncResult asyncResult;

		private int totalTargets;

		internal class SmtpTarget
		{
			public SmtpTarget(IPAddress address, string targetHostName, ushort port)
			{
				this.address = address;
				this.targetHostName = targetHostName;
				this.port = port;
			}

			internal IPAddress Address
			{
				get
				{
					return this.address;
				}
			}

			internal string TargetHostName
			{
				get
				{
					return this.targetHostName;
				}
			}

			internal ushort Port
			{
				get
				{
					return this.port;
				}
			}

			private readonly IPAddress address;

			private readonly string targetHostName;

			private readonly ushort port;
		}

		private class SmtpOutNextHopServer : INextHopServer
		{
			public SmtpOutNextHopServer(IPAddress address)
			{
				this.address = address;
			}

			public bool IsIPAddress
			{
				get
				{
					return true;
				}
			}

			public IPAddress Address
			{
				get
				{
					return this.address;
				}
			}

			public string Fqdn
			{
				get
				{
					return null;
				}
			}

			public bool IsFrontendAndHubColocatedServer
			{
				get
				{
					return false;
				}
			}

			private readonly IPAddress address;
		}
	}
}
