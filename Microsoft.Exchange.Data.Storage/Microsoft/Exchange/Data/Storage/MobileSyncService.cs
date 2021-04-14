using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MobileSyncService : HttpService
	{
		private MobileSyncService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.MobileSync, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.VirtualDirectoryIdentity = virtualDirectory.ToString();
			this.IsCertEnrollEnabled = virtualDirectory.MobileClientCertificateProvisioningEnabled;
			this.CertificateAuthorityUrl = virtualDirectory.MobileClientCertificateAuthorityURL;
			this.CertEnrollTemplateName = virtualDirectory.MobileClientCertTemplateName;
		}

		public string VirtualDirectoryIdentity { get; private set; }

		public string CertificateAuthorityUrl { get; private set; }

		public string CertEnrollTemplateName { get; private set; }

		public bool IsCertEnrollEnabled { get; private set; }

		internal static bool TryCreateMobileSyncService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsMobile)
			{
				service = new MobileSyncService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
