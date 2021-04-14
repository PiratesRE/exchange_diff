using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequestStatusInfo
	{
		public AsyncQueueStatus Status { get; set; }

		public DateTime? StartDatetime { get; set; }

		public DateTime? EndDatetime { get; set; }
	}
}
