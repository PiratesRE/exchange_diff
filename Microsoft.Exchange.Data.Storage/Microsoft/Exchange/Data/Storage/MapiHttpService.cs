using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiHttpService : HttpService
	{
		private MapiHttpService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.MapiHttp, url, clientAccessType, AuthenticationMethod.None, virtualDirectory)
		{
			this.LastConfigurationTime = (virtualDirectory.WhenChangedUTC ?? (virtualDirectory.WhenCreatedUTC ?? DateTime.MinValue));
		}

		public DateTime LastConfigurationTime { get; private set; }

		public static bool TryCreateMapiHttpService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			EnumValidator.ThrowIfInvalid<ClientAccessType>(clientAccessType, "clientAccessType");
			if (virtualDirectory.IsMapi)
			{
				service = new MapiHttpService(serverInfo, url, clientAccessType, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
