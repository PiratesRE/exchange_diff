using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common.Exceptions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SyncPermanentException : LocalizedException, ISyncException
	{
		public SyncPermanentException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public SyncPermanentException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		protected SyncPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private SyncPermanentException(bool isItemException, DetailedAggregationStatus detailedAggregationStatus, EventLogEntry eventLogEntry, Exception innerException, bool isPromotedFromTransientException) : base(LocalizedString.Empty, innerException)
		{
			SyncUtilities.ThrowIfArgumentNull("innerException", innerException);
			this.isItemException = isItemException;
			this.detailedAggregationStatus = detailedAggregationStatus;
			this.eventLogEntry = eventLogEntry;
			this.isPromotedFromTransientException = isPromotedFromTransientException;
		}

		public DetailedAggregationStatus DetailedAggregationStatus
		{
			get
			{
				return this.detailedAggregationStatus;
			}
		}

		public bool IsItemException
		{
			get
			{
				return this.isItemException;
			}
		}

		public bool IsPromotedFromTransientException
		{
			get
			{
				return this.isPromotedFromTransientException;
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

		public static SyncPermanentException CreateOperationLevelException(DetailedAggregationStatus detailedAggregationStatus, Exception innerException)
		{
			return new SyncPermanentException(false, detailedAggregationStatus, null, innerException, false);
		}

		public static SyncPermanentException CreateItemLevelException(Exception innerException)
		{
			return new SyncPermanentException(true, DetailedAggregationStatus.None, null, innerException, false);
		}

		public static SyncPermanentException CreateItemLevelExceptionPromotedFromTransientException(Exception innerException)
		{
			return new SyncPermanentException(true, DetailedAggregationStatus.None, null, innerException, true);
		}

		private readonly DetailedAggregationStatus detailedAggregationStatus;

		private EventLogEntry eventLogEntry;

		private bool isItemException;

		private bool isPromotedFromTransientException;
	}
}
