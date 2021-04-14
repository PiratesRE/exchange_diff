using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class CTelnetProxy : TransportConnection, IDataConnection
	{
		public CTelnetProxy(bool fCisco, ProxyChain proxyChain) : base(proxyChain)
		{
			this.fCisco = fCisco;
		}

		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			this.tcpCxn = tcpCxn;
			this.connectRequest = new StringBuilder();
			if (this.fCisco)
			{
				this.connectRequest.AppendFormat("cisco\r\n", new object[0]);
			}
			this.connectRequest.AppendFormat("telnet {0} {1}\r\n", remoteEndpoint.Address.ToString(), remoteEndpoint.Port);
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

		private bool fCisco;

		private TcpConnection tcpCxn;

		private enum TelnetProxyState
		{
			Invalid,
			ConnectSent,
			Finished
		}
	}
}
