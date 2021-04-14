using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility
{
	internal struct PayloadIdentifier
	{
		public Guid JobRunId { get; set; }

		public int TaskId { get; set; }
	}
}
