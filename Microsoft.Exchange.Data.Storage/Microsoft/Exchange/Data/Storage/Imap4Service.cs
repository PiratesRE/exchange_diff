using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Imap4Service : EmailTransportService
	{
		private Imap4Service(TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniEmailTransport emailTransport) : base(serverInfo, ServiceType.Imap4, clientAccessType, authenticationMethod, emailTransport)
		{
		}

		internal static bool TryCreateImap4Service(MiniEmailTransport emailTransport, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (emailTransport.IsImap4)
			{
				service = new Imap4Service(serverInfo, clientAccessType, authenticationMethod, emailTransport);
				return true;
			}
			service = null;
			return false;
		}
	}
}
