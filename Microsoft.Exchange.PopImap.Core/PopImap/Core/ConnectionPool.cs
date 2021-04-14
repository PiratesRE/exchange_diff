using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class ConnectionPool
	{
		public ConnectionPool(IPEndPoint endpoint, ConnectionPool.ConnectionAcceptedDelegate connectionAccepted)
		{
			if (endpoint == null)
			{
				throw new ArgumentNullException("endpoint");
			}
			if (connectionAccepted == null)
			{
				throw new ArgumentNullException("connectionAccepted");
			}
			this.connectionAcceptedDelegate = connectionAccepted;
			this.acceptSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			this.acceptSocket.Bind(endpoint);
			this.acceptSocket.Listen(int.MaxValue);
			this.acceptSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), null);
		}

		public void Shutdown()
		{
			this.acceptSocket.Close();
		}

		private void AcceptCallback(IAsyncResult iar)
		{
			Socket socket = null;
			try
			{
				socket = this.acceptSocket.EndAccept(iar);
			}
			catch (SocketException arg)
			{
				ProtocolBaseServices.ServerTracer.TraceError<SocketException>(0L, "AcceptCallback(): SocketException: {0} ", arg);
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			if (socket != null)
			{
				this.connectionAcceptedDelegate(socket);
			}
			this.acceptSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), null);
		}

		private Socket acceptSocket;

		private ConnectionPool.ConnectionAcceptedDelegate connectionAcceptedDelegate;

		public delegate void ConnectionAcceptedDelegate(Socket clientsocket);
	}
}
