using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OwaService : HttpService
	{
		private OwaService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.OutlookWebAccess, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.AnonymousFeaturesEnabled = (virtualDirectory.AnonymousFeaturesEnabled == true);
			this.FailbackUrl = virtualDirectory.FailbackUrl;
			this.IntegratedFeaturesEnabled = (virtualDirectory.IntegratedFeaturesEnabled != null && virtualDirectory.IntegratedFeaturesEnabled != null && virtualDirectory.IntegratedFeaturesEnabled.Value);
		}

		public bool IntegratedFeaturesEnabled { get; private set; }

		public bool AnonymousFeaturesEnabled { get; private set; }

		public Uri FailbackUrl { get; private set; }

		internal static bool TryCreateOwaService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsOwa)
			{
				service = new OwaService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
