using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class ReportHelper<T> : IReportHelper where T : class
	{
		public ReportHelper(string reportFolder, string messageClass)
		{
			if (string.IsNullOrEmpty(reportFolder))
			{
				throw new ArgumentNullException("A Report folder name should be provided!");
			}
			if (string.IsNullOrEmpty(messageClass))
			{
				throw new ArgumentNullException("A Report message class must be provided!");
			}
			this.reportFolderName = reportFolder;
			this.reportMessageClass = messageClass;
		}

		public static List<ReportEntry> MergeEntries(List<ReportEntry> entries1, List<ReportEntry> entries2)
		{
			if (entries1 == null || entries1.Count == 0)
			{
				return entries2;
			}
			if (entries2 == null || entries2.Count == 0)
			{
				return entries1;
			}
			List<ReportEntry> list = new List<ReportEntry>(entries1.Count + entries2.Count);
			int num = 0;
			int num2 = 0;
			while (num < entries1.Count || num2 < entries2.Count)
			{
				ReportEntry reportEntry = (num < entries1.Count) ? entries1[num] : ReportEntry.MaxEntry;
				ReportEntry reportEntry2 = (num2 < entries2.Count) ? entries2[num2] : ReportEntry.MaxEntry;
				if (reportEntry2.CreationTime < reportEntry.CreationTime)
				{
					list.Add(reportEntry2);
					num2++;
				}
				else
				{
					list.Add(reportEntry);
					num++;
				}
			}
			return list;
		}

		void IReportHelper.Load(ReportData report, MapiStore storeToUse)
		{
			using (MoveObjectInfo<T> moveObjectInfo = new MoveObjectInfo<T>(Guid.Empty, storeToUse, report.MessageId, this.reportFolderName, this.reportMessageClass, this.CreateMessageSubject(report), this.CreateMessageSearchKey(report)))
			{
				if (!moveObjectInfo.OpenMessage())
				{
					report.Entries = null;
				}
				else
				{
					report.MessageId = moveObjectInfo.MessageId;
					report.Entries = this.DeserializeEntries(moveObjectInfo, false);
				}
			}
		}

		void IReportHelper.Flush(ReportData report, MapiStore storeToUse)
		{
			MapiUtils.RetryOnObjectChanged(delegate
			{
				using (MoveObjectInfo<T> moveObjectInfo = new MoveObjectInfo<T>(Guid.Empty, storeToUse, report.MessageId, this.reportFolderName, this.reportMessageClass, this.CreateMessageSubject(report), this.CreateMessageSearchKey(report)))
				{
					if (moveObjectInfo.OpenMessage())
					{
						report.MessageId = moveObjectInfo.MessageId;
						List<ReportEntry> entries = this.DeserializeEntries(moveObjectInfo, true);
						List<ReportEntry> entries2 = ReportHelper<T>.MergeEntries(entries, report.NewEntries) ?? new List<ReportEntry>();
						this.SaveEntries(entries2, moveObjectInfo);
					}
					else
					{
						List<ReportEntry> entries2 = report.Entries ?? new List<ReportEntry>();
						this.SaveEntries(entries2, moveObjectInfo);
						report.MessageId = moveObjectInfo.MessageId;
					}
					report.Entries = null;
				}
			});
		}

		void IReportHelper.Delete(ReportData report, MapiStore storeToUse)
		{
			MapiUtils.RetryOnObjectChanged(delegate
			{
				using (MoveObjectInfo<T> moveObjectInfo = new MoveObjectInfo<T>(Guid.Empty, storeToUse, report.MessageId, this.reportFolderName, this.reportMessageClass, this.CreateMessageSubject(report), this.CreateMessageSearchKey(report)))
				{
					moveObjectInfo.OpenMessage();
					if (moveObjectInfo.MessageFound)
					{
						moveObjectInfo.DeleteMessage();
					}
				}
			});
			report.MessageId = null;
		}

		protected abstract void SaveEntries(List<ReportEntry> entries, MoveObjectInfo<T> moveObjectInfo);

		protected abstract List<ReportEntry> DeserializeEntries(MoveObjectInfo<T> moveObjectInfo, bool loadLastChunkOnly);

		private string CreateMessageSubject(ReportData report)
		{
			return string.Format(CultureInfo.InvariantCulture, "Subject = Mailbox Move Report : {0}", new object[]
			{
				report.IdentifyingGuid.ToString()
			});
		}

		private byte[] CreateMessageSearchKey(ReportData report)
		{
			byte[] array = new byte[16];
			report.IdentifyingGuid.ToByteArray().CopyTo(array, 0);
			return array;
		}

		private readonly string reportFolderName;

		private readonly string reportMessageClass;
	}
}
