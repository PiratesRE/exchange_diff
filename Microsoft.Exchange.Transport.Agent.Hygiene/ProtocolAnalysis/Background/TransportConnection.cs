using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal abstract class TransportConnection
	{
		protected TransportConnection(ProxyChain proxyChain)
		{
			this.proxyChain = proxyChain;
		}

		public abstract void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpConnection, NetworkCredential authInfo);

		public IDataConnection DataCxn
		{
			get
			{
				return this.dataCxn;
			}
			set
			{
				this.dataCxn = value;
			}
		}

		public ProxyChain ProxyChain
		{
			get
			{
				return this.proxyChain;
			}
		}

		private IDataConnection dataCxn;

		private ProxyChain proxyChain;
	}
}
