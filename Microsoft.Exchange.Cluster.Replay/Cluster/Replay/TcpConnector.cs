using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TcpConnector : ITcpConnector
	{
		public TcpClientChannel TryConnect(NetworkPath netPath, out NetworkTransportException failureEx)
		{
			return this.TryConnect(netPath, DagNetEnvironment.ConnectTimeoutInSec * 1000, out failureEx);
		}

		public TcpClientChannel TryConnect(NetworkPath netPath, int timeoutInMsec, out NetworkTransportException failureEx)
		{
			TcpClientChannel result = null;
			TcpClientChannel.TryOpenChannel(netPath, timeoutInMsec, out result, out failureEx);
			return result;
		}

		public NetworkPath BuildDnsNetworkPath(string targetServer, int replicationPort)
		{
			try
			{
				IPAddress ipaddress = NetworkManager.ChooseAddressFromDNS(targetServer);
				if (ipaddress != null)
				{
					return new NetworkPath(targetServer, ipaddress, replicationPort, null)
					{
						NetworkChoiceIsMandatory = true
					};
				}
			}
			catch (SocketException ex)
			{
				throw new NetworkTransportException(ReplayStrings.NetworkAddressResolutionFailed(targetServer, ex.Message), ex);
			}
			throw new NetworkTransportException(ReplayStrings.NetworkAddressResolutionFailedNoDnsEntry(targetServer));
		}

		public NetworkPath ChooseDagNetworkPath(string targetName, string networkName, NetworkPath.ConnectionPurpose purpose)
		{
			return NetworkManager.InternalChooseDagNetworkPath(targetName, networkName, purpose);
		}

		public TcpClientChannel OpenChannel(string targetServerName, ISimpleBufferPool socketStreamBufferPool, IPool<SocketStreamAsyncArgs> socketStreamAsyncArgPool, SocketStream.ISocketStreamPerfCounters perfCtrs, out NetworkPath netPath)
		{
			DagNetConfig dagConfig;
			DagNetRoute[] array = DagNetChooser.ProposeRoutes(targetServerName, out dagConfig);
			TcpClientChannel tcpClientChannel = null;
			netPath = null;
			NetworkTransportException ex = null;
			if (array != null)
			{
				foreach (DagNetRoute dagNetRoute in array)
				{
					netPath = new NetworkPath(targetServerName, dagNetRoute.TargetIPAddr, dagNetRoute.TargetPort, dagNetRoute.SourceIPAddr);
					netPath.CrossSubnet = dagNetRoute.IsCrossSubnet;
					this.ApplySocketStreamArgs(netPath, socketStreamBufferPool, socketStreamAsyncArgPool, perfCtrs);
					netPath.ApplyNetworkPolicy(dagConfig);
					tcpClientChannel = this.TryConnect(netPath, out ex);
					if (tcpClientChannel != null)
					{
						break;
					}
				}
			}
			if (tcpClientChannel == null)
			{
				netPath = this.BuildDnsNetworkPath(targetServerName, (int)NetworkManager.GetReplicationPort());
				this.ApplySocketStreamArgs(netPath, socketStreamBufferPool, socketStreamAsyncArgPool, perfCtrs);
				netPath.ApplyNetworkPolicy(dagConfig);
				tcpClientChannel = this.TryConnect(netPath, out ex);
				if (tcpClientChannel == null)
				{
					throw ex;
				}
			}
			return tcpClientChannel;
		}

		private void ApplySocketStreamArgs(NetworkPath netPath, ISimpleBufferPool socketStreamBufferPool, IPool<SocketStreamAsyncArgs> socketStreamAsyncArgPool, SocketStream.ISocketStreamPerfCounters perfCtrs)
		{
			if (socketStreamBufferPool != null)
			{
				netPath.SocketStreamBufferPool = socketStreamBufferPool;
				netPath.SocketStreamAsyncArgPool = socketStreamAsyncArgPool;
				netPath.SocketStreamPerfCounters = perfCtrs;
				netPath.UseSocketStream = true;
			}
		}

		public bool ForceSocketStream { get; set; }
	}
}
