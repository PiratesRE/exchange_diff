using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ServerNotFoundException : ServiceDiscoveryPermanentException
	{
		public ServerNotFoundException(string message, string serverName) : this(message, serverName, null)
		{
		}

		public ServerNotFoundException(string message) : this(message, null, null)
		{
		}

		public ServerNotFoundException(string message, string serverName, Exception innerException) : base(message, innerException)
		{
			this.ServerName = serverName;
		}

		public ServerNotFoundException(string message, Exception innerException) : this(message, null, innerException)
		{
		}

		public readonly string ServerName;
	}
}
