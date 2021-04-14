using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ServerParameters
	{
		public ServerParameters(string server, int port)
		{
			this.Server = server;
			this.Port = port;
		}

		public string Server { get; private set; }

		public int Port { get; private set; }
	}
}
