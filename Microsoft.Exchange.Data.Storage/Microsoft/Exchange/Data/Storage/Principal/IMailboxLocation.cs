using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxLocation
	{
		string ServerFqdn { get; }

		Guid ServerGuid { get; }

		string ServerLegacyDn { get; }

		int ServerVersion { get; }

		ADObjectId ServerSite { get; }

		string DatabaseName { get; }

		string RpcClientAccessServerLegacyDn { get; }

		string DatabaseLegacyDn { get; }

		Guid HomePublicFolderDatabaseGuid { get; }
	}
}
