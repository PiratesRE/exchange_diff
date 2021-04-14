using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class CompressedReportHelper : ReportHelper<CompressedReport>
	{
		public CompressedReportHelper(string reportFolder, string messageClass) : base(reportFolder, messageClass)
		{
		}

		protected override void SaveEntries(List<ReportEntry> entries, MoveObjectInfo<CompressedReport> moveObjectInfo)
		{
			if (entries != null && entries.Count > 100)
			{
				List<CompressedReport> list = new List<CompressedReport>();
				int num = 0;
				while (num + 100 < entries.Count)
				{
					list.Add(new CompressedReport(entries.GetRange(num, 100)));
					num += 100;
				}
				list.Add(new CompressedReport(entries.GetRange(num, entries.Count - num)));
				int num2 = TestIntegration.Instance.MaxReportEntryCount / 100;
				if (num2 < 1)
				{
					num2 = 1;
				}
				moveObjectInfo.SaveObjectChunks(list, num2, null);
				return;
			}
			moveObjectInfo.SaveObject(new CompressedReport(entries));
		}

		protected override List<ReportEntry> DeserializeEntries(MoveObjectInfo<CompressedReport> moveObjectInfo, bool loadLastChunkOnly)
		{
			ReadObjectFlags readObjectFlags = ReadObjectFlags.DontThrowOnCorruptData;
			if (loadLastChunkOnly)
			{
				readObjectFlags |= ReadObjectFlags.LastChunkOnly;
			}
			List<CompressedReport> list = moveObjectInfo.ReadObjectChunks(readObjectFlags);
			if (list == null)
			{
				return null;
			}
			List<ReportEntry> list2 = new List<ReportEntry>();
			foreach (CompressedReport compressedReport in list)
			{
				list2.AddRange(compressedReport.Entries);
			}
			return list2;
		}

		public const int MaxReportEntryCountDefault = 10000;

		public const int ChunkSize = 100;
	}
}
