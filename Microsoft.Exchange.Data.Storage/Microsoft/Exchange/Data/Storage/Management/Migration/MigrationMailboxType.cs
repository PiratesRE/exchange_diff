using System;

namespace Microsoft.Exchange.Data.Storage.Management.Migration
{
	[Serializable]
	internal enum MigrationMailboxType
	{
		PrimaryAndArchive,
		PrimaryOnly,
		ArchiveOnly
	}
}
