using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.Mserve
{
	internal interface IMserveService
	{
		bool TrackDuplicatedAddEntries { get; set; }

		List<RecipientSyncOperation> Synchronize(RecipientSyncOperation op);

		List<RecipientSyncOperation> Synchronize(List<RecipientSyncOperation> operations);

		List<RecipientSyncOperation> Synchronize();

		void Reset();
	}
}
