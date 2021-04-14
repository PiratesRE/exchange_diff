using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SkippedItemUtilities
	{
		public static ReportData GetReportData(Guid guid)
		{
			return new ReportData(guid, ReportVersion.ReportE14R6Compression, "TransportSync Reports", "IPM.MS-Exchange.TransportSyncReports");
		}

		private const string ReportFolderName = "TransportSync Reports";

		private const string ReportMessageClass = "IPM.MS-Exchange.TransportSyncReports";
	}
}
