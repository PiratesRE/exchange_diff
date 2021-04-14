using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IEnhancedDns
	{
		SmtpSendConnectorConfig EnterpriseRelayConnector { get; }

		SmtpSendConnectorConfig ClientProxyConnector { get; }

		int MaxDataPerRequest { get; set; }

		DnsServerList ServerList { get; set; }

		IEnumerable<IPAddress> LocalIPAddresses { get; set; }

		TimeSpan Timeout { get; set; }

		TimeSpan QueryRetryInterval { get; set; }

		AddressFamily DefaultAddressFamily { get; set; }

		DnsQueryOptions Options { get; set; }

		void SetRunTimeDependencies(IMailRouter router);

		void Load();

		void Unload();

		string OnUnhandledException(Exception e);

		void FlushCache();

		IAsyncResult BeginResolveToNextHop(NextHopSolutionKey key, RiskLevel riskLevel, int outboundIPPool, AsyncCallback requestCallback, object stateObject);

		IAsyncResult BeginResolveProxyNextHop(IEnumerable<INextHopServer> destinations, bool internalDestination, SmtpSendConnectorConfig sendConnector, SmtpOutProxyType proxyType, RiskLevel riskLevel, int outboundIPPool, AsyncCallback requestCallback, object stateObject);

		void HandleTransportServerConfigChange(TransportServerConfiguration args);

		string GetDiagnosticComponentName();

		XElement GetDiagnosticInfo(DiagnosableParameters parameters);

		void AdapterServerList(Guid adapterGuid);

		void AdapterServerList(Guid adapterGuid, bool excludeServersFromLoopbackAdapters, bool excludeIPv6SiteLocalAddresses);

		void InitializeFromMachineServerList();

		void InitializeServerList(IPAddress[] addresses);

		IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToAddresses(string domainName, AddressFamily type, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToAddresses(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToMailServers(string domainName, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToMailServers(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToMailServers(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToMailServers(string domainName, bool implicitSearch, AddressFamily type, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveTextRecords(string domainName, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveTextRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveTextRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveSoaRecords(string domainName, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveSoaRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveSoaRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveCNameRecords(string domainName, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveCNameRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveCNameRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToNames(IPAddress address, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToNames(IPAddress address, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginResolveToNames(IPAddress address, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveSrvRecords(string name, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveNsRecords(string domainName, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveNsRecords(string domainName, DnsQueryOptions options, AsyncCallback requestCallback, object state);

		IAsyncResult BeginRetrieveNsRecords(string domainName, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object state);
	}
}
