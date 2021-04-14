using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OccurrenceInfo
	{
		internal OccurrenceInfo(VersionedId versionedId, ExDateTime occurrenceDateId, ExDateTime originalStartTime, ExDateTime startTime, ExDateTime endTime)
		{
			this.OriginalStartTime = originalStartTime;
			this.OccurrenceDateId = occurrenceDateId;
			this.VersionedId = versionedId;
			this.StartTime = startTime;
			this.EndTime = endTime;
		}

		public readonly VersionedId VersionedId;

		public readonly ExDateTime OccurrenceDateId;

		public readonly ExDateTime OriginalStartTime;

		public readonly ExDateTime StartTime;

		public readonly ExDateTime EndTime;

		public DifferencesBetweenBlobAndAttach BlobDifferences;
	}
}
