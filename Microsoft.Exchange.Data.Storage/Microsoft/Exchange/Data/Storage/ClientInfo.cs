using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientInfo
	{
		private ClientInfo(string clientInfoStringWithoutAction)
		{
			this.clientInfoStringWithoutAction = clientInfoStringWithoutAction;
			this.clientInfoStringWithAnyAction = this.clientInfoStringWithoutAction + ";";
		}

		public bool IsMatch(string clientInfoString)
		{
			return clientInfoString.Equals(this.clientInfoStringWithoutAction, StringComparison.OrdinalIgnoreCase) || clientInfoString.StartsWith(this.clientInfoStringWithAnyAction, StringComparison.OrdinalIgnoreCase);
		}

		public static readonly ClientInfo OWA = new ClientInfo("Client=OWA");

		public static readonly ClientInfo MOMT = new ClientInfo("Client=MSExchangeRPC");

		public static readonly ClientInfo HubTransport = new ClientInfo("Client=Hub Transport");

		private readonly string clientInfoStringWithoutAction;

		private readonly string clientInfoStringWithAnyAction;
	}
}
