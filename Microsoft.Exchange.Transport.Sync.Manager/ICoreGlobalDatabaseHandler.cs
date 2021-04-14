using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreGlobalDatabaseHandler
	{
		SortedDictionary<Guid, bool> FindLocalDatabasesFromAD();

		SortedDictionary<Guid, bool> FindLocalDatabasesFromAdminRPC();

		void OnNewDatabaseManager(DatabaseManager databaseManager);
	}
}
