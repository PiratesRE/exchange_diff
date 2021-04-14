using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class ProxyEndPoint
	{
		public ProxyEndPoint(IPEndPoint endpoint, ProxyType type, NetworkCredential authInfo)
		{
			this.endpoint = endpoint;
			this.type = type;
			this.authInfo = authInfo;
		}

		public ProxyEndPoint(IPAddress ip, int port, ProxyType type, NetworkCredential authInfo)
		{
			this.endpoint = new IPEndPoint(ip, port);
			this.type = type;
			this.authInfo = authInfo;
		}

		public IPEndPoint Endpoint
		{
			get
			{
				return this.endpoint;
			}
		}

		public ProxyType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public NetworkCredential AuthInfo
		{
			get
			{
				return this.authInfo;
			}
		}

		private IPEndPoint endpoint;

		private ProxyType type;

		private NetworkCredential authInfo;
	}
}
