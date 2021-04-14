using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PingSource
	{
		public Socket Socket { get; private set; }

		public IPAddress OutgoingAddress { get; private set; }

		public void Close()
		{
			this.Socket.Close();
		}

		public PingSource(IPAddress src, int pingTimeout)
		{
			this.OutgoingAddress = src;
			IPEndPoint ipendPoint = new IPEndPoint(src, 0);
			this.Socket = new Socket(ipendPoint.AddressFamily, SocketType.Raw, ProtocolType.Icmp);
			this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, pingTimeout);
			this.Socket.Bind(ipendPoint);
		}
	}
}
