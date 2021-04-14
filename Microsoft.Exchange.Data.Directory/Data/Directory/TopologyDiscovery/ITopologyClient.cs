using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(Namespace = "http://Microsoft.Exchange.Directory.TopologyService", ConfigurationName = "Microsoft.Exchange.Directory.TopologyService.ITopologyService")]
	internal interface ITopologyClient : ITopologyService
	{
		[FaultContract(typeof(InvalidOperationException))]
		[FaultContract(typeof(ArgumentException))]
		[OperationContract(Name = "GetServersForRole", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServersForRole", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServersForRoleResponse")]
		[FaultContract(typeof(ArgumentNullException))]
		[FaultContract(typeof(TopologyServiceFault))]
		List<ServerInfo> GetServersForRole(string partitionFqdn, List<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false);

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(ArgumentNullException))]
		[FaultContract(typeof(InvalidOperationException))]
		[FaultContract(typeof(TopologyServiceFault))]
		[OperationContract(Name = "GetServerFromDomainDN", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServerFromDomainDN", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/GetServerFromDomainDNResponse")]
		ServerInfo GetServerFromDomainDN(string domainDN);

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract(Name = "SetConfigDC", Action = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/SetConfigDC", ReplyAction = "http://Microsoft.Exchange.Directory.TopologyService/ITopologyService/SetConfigDCResponse")]
		[FaultContract(typeof(ArgumentNullException))]
		[FaultContract(typeof(TopologyServiceFault))]
		void SetConfigDC(string partitionFqdn, string serverName);
	}
}
