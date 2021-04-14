using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecurringItemLatencyInformation
	{
		public string Subject { get; set; }

		public long BlobStreamTime { get; set; }

		public long BlobParseTime { get; set; }

		public long BlobExpansionTime { get; set; }

		public long AddRowsForInstancesTime { get; set; }
	}
}
