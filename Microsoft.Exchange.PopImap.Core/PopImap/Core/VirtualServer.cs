using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class VirtualServer : ExchangeDiagnosableWrapper<SessionsInfo>, IDisposable
	{
		public VirtualServer(ProtocolBaseServices server, PopImapAdConfiguration configuration)
		{
			ProtocolBaseServices.Assert(server != null, "server is null", new object[0]);
			ProtocolBaseServices.Assert(configuration != null, "configuration is null", new object[0]);
			this.server = server;
			if (ResponseFactory.AppendServerNameInBannerEnabled)
			{
				string s = string.Format("{0}.{1}", configuration.Server.Name, configuration.Server.DomainId);
				string str = Convert.ToBase64String(Encoding.Unicode.GetBytes(s));
				this.banner = configuration.Banner + " [" + str + "]";
			}
			else
			{
				this.banner = configuration.Banner;
			}
			this.sslCertificateName = new string[]
			{
				configuration.X509CertificateName
			};
			if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.cafe)
			{
				this.sslPorts = new List<IPEndPoint>(configuration.SSLBindings.Count);
				foreach (IPBinding item in configuration.SSLBindings)
				{
					this.sslPorts.Add(item);
				}
				this.nonSslPorts = new List<IPEndPoint>(configuration.UnencryptedOrTLSBindings.Count);
				using (MultiValuedProperty<IPBinding>.Enumerator enumerator2 = configuration.UnencryptedOrTLSBindings.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IPBinding item2 = enumerator2.Current;
						this.nonSslPorts.Add(item2);
					}
					goto IL_210;
				}
			}
			ProtocolBaseServices.ServerTracer.TraceDebug<int>(0L, "Initializing BE virtual server . Proxytargetport:{0}.", configuration.ProxyTargetPort);
			this.sslPorts = new List<IPEndPoint>(4);
			this.sslPorts.Add(new IPEndPoint(IPAddress.Any, configuration.ProxyTargetPort));
			this.sslPorts.Add(new IPEndPoint(IPAddress.IPv6Any, configuration.ProxyTargetPort));
			this.nonSslPorts = new List<IPEndPoint>(4);
			this.nonSslPorts.Add(new IPEndPoint(IPAddress.Any, configuration.ProxyTargetPort));
			this.nonSslPorts.Add(new IPEndPoint(IPAddress.IPv6Any, configuration.ProxyTargetPort));
			IL_210:
			this.sessions = new List<ProtocolSession>();
		}

		public ProtocolBaseServices Server
		{
			get
			{
				return this.server;
			}
		}

		public int ConnectionCount
		{
			get
			{
				return this.sessions.Count;
			}
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this.certificateCache.Find(this.sslCertificateName, CertificateSelectionOption.WildcardAllowed | CertificateSelectionOption.PreferedNonSelfSigned);
			}
		}

		public string Banner
		{
			get
			{
				return this.banner;
			}
		}

		public abstract ExPerformanceCounter Connections_Current { get; }

		public abstract ExPerformanceCounter Connections_Failed { get; }

		public abstract ExPerformanceCounter Connections_Rejected { get; }

		public abstract ExPerformanceCounter Connections_Total { get; }

		public abstract ExPerformanceCounter UnAuth_Connections { get; }

		public abstract ExPerformanceCounter SSLConnections_Current { get; }

		public abstract ExPerformanceCounter SSLConnections_Total { get; }

		public abstract ExPerformanceCounter InvalidCommands { get; }

		public abstract ExPerformanceCounter AverageCommandProcessingTime { get; }

		public abstract ExPerformanceCounter Proxy_Connections_Current { get; }

		public abstract ExPerformanceCounter Proxy_Connections_Failed { get; }

		public abstract ExPerformanceCounter Proxy_Connections_Total { get; }

		public abstract ExPerformanceCounter Requests_Total { get; }

		public abstract ExPerformanceCounter Requests_Failure { get; }

		public abstract int RpcLatencyCounterIndex { get; }

		public abstract int LdapLatencyCounterIndex { get; }

		public static VirtualServer GetInstance()
		{
			return ProtocolBaseServices.VirtualServer;
		}

		public bool Initialize()
		{
			if (!string.IsNullOrEmpty(this.sslCertificateName[0]))
			{
				this.certificateCache.Open(OpenFlags.ReadOnly);
				if (this.Certificate == null)
				{
					ProtocolBaseServices.ServerTracer.TraceError<string>(0L, "Unable to find certificate \"{0}\".", this.sslCertificateName[0]);
					ProtocolBaseServices.LogEvent(this.server.SslCertificateNotFoundEventTuple, this.sslCertificateName[0], new string[]
					{
						this.sslCertificateName[0]
					});
				}
			}
			else
			{
				ProtocolBaseServices.ServerTracer.TraceError(0L, "Configuration information for the SSL certificate is null or empty.");
				ProtocolBaseServices.LogEvent(this.server.SslCertificateNotFoundEventTuple, string.Empty, new string[]
				{
					string.Empty
				});
			}
			return true;
		}

		public void AcceptClientConnection(Socket socket)
		{
			ProtocolSession protocolSession = null;
			if (this.stopping)
			{
				VirtualServer.ShutdownSocket(socket);
				return;
			}
			IPAddress address = ((IPEndPoint)socket.RemoteEndPoint).Address;
			if (!this.server.IsOnline() && !address.Equals(IPAddress.Loopback) && !address.Equals(IPAddress.IPv6Loopback))
			{
				ProtocolBaseServices.ServerTracer.TraceError(0L, "Rejecting connection because Protocol is set to Offline.");
				VirtualServer.ShutdownSocket(socket);
				return;
			}
			try
			{
				lock (this.sessions)
				{
					if (this.stopping)
					{
						ProtocolBaseServices.ServerTracer.TraceDebug(0L, "AcceptClientConnection is called when service is stopping, connection rejected.");
						VirtualServer.ShutdownSocket(socket);
						return;
					}
					this.Connections_Total.Increment();
					this.Connections_Current.Increment();
					NetworkConnection connection = new NetworkConnection(socket, 4096);
					protocolSession = this.CreateNewSession(connection);
					ProtocolBaseServices.ServerTracer.TraceDebug<long, IPEndPoint, IPEndPoint>(0L, "New Tcp connection {0} detected from {1} to {2}", protocolSession.SessionId, protocolSession.RemoteEndPoint, protocolSession.LocalEndPoint);
					if (this.sessions.Count >= this.server.MaxConnectionCount)
					{
						ProtocolBaseServices.ServerTracer.TraceWarning<int>(0L, "Maximum connections exceeded {0}, session disconnected.", this.server.MaxConnectionCount);
						ProtocolBaseServices.LogEvent(this.server.MaxConnectionCountExceededEventTuple, this.ToString(), new string[0]);
						protocolSession.BeginShutdown(this.server.MaxConnectionsError);
						this.Connections_Rejected.Increment();
						return;
					}
					this.sessions.Add(protocolSession);
				}
				bool flag2 = false;
				if (!this.sslPorts.Contains(protocolSession.LocalEndPoint))
				{
					if (!this.nonSslPorts.Contains(protocolSession.LocalEndPoint))
					{
						IPEndPoint item = new IPEndPoint(IPAddress.Any, protocolSession.LocalEndPoint.Port);
						if (this.sslPorts.Contains(item))
						{
							this.sslPorts.Add(protocolSession.LocalEndPoint);
							flag2 = true;
						}
						else if (this.nonSslPorts.Contains(item))
						{
							this.nonSslPorts.Add(protocolSession.LocalEndPoint);
						}
					}
				}
				else
				{
					flag2 = true;
				}
				if (flag2 && this.Certificate == null)
				{
					ProtocolBaseServices.ServerTracer.TraceWarning(0L, "Unable to start SSL connection without a certificate.");
					ProtocolBaseServices.LogEvent(this.server.SslConnectionNotStartedEventTuple, this.ToString(), new string[0]);
					protocolSession.BeginShutdown(this.server.NoSslCertificateError);
					this.Connections_Rejected.Increment();
				}
				else
				{
					protocolSession.StartSession(flag2);
				}
			}
			finally
			{
				ProtocolBaseServices.InMemoryTraceOperationCompleted(0L);
			}
		}

		public void RemoveSession(ProtocolSession protocolSession)
		{
			if (this.stopping)
			{
				return;
			}
			lock (this.sessions)
			{
				if (!this.stopping)
				{
					this.sessions.Remove(protocolSession);
					this.Connections_Current.Decrement();
					if (protocolSession.IsTls)
					{
						this.SSLConnections_Current.Decrement();
					}
				}
			}
		}

		public void SessionStopped()
		{
			if (Interlocked.Decrement(ref this.sessionsToStop) == 0)
			{
				this.allStopped.Set();
			}
		}

		public int DisposeExpiredSessions(string connectionId)
		{
			ProtocolBaseServices.ServerTracer.TraceDebug<string>(0L, "DisposeExpiredSessions for {0}", connectionId);
			int num = 0;
			if (this.stopping)
			{
				return 0;
			}
			List<ProtocolSession> list = new List<ProtocolSession>();
			lock (this.sessions)
			{
				if (this.stopping)
				{
					return 0;
				}
				ExDateTime t = ExDateTime.UtcNow.AddSeconds((double)(-(double)this.Server.ConnectionTimeout));
				foreach (ProtocolSession protocolSession in this.sessions)
				{
					if (!protocolSession.Disposed)
					{
						if (Monitor.TryEnter(protocolSession))
						{
							try
							{
								if (protocolSession.Disposed)
								{
									continue;
								}
								if (protocolSession.ResponseFactory != null && protocolSession.ResponseFactory.ProtocolUser != null && string.Equals(protocolSession.ResponseFactory.ProtocolUser.ConnectionIdentity, connectionId))
								{
									if (protocolSession.Disconnected || protocolSession.Connection == null || protocolSession.LastActivityTime < t)
									{
										ProtocolBaseServices.SessionTracer.TraceDebug<ProtocolSession>(protocolSession.SessionId, "Expired session found: {0}", protocolSession);
										protocolSession.ResponseFactory.ProtocolUser.ConnectionIdentity = null;
										list.Add(protocolSession);
									}
									else
									{
										num++;
									}
								}
								continue;
							}
							finally
							{
								Monitor.Exit(protocolSession);
							}
						}
						ProtocolBaseServices.SessionTracer.TraceDebug(protocolSession.SessionId, "session already locked by another thread.");
					}
				}
			}
			foreach (ProtocolSession protocolSession2 in list)
			{
				protocolSession2.Dispose();
			}
			return num;
		}

		public void Stop()
		{
			lock (this.sessions)
			{
				this.stopping = true;
			}
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Stop all sessions.");
			BaseSession.ConnectionShutdownDelegate connectionShutdown = new BaseSession.ConnectionShutdownDelegate(this.SessionStopped);
			this.allStopped = new AutoResetEvent(this.sessions.Count == 0);
			this.sessionsToStop = this.sessions.Count;
			foreach (ProtocolSession protocolSession in this.sessions)
			{
				if (!protocolSession.Disposed)
				{
					lock (protocolSession)
					{
						if (!protocolSession.Disposed)
						{
							ProtocolBaseServices.SessionTracer.TraceDebug(protocolSession.SessionId, "Try to stop the session.");
							protocolSession.BeginShutdown(this.server.ServerShutdownMessage, connectionShutdown);
						}
					}
				}
			}
			this.allStopped.WaitOne(30000, true);
			this.certificateCache.Close();
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Close performance counters");
			this.ClosePerfCounters();
		}

		public abstract ProtocolSession CreateNewSession(NetworkConnection connection);

		public abstract void ClosePerfCounters();

		void IDisposable.Dispose()
		{
			this.ClosePerfCounters();
			if (this.allStopped != null)
			{
				this.allStopped.Close();
				this.allStopped = null;
			}
			GC.SuppressFinalize(this);
		}

		protected override string UsageText
		{
			get
			{
				return "Returns active session and user info.";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Get-ExchangeDiagnosticInfo -Server <TargetServer> -Process <ProcessName> -Component SessionsInfo [-Argument Detailed]";
			}
		}

		protected override string ComponentName
		{
			get
			{
				return "SessionsInfo";
			}
		}

		internal override SessionsInfo GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			SessionsInfo sessionsInfo = new SessionsInfo();
			sessionsInfo.Count = this.sessions.Count;
			sessionsInfo.Stopping = this.stopping;
			if (string.Equals("Detailed", argument.Argument, StringComparison.InvariantCultureIgnoreCase))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(ResponseFactory.ConnectionsPerUser.Counters.Count);
				lock (this.sessions)
				{
					sessionsInfo.Count = this.sessions.Count;
					sessionsInfo.Stopping = this.stopping;
					sessionsInfo.Sessions = new SessionInfo[sessionsInfo.Count];
					int num = 0;
					foreach (ProtocolSession protocolSession in this.sessions)
					{
						try
						{
							SessionInfo sessionInfo = new SessionInfo(protocolSession);
							sessionsInfo.Sessions[num++] = sessionInfo;
							if (!string.IsNullOrEmpty(sessionInfo.ConnectionId))
							{
								dictionary[sessionInfo.ConnectionId] = sessionInfo.User;
							}
						}
						catch (Exception arg)
						{
							ProtocolBaseServices.ServerTracer.TraceDebug<ProtocolSession, Exception>(0L, "Exception while creating SessionInfo for session {0}\r\n{1}", protocolSession, arg);
						}
					}
				}
				lock (ResponseFactory.ConnectionsPerUser)
				{
					sessionsInfo.Users = new UserInfo[ResponseFactory.ConnectionsPerUser.Counters.Count];
					int num2 = 0;
					foreach (string text in ResponseFactory.ConnectionsPerUser.Counters.Keys)
					{
						UserInfo userInfo = new UserInfo();
						string name;
						if (dictionary.TryGetValue(text, out name))
						{
							userInfo.Name = name;
						}
						else
						{
							userInfo.Name = text;
						}
						userInfo.SessionCount = ResponseFactory.ConnectionsPerUser.Counters[text];
						sessionsInfo.Users[num2++] = userInfo;
					}
				}
			}
			return sessionsInfo;
		}

		private static void ShutdownSocket(Socket socket)
		{
			try
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			catch (SocketException arg)
			{
				ProtocolBaseServices.ServerTracer.TraceDebug<SocketException>(0L, "Exception while shutting down socket: {0}", arg);
			}
		}

		private ProtocolBaseServices server;

		private string banner;

		private string[] sslCertificateName;

		private TlsCertificateCache certificateCache = new TlsCertificateCache();

		private List<IPEndPoint> sslPorts;

		private List<IPEndPoint> nonSslPorts;

		private bool stopping;

		private List<ProtocolSession> sessions;

		private AutoResetEvent allStopped;

		private int sessionsToStop;
	}
}
