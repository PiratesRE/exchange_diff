using System;
using Microsoft.Office.Compliance.Audit;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public struct RecordProcessingResult
	{
		public RecordProcessingResult(AuditRecord record, Exception exception, bool retry)
		{
			this = default(RecordProcessingResult);
			this.RecordType = record.RecordType;
			this.FailTime = DateTime.UtcNow;
			this.RecordId = record.Id;
			this.Exception = ExceptionDetails.FromException(exception);
			this.Retry = retry;
			this.Index = RecordProcessingResult.count++;
			RecordProcessingResult.count %= 25;
		}

		public AuditLogRecordType RecordType { get; set; }

		public DateTime FailTime { get; set; }

		public Guid RecordId { get; set; }

		public bool Retry { get; set; }

		public ExceptionDetails Exception { get; set; }

		internal int Index { get; set; }

		private static int count;
	}
}
