using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TcpListener
	{
		public TcpListener.Config ListenerConfig { get; private set; }

		public Exception StartListening(TcpListener.Config cfg)
		{
			bool flag = false;
			Exception ex = null;
			if (this.m_started)
			{
				throw new ArgumentException("TcpListener is one time only");
			}
			try
			{
				this.ListenerConfig = cfg;
				this.m_started = true;
				this.m_listenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				this.m_listenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
				this.m_listenSocket.ReceiveBufferSize = cfg.ReceiveBufferSize;
				this.m_listenSocket.SendBufferSize = cfg.SendBufferSize;
				IPEndPoint localEP = new IPEndPoint(IPAddress.IPv6Any, cfg.ListenPort);
				this.m_listenSocket.Bind(localEP);
				int maxValue = int.MaxValue;
				this.m_listenSocket.Listen(maxValue);
				this.m_listenSocket.BeginAccept(new AsyncCallback(TcpListener.AcceptCallback), this);
				this.m_idleTimer = TcpListener.IdleTimer.CreateTimer(this);
				this.m_idleTimer.Start();
				flag = true;
				TcpListener.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Listening on port {0}", cfg.ListenPort);
				ReplayCrimsonEvents.TcpListenerStarted.Log<int>(cfg.ListenPort);
			}
			catch (NetworkTransportException ex2)
			{
				ex = ex2;
			}
			catch (SocketException ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (!flag)
				{
					TcpListener.Tracer.TraceError<Exception>((long)this.GetHashCode(), "StartListening failed: {0}", ex);
					this.m_listenSocket.Close();
					this.m_listenSocket = null;
				}
			}
			return ex;
		}

		public void Stop()
		{
			this.m_stopping = true;
			if (this.m_idleTimer != null)
			{
				this.m_idleTimer.Stop();
				this.m_idleTimer = null;
			}
			if (this.m_listenSocket != null)
			{
				this.m_listenSocket.Close();
			}
		}

		private static void AcceptCallback(IAsyncResult ar)
		{
			Socket socket = null;
			TcpListener tcpListener = (TcpListener)ar.AsyncState;
			try
			{
				socket = tcpListener.m_listenSocket.EndAccept(ar);
			}
			catch (SocketException ex)
			{
				ExTraceGlobals.TcpServerTracer.TraceDebug<string>(0L, "TCP listener callback got exception: {0}", ex.Message);
			}
			catch (ObjectDisposedException ex2)
			{
				if (!tcpListener.m_stopping)
				{
					tcpListener.HandleUnexpectedException("TcpListener.ConnectCallback.EndAccept", ex2);
				}
			}
			if (!tcpListener.m_stopping)
			{
				Exception ex3 = null;
				try
				{
					tcpListener.m_listenSocket.BeginAccept(new AsyncCallback(TcpListener.AcceptCallback), tcpListener);
				}
				catch (SocketException ex4)
				{
					ex3 = ex4;
				}
				catch (ObjectDisposedException ex5)
				{
					ex3 = ex5;
				}
				if (ex3 != null)
				{
					ExTraceGlobals.TcpServerTracer.TraceError<Exception>(0L, "TCP listener. BeginAccept threw: {0}", ex3);
					if (socket != null)
					{
						socket.Close();
					}
					if (!tcpListener.m_stopping)
					{
						tcpListener.HandleUnexpectedException("TcpListener.ConnectCallback.BeginAccept", ex3);
					}
					return;
				}
			}
			if (socket != null)
			{
				tcpListener.ProcessConnection(socket);
			}
		}

		private void HandleUnexpectedException(string context, Exception ex)
		{
			string text = string.Format("{0}:{1}", context, ex.ToString());
			ExTraceGlobals.TcpServerTracer.TraceError((long)this.GetHashCode(), text);
			ReplayCrimsonEvents.OperationGeneratedUnhandledException.Log<string>(text);
			throw ex;
		}

		private void ProcessConnection(Socket connection)
		{
			if (this.ListenerConfig.SocketConnectioHandOff != null)
			{
				this.ListenerConfig.SocketConnectioHandOff(connection);
				return;
			}
			TcpServerChannel tcpServerChannel = null;
			bool flag = false;
			Exception ex = null;
			try
			{
				ExTraceGlobals.TcpServerTracer.TraceDebug<EndPoint>((long)connection.GetHashCode(), "Connection received from {0}", connection.RemoteEndPoint);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2577804605U);
				tcpServerChannel = TcpServerChannel.AuthenticateAsServer(this, connection);
				if (tcpServerChannel != null)
				{
					this.ListenerConfig.AuthConnectionHandOff(tcpServerChannel, this);
					flag = true;
				}
			}
			catch (SocketException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (AuthenticationException ex4)
			{
				ex = ex4;
			}
			catch (IdentityNotMappedException ex5)
			{
				ex = ex5;
			}
			catch (NetworkTransportException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.TcpServerTracer.TraceError<Exception>((long)this.GetHashCode(), "TCP ProcessConnection fails: {0}", ex);
				}
				if (tcpServerChannel == null)
				{
					connection.Close();
				}
				else if (!flag)
				{
					tcpServerChannel.Close();
				}
			}
		}

		public void AddActiveChannel(NetworkChannel channel)
		{
			lock (this.m_activeClientConnections)
			{
				this.m_activeClientConnections.Add(channel);
			}
		}

		public void RemoveActiveChannel(NetworkChannel channel)
		{
			lock (this.m_activeClientConnections)
			{
				this.m_activeClientConnections.Remove(channel);
			}
		}

		public NetworkChannel FindSeedingChannel(MonitoredDatabase db)
		{
			lock (this.m_activeClientConnections)
			{
				foreach (NetworkChannel networkChannel in this.m_activeClientConnections)
				{
					if (networkChannel.IsSeeding && object.ReferenceEquals(networkChannel.MonitoredDatabase, db))
					{
						return networkChannel;
					}
				}
			}
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.TcpServerTracer;

		private Socket m_listenSocket;

		private bool m_stopping;

		private List<NetworkChannel> m_activeClientConnections = new List<NetworkChannel>();

		private TcpListener.IdleTimer m_idleTimer;

		private bool m_started;

		internal delegate void AuthenticatedConnectionHandler(TcpServerChannel tcpChannel, TcpListener listener);

		internal delegate void SocketConnectionHandler(Socket socket);

		internal class Config
		{
			public int ListenPort { get; set; }

			public string LocalNodeName { get; set; }

			public int ReceiveBufferSize { get; set; }

			public int SendBufferSize { get; set; }

			public int IOTimeoutInMSec { get; set; }

			public TimeSpan IdleLimit { get; set; }

			public TcpListener.AuthenticatedConnectionHandler AuthConnectionHandOff { get; set; }

			public TcpListener.SocketConnectionHandler SocketConnectioHandOff { get; set; }

			public bool KeepServerOpenByDefault { get; set; }

			public Config()
			{
				this.LocalNodeName = Environment.MachineName;
				this.ReceiveBufferSize = RegistryParameters.LogCopyNetworkTransferSize;
				this.SendBufferSize = RegistryParameters.LogCopyNetworkTransferSize;
				this.IOTimeoutInMSec = TcpChannel.GetDefaultTimeoutInMs();
				this.IdleLimit = TimeSpan.FromSeconds((double)RegistryParameters.TcpChannelIdleLimitInSec);
			}
		}

		private class IdleTimer : TimerComponent
		{
			public static TcpListener.IdleTimer CreateTimer(TcpListener listener)
			{
				TimeSpan period = TimeSpan.FromSeconds((double)Math.Min((int)listener.ListenerConfig.IdleLimit.TotalSeconds, 120));
				return new TcpListener.IdleTimer(listener, period);
			}

			private IdleTimer(TcpListener listener, TimeSpan period) : base(period, period, "TcpServerIdleTimer")
			{
				this.m_listener = listener;
			}

			protected override void TimerCallbackInternal()
			{
				TcpListener.Tracer.TraceDebug(0L, "TcpServerIdleTimer running");
				NetworkChannel[] array;
				lock (this.m_listener.m_activeClientConnections)
				{
					array = this.m_listener.m_activeClientConnections.ToArray();
				}
				foreach (NetworkChannel networkChannel in array)
				{
					networkChannel.TcpChannel.CancelIfIdleTooLong();
				}
			}

			private TcpListener m_listener;
		}
	}
}
