using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AvailabilityForeignConnectorService : HttpService
	{
		private AvailabilityForeignConnectorService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory) : base(serverInfo, ServiceType.AvailabilityForeignConnector, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.AvailabilityForeignConnectorType = virtualDirectory.AvailabilityForeignConnectorType;
			this.AvailabilityForeignConnectorDomains = new ReadOnlyCollection<string>(virtualDirectory.AvailabilityForeignConnectorDomains.ToArray());
		}

		public string AvailabilityForeignConnectorType { get; private set; }

		public ReadOnlyCollection<string> AvailabilityForeignConnectorDomains { get; private set; }

		internal static bool TryCreateAvailabilityForeignConnectorService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsAvailabilityForeignConnector)
			{
				service = new AvailabilityForeignConnectorService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory);
				return true;
			}
			service = null;
			return false;
		}
	}
}
