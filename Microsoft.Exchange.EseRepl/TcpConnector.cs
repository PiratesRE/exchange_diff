using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.EseRepl
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
				IPAddress ipaddress = TcpConnector.ChooseAddressFromDNS(targetServer);
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
				throw new NetworkTransportException(Strings.NetworkAddressResolutionFailed(targetServer, ex.Message), ex);
			}
			throw new NetworkTransportException(Strings.NetworkAddressResolutionFailedNoDnsEntry(targetServer));
		}

		public NetworkPath ChooseDagNetworkPath(string targetServerName, string networkName, NetworkPath.ConnectionPurpose purpose)
		{
			DagNetConfig dagConfig;
			DagNetRoute[] array = DagNetEnvironment.NetChooser.BuildRoutes(targetServerName, false, networkName, out dagConfig);
			NetworkPath networkPath = null;
			if (array != null)
			{
				foreach (DagNetRoute dagNetRoute in array)
				{
					networkPath = new NetworkPath(targetServerName, dagNetRoute.TargetIPAddr, dagNetRoute.TargetPort, dagNetRoute.SourceIPAddr);
					networkPath.CrossSubnet = dagNetRoute.IsCrossSubnet;
					networkPath.ApplyNetworkPolicy(dagConfig);
				}
			}
			return networkPath;
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
				netPath = this.BuildDnsNetworkPath(targetServerName, this.GetCurrentReplicationPort());
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

		private static NetworkPath BuildDnsNetworkPath(string targetName, ushort replicationPort)
		{
			try
			{
				IPAddress ipaddress = TcpConnector.ChooseAddressFromDNS(targetName);
				if (ipaddress != null)
				{
					return new NetworkPath(targetName, ipaddress, (int)replicationPort, null)
					{
						NetworkChoiceIsMandatory = true
					};
				}
			}
			catch (SocketException ex)
			{
				throw new NetworkTransportException(Strings.NetworkAddressResolutionFailed(targetName, ex.Message), ex);
			}
			throw new NetworkTransportException(Strings.NetworkAddressResolutionFailedNoDnsEntry(targetName));
		}

		public static IPAddress ChooseAddressFromDNS(string targetName)
		{
			Exception ex;
			IPAddress[] dnsAddresses = NetworkUtil.GetDnsAddresses(targetName, ref ex);
			foreach (IPAddress ipaddress in dnsAddresses)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return ipaddress;
				}
			}
			foreach (IPAddress ipaddress2 in dnsAddresses)
			{
				if (ipaddress2.AddressFamily == AddressFamily.InterNetworkV6)
				{
					return ipaddress2;
				}
			}
			return null;
		}

		private int GetCurrentReplicationPort()
		{
			DagNetConfig dagNetConfig = DagNetEnvironment.FetchLastKnownNetConfig();
			return dagNetConfig.ReplicationPort;
		}

		public bool ForceSocketStream { get; set; }

		private static readonly Trace Tracer = ExTraceGlobals.NetPathTracer;
	}
}
