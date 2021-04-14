using System;
using System.Net;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal struct ProxyParseCommonOutput
	{
		public string SessionId;

		public IPAddress ClientIp;

		public int ClientPort;

		public string ClientHelloDomain;
	}
}
