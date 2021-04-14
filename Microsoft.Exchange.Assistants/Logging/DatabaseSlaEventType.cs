using System;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal enum DatabaseSlaEventType
	{
		StartDatabase,
		StopDatabase,
		StartMailboxTableQuery,
		EndMailboxTableQuery,
		ErrorMailboxTableQuery,
		DatabaseIsStopped,
		ErrorProcessingDatabase
	}
}
