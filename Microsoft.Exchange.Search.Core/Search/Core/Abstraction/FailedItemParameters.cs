using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal class FailedItemParameters
	{
		public FailedItemParameters(FailureMode failureMode, FieldSet fields)
		{
			this.ResultLimit = int.MaxValue;
			this.Fields = fields;
			this.FailureMode = failureMode;
			this.Culture = CultureInfo.InvariantCulture;
		}

		public Guid? MailboxGuid { get; set; }

		public FieldSet Fields { get; set; }

		public int? ErrorCode { get; set; }

		public FailureMode FailureMode { get; set; }

		public ExDateTime? StartDate { get; set; }

		public ExDateTime? EndDate { get; set; }

		public StoreObjectId ParentEntryId { get; set; }

		public int ResultLimit { get; set; }

		public long StartingIndexId { get; set; }

		public bool? IsPartiallyProcessed { get; set; }

		public CultureInfo Culture { get; set; }
	}
}
