using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IAmCluster : IDisposable
	{
		string Name { get; }

		string CnoName { get; }

		AmClusterHandle Handle { get; }

		bool IsRefreshRequired { get; }

		bool IsLocalNodeUp { get; }

		void DestroyExchangeCluster(IClusterSetupProgress setupProgress, IntPtr context, out Exception errorException, bool throwExceptionOnFailure);

		IAmClusterNode OpenNode(AmServerName nodeName);

		IEnumerable<IAmClusterNode> EnumerateNodes();

		AmNodeState GetNodeState(AmServerName nodeName, out Exception ex);

		void AddNodeToCluster(AmServerName nodeName, IClusterSetupProgress setupProgress, IntPtr context, out Exception errorException, bool throwExceptionOnFailure);

		void EvictNodeFromCluster(AmServerName nodeName);

		IAmClusterGroup FindCoreClusterGroup();

		AmClusterResource OpenQuorumResource();

		string GetQuorumResourceInformation(out string outDeviceName, out uint outMaxQuorumLogSize);

		void SetQuorumResource(IAmClusterResource newQuorum, string deviceName, uint maxLogSize);

		void ClearQuorumResource();

		AmClusterResource OpenResource(string resourceName);

		IEnumerable<AmClusterNetwork> EnumerateNetworks();

		AmClusterNetwork OpenNetwork(string networkName);

		AmClusterNetwork FindNetworkByName(string networkName, IPVersion ipVer);

		AmClusterNetwork FindNetworkByIPv4Address(IPAddress ipAddr);

		AmClusterNetInterface OpenNetInterface(string nicName);

		AmNetInterfaceState GetNetInterfaceState(string nicName, out Exception ex);
	}
}
