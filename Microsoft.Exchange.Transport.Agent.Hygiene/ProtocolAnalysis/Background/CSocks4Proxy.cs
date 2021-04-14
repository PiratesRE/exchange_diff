using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class CSocks4Proxy : TransportConnection, IDataConnection
	{
		public CSocks4Proxy(ProxyChain proxyChain) : base(proxyChain)
		{
		}

		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			this.tcpCxn = tcpCxn;
			if (remoteEndpoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				base.ProxyChain.OnDisconnected();
				return;
			}
			this.connectRequest = new byte[8];
			this.connectRequest[0] = 4;
			this.connectRequest[1] = 1;
			this.connectRequest[2] = (byte)(remoteEndpoint.Port >> 8 & 255);
			this.connectRequest[3] = (byte)(remoteEndpoint.Port & 255);
			byte[] addressBytes = remoteEndpoint.Address.GetAddressBytes();
			Array.Copy(addressBytes, 0, this.connectRequest, 4, 4);
			for (int i = 0; i < authInfo.UserName.Length; i++)
			{
				this.connectRequest[i + 3] = (byte)authInfo.UserName[i];
			}
			this.connectRequest[7] = 0;
			try
			{
				this.tcpCxn.SendMessage(this.connectRequest, 0, 8);
				this.state = CSocks4Proxy.Socks4ProxyState.ConnectSent;
			}
			catch (AtsException)
			{
				base.ProxyChain.OnDisconnected();
			}
		}

		public int OnDataReceived(byte[] dataReceived, int offset, int length)
		{
			int num = 0;
			bool flag = false;
			while (length - num > 0)
			{
				CSocks4Proxy.Socks4ProxyState socks4ProxyState = this.state;
				if (socks4ProxyState != CSocks4Proxy.Socks4ProxyState.ConnectSent)
				{
					return num;
				}
				if (length - num < 8)
				{
					return num;
				}
				byte b = dataReceived[offset + num + 1];
				if (b == 90)
				{
					flag = true;
				}
				num += 8;
				this.state = CSocks4Proxy.Socks4ProxyState.Finished;
				if (flag)
				{
					num += base.ProxyChain.OnConnected(dataReceived, offset + num, length - num);
				}
				else
				{
					base.ProxyChain.OnDisconnected();
				}
			}
			return num;
		}

		private const int ConnectRequestSize = 8;

		private const int ConnectResponseSize = 8;

		private const byte Socks4RequestGrantCode = 90;

		private TcpConnection tcpCxn;

		private CSocks4Proxy.Socks4ProxyState state;

		private byte[] connectRequest;

		private enum Socks4ProxyState
		{
			Invalid,
			ConnectSent,
			Finished
		}
	}
}
