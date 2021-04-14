using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OabService : HttpService
	{
		private OabService(TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, MiniVirtualDirectory virtualDirectory, HashSet<string> linkedOfflineAddressBookDistinguishedNames) : base(serverInfo, ServiceType.OfflineAddressBook, url, clientAccessType, authenticationMethod, virtualDirectory)
		{
			this.LinkedOfflineAddressBookDistinguishedNames = linkedOfflineAddressBookDistinguishedNames;
		}

		public HashSet<string> LinkedOfflineAddressBookDistinguishedNames { get; private set; }

		internal static bool TryCreateOabService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service)
		{
			if (virtualDirectory.IsOab)
			{
				MultiValuedProperty<ADObjectId> offlineAddressBooks = virtualDirectory.OfflineAddressBooks;
				HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
				for (int i = 0; i < offlineAddressBooks.Count; i++)
				{
					hashSet.Add(offlineAddressBooks[i].DistinguishedName);
				}
				service = new OabService(serverInfo, url, clientAccessType, authenticationMethod, virtualDirectory, hashSet);
				return true;
			}
			service = null;
			return false;
		}
	}
}
