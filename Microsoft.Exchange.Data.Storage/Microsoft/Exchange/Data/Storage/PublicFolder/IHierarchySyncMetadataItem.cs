using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IHierarchySyncMetadataItem : IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		ExDateTime LastAttemptedSyncTime { get; set; }

		ExDateTime LastFailedSyncTime { get; set; }

		ExDateTime LastSuccessfulSyncTime { get; set; }

		ExDateTime FirstFailedSyncTimeAfterLastSuccess { get; set; }

		string LastSyncFailure { get; set; }

		int NumberOfAttemptsAfterLastSuccess { get; set; }

		int NumberOfBatchesExecuted { get; set; }

		int NumberOfFoldersSynced { get; set; }

		int NumberOfFoldersToBeSynced { get; set; }

		int BatchSize { get; set; }

		void SetPartiallyCommittedFolderIds(IdSet value);

		IdSet GetPartiallyCommittedFolderIds();

		Stream GetSyncStateReadStream();

		Stream GetSyncStateOverrideStream();

		Stream GetFinalJobSyncStateReadStream();

		Stream GetFinalJobSyncStateWriteStream(bool overrideIfExisting);

		void CommitSyncStateForCompletedBatch();

		void ClearSyncState();
	}
}
