using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SyncTransientException : TransientException, ISyncException
	{
		private SyncTransientException(bool isItemException, DetailedAggregationStatus detailedAggregationStatus, EventLogEntry eventLogEntry, bool needsBackoff, Exception innerException) : base(LocalizedString.Empty, innerException)
		{
			SyncUtilities.ThrowIfArgumentNull("innerException", innerException);
			this.isItemException = isItemException;
			this.detailedAggregationStatus = detailedAggregationStatus;
			this.eventLogEntry = eventLogEntry;
			this.needsBackoff = needsBackoff;
		}

		public DetailedAggregationStatus DetailedAggregationStatus
		{
			get
			{
				return this.detailedAggregationStatus;
			}
		}

		public bool NeedsBackoff
		{
			get
			{
				return this.needsBackoff;
			}
		}

		public EventLogEntry EventLogEntry
		{
			get
			{
				return this.eventLogEntry;
			}
			set
			{
				this.eventLogEntry = value;
			}
		}

		public bool IsItemException
		{
			get
			{
				return this.isItemException;
			}
		}

		public static SyncTransientException CreateOperationLevelException(DetailedAggregationStatus detailedAggregationStatus, Exception innerException, bool needsBackoff)
		{
			return new SyncTransientException(false, detailedAggregationStatus, null, needsBackoff, innerException);
		}

		public static SyncTransientException CreateOperationLevelException(DetailedAggregationStatus detailedAggregationStatus, Exception innerException)
		{
			return SyncTransientException.CreateOperationLevelException(detailedAggregationStatus, innerException, SyncTransientException.DefaultBackoff);
		}

		public static SyncTransientException CreateItemLevelException(Exception innerException)
		{
			return new SyncTransientException(true, DetailedAggregationStatus.None, null, false, innerException);
		}

		private static readonly bool DefaultBackoff;

		private readonly DetailedAggregationStatus detailedAggregationStatus;

		private readonly bool needsBackoff;

		private bool isItemException;

		private EventLogEntry eventLogEntry;
	}
}
