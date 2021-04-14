using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ProgressRecord
	{
		public ProgressRecord()
		{
		}

		public ProgressRecord(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.ItemExportedRecords = new List<ExportRecord>();
			this.ItemErrorRecords = new List<ErrorRecord>();
			this.Duration = TimeSpan.Zero;
		}

		public DataContext DataContext { get; private set; }

		public List<ExportRecord> ItemExportedRecords { get; private set; }

		public List<ErrorRecord> ItemErrorRecords { get; private set; }

		public long Size { get; private set; }

		public TimeSpan Duration { get; private set; }

		public ExportRecord RootExportedRecord { get; private set; }

		public ErrorRecord SourceErrorRecord { get; private set; }

		public void ReportRootRecord(ExportRecord rootExportRecord)
		{
			this.RootExportedRecord = rootExportRecord;
		}

		public void ReportItemExported(ItemId itemId, string targetMessageId, ExportRecord rootExportRecord)
		{
			this.ItemExportedRecords.Add(new ExportRecord
			{
				Id = itemId.Id,
				Parent = rootExportRecord,
				SourceId = itemId.SourceId,
				ExportFile = null,
				OriginalPath = itemId.ReportingPath,
				TargetPath = targetMessageId,
				PrimaryIdOfDuplicates = itemId.PrimaryItemId,
				Title = itemId.Subject,
				Size = itemId.Size,
				Sender = itemId.Sender,
				SenderSmtpAddress = itemId.SenderSmtpAddress,
				SentTime = itemId.SentTime,
				ReceivedTime = itemId.ReceivedTime,
				BodyPreview = itemId.BodyPreview,
				Importance = itemId.Importance,
				IsRead = itemId.IsRead,
				HasAttachment = itemId.HasAttachment,
				ToRecipients = itemId.ToRecipients,
				CcRecipients = itemId.CcRecipients,
				BccRecipients = itemId.BccRecipients,
				DocumentId = itemId.DocumentId,
				InternetMessageId = itemId.InternetMessageId,
				ToGroupExpansionRecipients = itemId.ToGroupExpansionRecipients,
				CcGroupExpansionRecipients = itemId.CcGroupExpansionRecipients,
				BccGroupExpansionRecipients = itemId.BccGroupExpansionRecipients,
				DGGroupExpansionError = itemId.DGGroupExpansionError.ToString(),
				DocumentType = "Message",
				RelationshipType = "Container",
				IsUnsearchable = this.DataContext.IsUnsearchable
			});
			if (!itemId.IsDuplicate)
			{
				this.Size += (long)((ulong)itemId.Size);
			}
		}

		public void ReportItemError(ItemId itemId, ExportRecord rootExportRecord, ExportErrorType errorType, string diagnosticMessage)
		{
			this.ItemErrorRecords.Add(new ErrorRecord
			{
				Item = new ExportRecord
				{
					Id = itemId.Id,
					DocumentId = itemId.DocumentId,
					InternetMessageId = itemId.InternetMessageId,
					Parent = rootExportRecord,
					Size = itemId.Size,
					SourceId = itemId.SourceId,
					ExportFile = null,
					OriginalPath = itemId.ReportingPath,
					Title = (string.IsNullOrEmpty(itemId.Subject) ? string.Empty : itemId.Subject),
					IsUnsearchable = this.DataContext.IsUnsearchable
				},
				ErrorType = errorType,
				DiagnosticMessage = diagnosticMessage,
				SourceId = itemId.SourceId,
				Time = DateTime.UtcNow
			});
			if (!itemId.IsDuplicate)
			{
				this.Size += (long)((ulong)itemId.Size);
			}
		}

		public void ReportSourceError(ErrorRecord errorRecord)
		{
			this.SourceErrorRecord = errorRecord;
		}

		public void ReportDuration(TimeSpan duration)
		{
			this.Duration = duration;
		}
	}
}
