using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreDatabaseManager
	{
		bool FindSystemMailboxGuid(string systemMailboxName, out Guid systemMailboxGuid);

		bool StartCacheManager(DatabaseManager databaseManager);

		void ShutdownCacheManager(DatabaseManager databaseManager);
	}
}
