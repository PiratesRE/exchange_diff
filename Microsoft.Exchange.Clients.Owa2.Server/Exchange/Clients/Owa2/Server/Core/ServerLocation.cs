using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ServerLocation : NotificationLocation
	{
		public ServerLocation(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentException("Server address cannot be null or empty string.", "address");
			}
			this.address = address;
		}

		public override KeyValuePair<string, object> GetEventData()
		{
			return new KeyValuePair<string, object>("ServerAddress", this.address);
		}

		public override int GetHashCode()
		{
			return ServerLocation.TypeHashCode ^ this.address.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ServerLocation serverLocation = obj as ServerLocation;
			return serverLocation != null && this.address.Equals(serverLocation.address);
		}

		public override string ToString()
		{
			return this.address;
		}

		private const string EventKey = "ServerAddress";

		private static readonly int TypeHashCode = typeof(ServerLocation).GetHashCode();

		private readonly string address;
	}
}
