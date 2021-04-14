using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EcpService : HttpService
	{
		private EcpService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.ExchangeControlPanel, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.LiveIdAuthentication = virtualDirectory.LiveIdAuthentication;
			this.MetabasePath = virtualDirectory.MetabasePath;
			this.AdminEnabled = virtualDirectory.AdminEnabled;
			this.OwaOptionsEnabled = virtualDirectory.OwaOptionsEnabled;
		}

		public bool LiveIdAuthentication { get; private set; }

		public string MetabasePath { get; private set; }

		public bool AdminEnabled { get; private set; }

		public bool OwaOptionsEnabled { get; private set; }

		internal static bool TryCreateEcpService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsEcp)
			{
				service = new EcpService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
