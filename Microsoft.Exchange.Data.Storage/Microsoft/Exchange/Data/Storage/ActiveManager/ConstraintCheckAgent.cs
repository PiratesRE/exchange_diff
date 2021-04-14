using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum ConstraintCheckAgent
	{
		None,
		MailboxDatabaseReplication,
		ContentIndexing,
		TestHook
	}
}
