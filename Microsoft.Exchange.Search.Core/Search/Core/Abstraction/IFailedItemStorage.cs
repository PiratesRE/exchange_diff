using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFailedItemStorage : IDisposable
	{
		long GetFailedItemsCount(FailedItemParameters parameters);

		ICollection<IFailureEntry> GetFailedItems(FailedItemParameters parameters);

		long GetPermanentFailureCount();

		ICollection<IFailureEntry> GetRetriableItems(FieldSet fields);

		ICollection<IDocEntry> GetDeletionPendingItems(int deletedMailboxNumber);

		long GetItemsCount(Guid filterMailboxGuid);

		ICollection<long> GetPoisonDocuments();
	}
}
