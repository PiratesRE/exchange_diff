using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcHttpService : HttpService
	{
		private RpcHttpService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.RpcHttp, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.IISAuthenticationMethods = Service.ConvertToReadOnlyCollection<AuthenticationMethod>(virtualDirectory.IISAuthenticationMethods);
			this.ExternalClientAuthenticationMethod = virtualDirectory.ExternalClientAuthenticationMethod;
			this.InternalClientAuthenticationMethod = virtualDirectory.InternalClientAuthenticationMethod;
			this.XropUrl = virtualDirectory.XropUrl;
		}

		public Uri XropUrl { get; private set; }

		public ReadOnlyCollection<AuthenticationMethod> IISAuthenticationMethods { get; private set; }

		public AuthenticationMethod ExternalClientAuthenticationMethod { get; private set; }

		public AuthenticationMethod InternalClientAuthenticationMethod { get; private set; }

		public static bool TryCreateRpcHttpService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsRpcHttp)
			{
				service = new RpcHttpService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
