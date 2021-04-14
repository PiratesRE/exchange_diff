using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncFilter
	{
		string Id { get; }

		bool IsItemInFilter(ISyncItemId itemId);

		void UpdateFilterState(SyncOperation syncOperation);
	}
}
