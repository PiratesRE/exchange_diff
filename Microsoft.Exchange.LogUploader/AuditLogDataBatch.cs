using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogUploaderProxy;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	internal class AuditLogDataBatch : LogDataBatch
	{
		public AuditLogDataBatch(int batchSizeInBytes, long beginOffSet, string fullLogName, string logPrefix) : base(batchSizeInBytes, beginOffSet, fullLogName, logPrefix)
		{
			this.records = new List<AuditRecord>();
			this.recordSerializer = new RecordSerializer();
		}

		public RecordSerializer RecordSerializer
		{
			get
			{
				RecordSerializer result;
				if ((result = this.recordSerializer) == null)
				{
					result = (this.recordSerializer = new RecordSerializer());
				}
				return result;
			}
		}

		public List<AuditRecord> GetRecords()
		{
			if (this.recordSerializer != null)
			{
				this.recordSerializer.Dispose();
				this.recordSerializer = null;
			}
			return this.records;
		}

		protected override bool ShouldProcessLogLine(ParsedReadOnlyRow parsedRow)
		{
			object field = parsedRow.UnParsedRow.GetField<object>(3);
			return field != null;
		}

		protected override void ProcessRowData(ParsedReadOnlyRow rowData)
		{
			ReadOnlyRow unParsedRow = rowData.UnParsedRow;
			AuditLogRecordType field = unParsedRow.GetField<AuditLogRecordType>(3);
			string field2 = unParsedRow.GetField<string>(4);
			AuditRecord auditRecord = this.RecordSerializer.Read<AuditRecord>(field2, field);
			switch (AuditUploaderConfig.GetAction(auditRecord.RecordType.ToString(), auditRecord.OrganizationName, auditRecord.UserId, auditRecord.Operation))
			{
			case Actions.LetThrough:
				this.records.Add(auditRecord);
				break;
			case Actions.Skip:
			case Actions.SkipAndLogEvent:
				break;
			default:
				return;
			}
		}

		private readonly List<AuditRecord> records;

		private RecordSerializer recordSerializer;
	}
}
