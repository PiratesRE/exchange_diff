using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TcpServerChannel : TcpChannel
	{
		public string NetworkName
		{
			get
			{
				return this.m_networkName;
			}
		}

		public string ClientNodeName
		{
			get
			{
				return this.m_clientNodeName;
			}
		}

		public bool ClientIsDagMember
		{
			get
			{
				return this.m_networkName != null;
			}
		}

		public ExchangeNetworkPerfmonCounters PerfCounters
		{
			get
			{
				return this.m_networkPerfCounters;
			}
		}

		private TcpServerChannel(Socket socket, NegotiateStream stream, int ioTimeoutInMSec, TimeSpan idleLimit) : base(socket, stream, ioTimeoutInMSec, idleLimit)
		{
			IPEndPoint ipendPoint = (IPEndPoint)socket.RemoteEndPoint;
			NetworkEndPoint networkEndPoint;
			ExchangeNetwork exchangeNetwork = NetworkManager.LookupEndPoint(ipendPoint.Address, out networkEndPoint);
			if (exchangeNetwork != null)
			{
				this.m_networkName = exchangeNetwork.Name;
				this.m_clientNodeName = networkEndPoint.NodeName;
				this.m_networkPerfCounters = exchangeNetwork.PerfCounters;
				base.PartnerNodeName = this.m_clientNodeName;
				ExTraceGlobals.TcpChannelTracer.TraceDebug<string, string, EndPoint>((long)this.GetHashCode(), "Opening server channel with DAG member {0} on network {1} from ip {2}", this.m_clientNodeName, this.m_networkName, socket.RemoteEndPoint);
			}
			else
			{
				base.PartnerNodeName = socket.RemoteEndPoint.ToString();
				ExTraceGlobals.TcpChannelTracer.TraceDebug<EndPoint>((long)this.GetHashCode(), "Opening server channel with unknown client on ip {0}", socket.RemoteEndPoint);
			}
			ReplayCrimsonEvents.ServerNetworkConnectionOpen.Log<string, string, string>(base.PartnerNodeName, base.RemoteEndpointString, base.LocalEndpointString);
		}

		public static TcpServerChannel AuthenticateAsServer(TcpListener listener, Socket connection)
		{
			TcpServerChannel tcpServerChannel = null;
			int iotimeoutInMSec = listener.ListenerConfig.IOTimeoutInMSec;
			NegotiateStream negotiateStream = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				NetworkStream networkStream = new NetworkStream(connection, false);
				disposeGuard.Add<NetworkStream>(networkStream);
				negotiateStream = new NegotiateStream(networkStream, false);
				disposeGuard.Add<NegotiateStream>(negotiateStream);
				negotiateStream.WriteTimeout = iotimeoutInMSec;
				negotiateStream.ReadTimeout = iotimeoutInMSec;
				negotiateStream.AuthenticateAsServer(CredentialCache.DefaultNetworkCredentials, ProtectionLevel.None, TokenImpersonationLevel.Identification);
				if (!negotiateStream.IsAuthenticated)
				{
					string text = "Authentication failed";
					ExTraceGlobals.TcpServerTracer.TraceError((long)connection.GetHashCode(), text);
					ReplayCrimsonEvents.ServerSideConnectionFailure.LogPeriodic<string, string, string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, connection.RemoteEndPoint.ToString(), connection.LocalEndPoint.ToString(), text);
				}
				if (!negotiateStream.IsMutuallyAuthenticated)
				{
					ExTraceGlobals.TcpServerTracer.TraceError((long)connection.GetHashCode(), "Mutual Authentication failed");
				}
				WindowsIdentity wid = negotiateStream.RemoteIdentity as WindowsIdentity;
				string text2 = null;
				try
				{
					text2 = negotiateStream.RemoteIdentity.Name;
				}
				catch (SystemException ex)
				{
					string text3 = string.Format("RemoteIdentity.Name failed: {0}", ex.ToString());
					ExTraceGlobals.TcpServerTracer.TraceError((long)connection.GetHashCode(), text3);
					ReplayCrimsonEvents.ServerSideConnectionFailure.LogPeriodic<string, string, string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, connection.RemoteEndPoint.ToString(), connection.LocalEndPoint.ToString(), text3);
				}
				if (!RemoteDataProvider.AuthorizeRequest(wid))
				{
					ExTraceGlobals.TcpServerTracer.TraceError<string, string>((long)connection.GetHashCode(), "Authorization failed. ClientMachine={0}, User={1}", connection.RemoteEndPoint.ToString(), text2);
					ReplayCrimsonEvents.ServerSideConnectionFailure.LogPeriodic<string, string, string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, connection.RemoteEndPoint.ToString(), connection.LocalEndPoint.ToString(), string.Format("AuthorizeRequest failed. User={0}", text2));
					return null;
				}
				tcpServerChannel = new TcpServerChannel(connection, negotiateStream, listener.ListenerConfig.IOTimeoutInMSec, listener.ListenerConfig.IdleLimit);
				ExTraceGlobals.TcpServerTracer.TraceDebug<string, bool, bool>((long)tcpServerChannel.GetHashCode(), "Connection authenticated as {0}. Encrypted={1} Signed={2}", text2, negotiateStream.IsEncrypted, negotiateStream.IsSigned);
				if (tcpServerChannel != null)
				{
					disposeGuard.Success();
				}
			}
			return tcpServerChannel;
		}

		private string m_networkName;

		private string m_clientNodeName;

		private ExchangeNetworkPerfmonCounters m_networkPerfCounters;
	}
}
