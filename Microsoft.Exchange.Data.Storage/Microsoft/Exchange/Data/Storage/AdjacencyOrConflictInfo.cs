using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdjacencyOrConflictInfo
	{
		public AdjacencyOrConflictInfo(OccurrenceInfo occurrenceInfo, string subject, string location, BusyType freeBusyType, AdjacencyOrConflictType type, byte[] globalObjectId, Sensitivity sensitivity, bool isAllDayEvent)
		{
			EnumValidator.ThrowIfInvalid<BusyType>(freeBusyType, "freeBusyType");
			EnumValidator.ThrowIfInvalid<AdjacencyOrConflictType>(type, "type");
			EnumValidator.ThrowIfInvalid<Sensitivity>(sensitivity, "sensitivity");
			this.OccurrenceInfo = occurrenceInfo;
			this.Subject = subject;
			this.Location = location;
			this.FreeBusyStatus = freeBusyType;
			this.AdjacencyOrConflictType = type;
			this.GlobalObjectId = globalObjectId;
			this.Sensitivity = sensitivity;
			this.IsAllDayEvent = isAllDayEvent;
		}

		public readonly OccurrenceInfo OccurrenceInfo;

		public readonly string Subject;

		public readonly string Location;

		public readonly BusyType FreeBusyStatus;

		public readonly AdjacencyOrConflictType AdjacencyOrConflictType;

		public readonly byte[] GlobalObjectId;

		public readonly Sensitivity Sensitivity;

		public readonly bool IsAllDayEvent;
	}
}
