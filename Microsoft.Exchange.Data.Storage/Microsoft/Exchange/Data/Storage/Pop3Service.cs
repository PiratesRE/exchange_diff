using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Pop3Service : EmailTransportService
	{
		private Pop3Service(TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniEmailTransport emailTransport) : base(serverInfo, ServiceType.Pop3, clientAccessType, authenticationMethod, emailTransport)
		{
		}

		internal static bool TryCreatePop3Service(MiniEmailTransport emailTransport, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (emailTransport.IsPop3)
			{
				service = new Pop3Service(serverInfo, clientAccessType, authenticationMethod, emailTransport);
				return true;
			}
			service = null;
			return false;
		}
	}
}
