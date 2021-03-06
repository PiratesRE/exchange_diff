using System;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SocketFactory
	{
		public static Socket CreateTcpStreamSocket()
		{
			return SocketFactory.CreateTcpStreamSocket(Socket.OSSupportsIPv6);
		}

		internal static Socket CreateTcpStreamSocket(bool osSupportsIPv6)
		{
			Socket socket;
			if (osSupportsIPv6)
			{
				socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
			}
			else
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
			return socket;
		}
	}
}
