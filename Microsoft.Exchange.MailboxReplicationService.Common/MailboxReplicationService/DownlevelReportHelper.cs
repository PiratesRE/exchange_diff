using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DownlevelReportHelper : ReportHelper<List<ReportEntry>>
	{
		public DownlevelReportHelper(string reportFolder, string messageClass) : base(reportFolder, messageClass)
		{
		}

		protected override void SaveEntries(List<ReportEntry> entries, MoveObjectInfo<List<ReportEntry>> moveObjectInfo)
		{
			moveObjectInfo.SaveObject(entries);
		}

		protected override List<ReportEntry> DeserializeEntries(MoveObjectInfo<List<ReportEntry>> moveObjectInfo, bool loadLastChunkOnly)
		{
			return moveObjectInfo.ReadObject(ReadObjectFlags.DontThrowOnCorruptData);
		}
	}
}
