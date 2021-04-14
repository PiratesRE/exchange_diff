using System;

namespace Microsoft.Exchange.PopImap.Core
{
	public class SessionInfo
	{
		internal SessionInfo()
		{
		}

		internal SessionInfo(ProtocolSession protocolSession)
		{
			lock (protocolSession)
			{
				if (protocolSession.LightLogSession != null)
				{
					this.ClientIp = protocolSession.LightLogSession.ClientIp;
					this.ProxyDestination = protocolSession.LightLogSession.ProxyDestination;
				}
				if (protocolSession.Connection != null)
				{
					this.LocalEndPoint = protocolSession.LocalEndPoint.ToString();
					this.RemoteEndPoint = protocolSession.RemoteEndPoint.ToString();
				}
				this.SessionId = protocolSession.SessionId;
				this.Disconnected = protocolSession.Disconnected;
				this.Disposed = protocolSession.Disposed;
				this.NegotiatingTls = protocolSession.NegotiatingTls;
				this.IsTls = protocolSession.IsTls;
				this.Started = protocolSession.Started.UniversalTime;
				this.LastActivityTime = protocolSession.LastActivityTime.UniversalTime;
				if (protocolSession.ResponseFactory != null)
				{
					this.User = protocolSession.ResponseFactory.UserName;
					this.Mailbox = protocolSession.ResponseFactory.Mailbox;
					if (protocolSession.ResponseFactory.ProtocolUser != null)
					{
						this.ConnectionId = protocolSession.ResponseFactory.ProtocolUser.ConnectionIdentity;
					}
				}
			}
		}

		public string ClientIp { get; set; }

		public string LocalEndPoint { get; set; }

		public string RemoteEndPoint { get; set; }

		public long SessionId { get; set; }

		public bool Disconnected { get; set; }

		public bool Disposed { get; set; }

		public bool NegotiatingTls { get; set; }

		public bool IsTls { get; set; }

		public DateTime Started { get; set; }

		public DateTime LastActivityTime { get; set; }

		public string User { get; set; }

		public string ConnectionId { get; set; }

		public string Mailbox { get; set; }

		public string ProxyDestination { get; set; }
	}
}
