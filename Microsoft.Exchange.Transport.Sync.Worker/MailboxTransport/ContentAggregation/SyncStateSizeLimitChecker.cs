using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncStateSizeLimitChecker
	{
		public SyncStateSizeLimitChecker(long maxLoadedSyncStateSizeInBytes)
		{
			SyncUtilities.ThrowIfArgumentLessThanZero("maxLoadedSyncStateSizeInBytes", maxLoadedSyncStateSizeInBytes);
			this.maxLoadedSyncStateSizeInBytes = maxLoadedSyncStateSizeInBytes;
		}

		public virtual void CheckUncompressedSyncStateWithinBounds(Guid subscriptionGuid, string syncStateId, long currentSizeOfLoadedSyncStateInBytes)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("syncStateId", syncStateId);
			SyncUtilities.ThrowIfArgumentLessThanZero("currentSizeOfLoadedSyncStateInBytes", currentSizeOfLoadedSyncStateInBytes);
			UncompressedSyncStateSizeExceededException ex;
			if (this.IsUncompressedSyncStateExceededBounds(subscriptionGuid, syncStateId, currentSizeOfLoadedSyncStateInBytes, out ex))
			{
				throw ex;
			}
		}

		public virtual bool IsUncompressedSyncStateExceededBounds(Guid subscriptionGuid, string syncStateId, long currentSizeOfLoadedSyncStateInBytes, out UncompressedSyncStateSizeExceededException uncompressedSyncStateSizeExceededException)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("syncStateId", syncStateId);
			SyncUtilities.ThrowIfArgumentLessThanZero("currentSizeOfLoadedSyncStateInBytes", currentSizeOfLoadedSyncStateInBytes);
			uncompressedSyncStateSizeExceededException = null;
			if (currentSizeOfLoadedSyncStateInBytes >= this.maxLoadedSyncStateSizeInBytes)
			{
				uncompressedSyncStateSizeExceededException = new UncompressedSyncStateSizeExceededException(syncStateId, subscriptionGuid, ByteQuantifiedSize.FromBytes(Convert.ToUInt64(currentSizeOfLoadedSyncStateInBytes)), ByteQuantifiedSize.FromBytes(Convert.ToUInt64(this.maxLoadedSyncStateSizeInBytes)));
				return true;
			}
			return false;
		}

		public virtual void CheckCompressedSyncStateWithinBounds(Guid subscriptionGuid, string syncStateId, StoragePermanentException storagePermanentException)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("syncStateId", syncStateId);
			SyncUtilities.ThrowIfArgumentNull("storagePermanentException", storagePermanentException);
			CompressedSyncStateSizeExceededException ex;
			if (this.IsCompressedSyncStateExceededBounds(subscriptionGuid, syncStateId, storagePermanentException, out ex))
			{
				throw ex;
			}
		}

		public virtual bool IsCompressedSyncStateExceededBounds(Guid subscriptionGuid, string syncStateId, StoragePermanentException storagePermanentException, out CompressedSyncStateSizeExceededException compressedSyncStateSizeExceededException)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("syncStateId", syncStateId);
			SyncUtilities.ThrowIfArgumentNull("storagePermanentException", storagePermanentException);
			compressedSyncStateSizeExceededException = null;
			if (storagePermanentException.InnerException is MapiExceptionStreamSizeError)
			{
				compressedSyncStateSizeExceededException = new CompressedSyncStateSizeExceededException(syncStateId, subscriptionGuid, storagePermanentException);
				return true;
			}
			return false;
		}

		private readonly long maxLoadedSyncStateSizeInBytes;
	}
}
