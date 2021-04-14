using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UpgradeCommon
	{
		public static string DefaultSymphonyCertificateSubject
		{
			get
			{
				switch (CommonUtils.ForestType)
				{
				case ForestType.TestTopology:
					return "CN=exchangeonline-redirection-test-symphony";
				case ForestType.ServiceDogfood:
					return "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
				case ForestType.ServiceProduction:
					return "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
				default:
					return "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
				}
			}
		}

		public static Uri DefaultSymphonyWebserviceUri
		{
			get
			{
				string uriString;
				switch (CommonUtils.ForestType)
				{
				case ForestType.TestTopology:
					uriString = "https://365upgrade.devfabric.bosxlab.com:443";
					break;
				case ForestType.ServiceDogfood:
					uriString = "https://365upgrade.ccsctp.com";
					break;
				case ForestType.ServiceProduction:
					uriString = "https://365upgrade.microsoftonline.com";
					break;
				default:
					uriString = "https://365upgrade.microsoftonline.com";
					break;
				}
				return new Uri(uriString);
			}
		}

		public const string SDFWebServiceUri = "https://365upgrade.ccsctp.com";

		public const string ProdWebServiceUri = "https://365upgrade.microsoftonline.com";

		public const string ProdCertificateSubject = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";

		public const string SDFCertificateSubject = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";

		public const string TopologyWebServiceUri = "https://365upgrade.devfabric.bosxlab.com:443";

		public const string TopologyCertificateSubject = "CN=exchangeonline-redirection-test-symphony";
	}
}
