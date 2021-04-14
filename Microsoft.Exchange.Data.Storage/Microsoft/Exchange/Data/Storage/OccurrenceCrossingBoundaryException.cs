using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OccurrenceCrossingBoundaryException : RecurrenceException
	{
		public OccurrenceCrossingBoundaryException(OccurrenceInfo occurrenceInfo, OccurrenceInfo neighborInfo, LocalizedString message, bool isSameDayInOrganizerTimeZone) : base(message, null)
		{
			this.OccurrenceInfo = occurrenceInfo;
			this.NeighborInfo = neighborInfo;
			this.IsSameDayInOrganizerTimeZone = isSameDayInOrganizerTimeZone;
		}

		public readonly OccurrenceInfo OccurrenceInfo;

		public readonly OccurrenceInfo NeighborInfo;

		public readonly bool IsSameDayInOrganizerTimeZone;
	}
}
