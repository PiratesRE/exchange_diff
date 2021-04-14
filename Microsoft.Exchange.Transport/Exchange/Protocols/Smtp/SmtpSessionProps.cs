using System;
using System.Net;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	[Serializable]
	internal class SmtpSessionProps : ISmtpSession
	{
		private SmtpSessionProps()
		{
		}

		public SmtpSessionProps(ulong id)
		{
			this.sessionId = id;
		}

		public ulong SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.remoteEndPoint;
			}
			internal set
			{
				this.remoteEndPoint = value;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.localEndPoint;
			}
			internal set
			{
				this.localEndPoint = value;
			}
		}

		public string HelloDomain
		{
			get
			{
				return this.helloDomain;
			}
			set
			{
				this.helloDomain = value;
			}
		}

		public SmtpResponse Banner
		{
			get
			{
				return this.banner;
			}
			set
			{
				this.banner = value;
			}
		}

		public IEhloOptions AdvertisedEhloOptions
		{
			get
			{
				return this.advertisedEhloOptions;
			}
			internal set
			{
				this.advertisedEhloOptions = value;
			}
		}

		public void Disconnect(DisconnectReason disconnectReason)
		{
			this.shouldDisconnect = true;
		}

		internal bool ShouldDisconnect
		{
			get
			{
				return this.shouldDisconnect;
			}
			set
			{
				this.shouldDisconnect = value;
			}
		}

		private readonly ulong sessionId;

		private IPEndPoint remoteEndPoint;

		private IPEndPoint localEndPoint;

		private string helloDomain;

		private SmtpResponse banner;

		private IEhloOptions advertisedEhloOptions;

		private bool shouldDisconnect;
	}
}
