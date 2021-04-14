using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal static class Constants
	{
		internal static readonly int ReadWriteBatchSize = 100;

		internal static readonly string MailboxItemIdPropertyIdSet = "2B55BDD4-255C-4C04-84B7-5B15DCCB9B15";

		internal static readonly string MailboxSearchRecycleFolderName = "MailboxSearchRecycleBin";

		internal static readonly string WorkingFolderSuffix = ".working";

		internal static readonly string ResultPathSeparator = "###";

		internal static readonly string SourceConfigurationLogItemSubjectPrefix = "configuration-";

		internal static readonly string SourceStatusLogItemSubjectPrefix = "status-";

		internal static readonly string SourceOperationStatusLogItemSubject = "operation-status";
	}
}
