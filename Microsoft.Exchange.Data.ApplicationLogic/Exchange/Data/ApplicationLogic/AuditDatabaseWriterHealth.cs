using System;
using System.Linq;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public class AuditDatabaseWriterHealth
	{
		public int FailedBatchCount { get; set; }

		public int RecordCount { get; set; }

		public int BatchRetryCount { get; set; }

		public RecordProcessingResult[] BadRecords
		{
			get
			{
				return (from br in this.badRecords
				where br.Exception != null
				orderby br.Index
				select br).ToArray<RecordProcessingResult>();
			}
			set
			{
				throw new NotSupportedException("Do not set BadRecords.");
			}
		}

		public void Add(RecordProcessingResult result)
		{
			this.badRecords[result.Index] = result;
		}

		public const int MaxRecordProcessingResultCount = 25;

		private RecordProcessingResult[] badRecords = new RecordProcessingResult[25];
	}
}
