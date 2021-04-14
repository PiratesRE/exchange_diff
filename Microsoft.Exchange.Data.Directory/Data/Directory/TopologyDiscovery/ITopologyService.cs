using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[ServiceContract(Namespace = "http://Microsoft.Exchange.Directory.TopologyService", ConfigurationName = "Microsoft.Exchange.Directory.TopologyService.ITopologyService")]
	internal interface ITopologyService
	{
		[OperationContract(Name = "GetExchangeTopology", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetExchangeTopology", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetExchangeTopologyResponse")]
		[FaultContract(typeof(TopologyServiceFault))]
		byte[][] GetExchangeTopology(DateTime currentTopologyTimestamp, ExchangeTopologyScope topologyScope, bool forceRefresh);

		[FaultContract(typeof(TopologyServiceFault))]
		[OperationContract(Name = "GetServiceVersion", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServiceVersion", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServiceVersionResponse")]
		ServiceVersion GetServiceVersion();

		[OperationContract(Name = "GetAllTopologyVersions", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetAllTopologyVersions", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetAllTopologyVersionsResponse")]
		[FaultContract(typeof(TopologyServiceFault))]
		List<TopologyVersion> GetAllTopologyVersions();

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(TopologyServiceFault))]
		[OperationContract(Name = "GetTopologyVersions", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetTopologyVersions", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetTopologyVersionsResponse")]
		[FaultContract(typeof(ArgumentNullException))]
		List<TopologyVersion> GetTopologyVersions(List<string> partitionFqdns);

		[OperationContract(Name = "GetServersForRole", AsyncPattern = true, Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServersForRole", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServersForRoleResponse")]
		IAsyncResult BeginGetServersForRole(string partitionFqdn, List<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested, AsyncCallback callback, object asyncState);

		List<ServerInfo> EndGetServersForRole(IAsyncResult result);

		[OperationContract(Name = "GetServerFromDomainDN", AsyncPattern = true, Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServerFromDomainDN", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServerFromDomainDNResponse")]
		IAsyncResult BeginGetServerFromDomainDN(string domainDN, AsyncCallback callback, object asyncState);

		ServerInfo EndGetServerFromDomainDN(IAsyncResult result);

		[OperationContract(Name = "SetConfigDC", AsyncPattern = true, Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/SetConfigDC", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/SetConfigDCResponse")]
		IAsyncResult BeginSetConfigDC(string partitionFqdn, string serverName, AsyncCallback callback, object asyncState);

		void EndSetConfigDC(IAsyncResult result);

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract(Name = "ReportServerDown", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/ReportServerDown")]
		[FaultContract(typeof(TopologyServiceFault))]
		[FaultContract(typeof(ArgumentNullException))]
		void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role);
	}
}
