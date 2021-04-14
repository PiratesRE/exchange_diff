using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class CWinGateProxy : TransportConnection, IDataConnection
	{
		public CWinGateProxy(ProxyChain proxyChain) : base(proxyChain)
		{
		}

		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			this.tcpCxn = tcpCxn;
			this.connectRequest = new StringBuilder();
			this.connectRequest.AppendFormat("{0}:{1}\r\n", remoteEndpoint.Address.ToString(), remoteEndpoint.Port);
			try
			{
				this.tcpCxn.SendString(this.connectRequest.ToString());
			}
			catch (AtsException)
			{
				base.ProxyChain.OnDisconnected();
			}
		}

		public int OnDataReceived(byte[] dataReceived, int offset, int length)
		{
			int num = 0;
			return num + base.ProxyChain.OnConnected(dataReceived, offset + num, length - num);
		}

		private StringBuilder connectRequest;

		private TcpConnection tcpCxn;

		private enum WinGateProxyState
		{
			Invalid,
			ConnectSent,
			Finished
		}
	}
}
