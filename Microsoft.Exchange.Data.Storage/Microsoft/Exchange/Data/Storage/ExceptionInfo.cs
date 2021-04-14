using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExceptionInfo : OccurrenceInfo
	{
		public ExceptionInfo(VersionedId versionedId, ExDateTime occurrenceDateId, ExDateTime originalStartTime, ExDateTime startTime, ExDateTime endTime, ModificationType modificationType, MemoryPropertyBag propertyBag) : base(versionedId, occurrenceDateId, originalStartTime, startTime, endTime)
		{
			this.ModificationType = modificationType;
			this.PropertyBag = propertyBag;
		}

		internal ExceptionInfo(VersionedId versionedId, ExceptionInfo exceptionInfo) : base(versionedId, exceptionInfo.OccurrenceDateId, exceptionInfo.OriginalStartTime, exceptionInfo.StartTime, exceptionInfo.EndTime)
		{
			this.PropertyBag = new MemoryPropertyBag(exceptionInfo.PropertyBag);
			this.ModificationType = exceptionInfo.ModificationType;
			this.BlobDifferences = exceptionInfo.BlobDifferences;
		}

		public readonly MemoryPropertyBag PropertyBag;

		public ModificationType ModificationType;
	}
}
