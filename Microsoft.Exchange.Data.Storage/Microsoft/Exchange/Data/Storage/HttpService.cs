using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HttpService : Service
	{
		internal HttpService(TopologyServerInfo serverInfo, ServiceType serviceType, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, serviceType, clientAccessType, authenticationMethod)
		{
			this.Url = url;
			this.IsFrontEnd = HttpService.IsFrontEndRole(virtualDirectory, serverInfo);
			this.ADObjectId = virtualDirectory.Id;
		}

		public ADObjectId ADObjectId { get; private set; }

		public Uri Url { get; private set; }

		public bool IsFrontEnd { get; private set; }

		public override string ToString()
		{
			return string.Format("Service. Type = {0}. ClientAccessType = {1}. Url = {2}. AuthenticationMethod = {3}. IsFrontEnd = {4}", new object[]
			{
				base.ServiceType,
				base.ClientAccessType,
				this.Url,
				base.AuthenticationMethod,
				this.IsFrontEnd
			});
		}

		internal static bool IsFrontEndRole(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo)
		{
			return (serverInfo.Role & ServerRole.Cafe) == ServerRole.Cafe && !virtualDirectory.Name.Contains("(Exchange Back End)");
		}

		internal static bool TryCreateHttpService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			service = new HttpService(serverInfo, ServiceType.Invalid, url, clientAccessType, authenticationMethod, virtualDirectory);
			return true;
		}

		private const string BackEndNameSegment = "(Exchange Back End)";
	}
}
