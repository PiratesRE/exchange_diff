using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	public class HttpPingServer
	{
		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public static HttpPingServer Instance
		{
			get
			{
				return HttpPingServer.theInstance;
			}
		}

		private HttpPingServer()
		{
		}

		public void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			lock (this)
			{
				if (!this.initialized)
				{
					while (this.connectRetryCount < 5)
					{
						Exception ex = this.Reconnect();
						if (ex == null)
						{
							this.initialized = true;
							return;
						}
						this.TrackError(ex);
						this.connectRetryCount++;
					}
					this.initialized = false;
				}
			}
		}

		private void TrackError(Exception e)
		{
			if (this.errors.Count > 10)
			{
				string text;
				this.errors.TryPop(out text);
			}
			this.errors.Push(e.ToString());
		}

		private Exception Reconnect()
		{
			if (this.mainSocket != null)
			{
				this.mainSocket.Dispose();
			}
			this.port = -1;
			try
			{
				this.localIps = Dns.GetHostEntry("localhost");
				IPAddress ipaddress = this.localIps.AddressList[0];
				this.mainSocket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.mainSocket.Bind(new IPEndPoint(ipaddress, 0));
				this.mainSocket.Listen(200);
				this.port = ((IPEndPoint)this.mainSocket.LocalEndPoint).Port;
				this.mainSocket.BeginAccept(new AsyncCallback(this.NewConnectionReceived), null);
			}
			catch (Exception result)
			{
				return result;
			}
			return null;
		}

		private void NewConnectionReceived(IAsyncResult asyncResult)
		{
			Socket socket;
			try
			{
				socket = this.mainSocket.EndAccept(asyncResult);
				this.connectRetryCount = 0;
			}
			catch (Exception e)
			{
				this.HandleSocketDisconnection(e);
				return;
			}
			try
			{
				this.mainSocket.BeginAccept(new AsyncCallback(this.NewConnectionReceived), null);
				if (!this.IsLocalConnection(socket))
				{
					socket.Close();
					socket.Dispose();
				}
				else
				{
					using (NetworkStream networkStream = new NetworkStream(socket, true))
					{
						using (StreamReader streamReader = new StreamReader(networkStream))
						{
							string text;
							do
							{
								text = streamReader.ReadLine();
							}
							while (text != null && text.Trim() != string.Empty);
							using (StreamWriter streamWriter = new StreamWriter(networkStream))
							{
								streamWriter.Write("HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Length: 0\r\nX-PingServer: 1\r\nConnection: Close\r\n\r\n");
								streamWriter.Flush();
							}
						}
					}
				}
			}
			catch (Exception e2)
			{
				this.HandleIndividualSocketException(e2);
			}
		}

		private void HandleIndividualSocketException(Exception e)
		{
			this.TrackError(e);
		}

		private void HandleSocketDisconnection(Exception e)
		{
			this.TrackError(e);
			this.connectRetryCount++;
			this.initialized = false;
			this.Initialize();
		}

		private bool IsLocalConnection(Socket newSocket)
		{
			IPEndPoint ipendPoint = (IPEndPoint)newSocket.RemoteEndPoint;
			foreach (IPAddress ipaddress in this.localIps.AddressList)
			{
				if (ipaddress.Equals(ipendPoint.Address))
				{
					return true;
				}
			}
			return false;
		}

		private const string OkResponse = "HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Length: 0\r\nX-PingServer: 1\r\nConnection: Close\r\n\r\n";

		private const int MaxReconnectAttempts = 5;

		private const int MaxErrorsToTrack = 10;

		private static HttpPingServer theInstance = new HttpPingServer();

		private Socket mainSocket;

		private int port;

		private bool initialized;

		private IPHostEntry localIps;

		private int connectRetryCount;

		private ConcurrentStack<string> errors = new ConcurrentStack<string>();
	}
}
