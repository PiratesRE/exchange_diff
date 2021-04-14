using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WebServicesService : HttpService
	{
		private WebServicesService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.WebServices, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.ServerDistinguishedName = virtualDirectory.Server.DistinguishedName;
			this.MRSProxyEnabled = virtualDirectory.MRSProxyEnabled;
		}

		public string ServerDistinguishedName { get; private set; }

		public bool MRSProxyEnabled { get; private set; }

		public static bool TryCreateWebServicesService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			EnumValidator.ThrowIfInvalid<ClientAccessType>(clientAccessType, "clientAccessType");
			EnumValidator.ThrowIfInvalid<AuthenticationMethod>(authenticationMethod, "authenticationMethod");
			if (virtualDirectory.IsWebServices)
			{
				service = new WebServicesService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
