using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class UpdateItemResponseWrapper
	{
		internal UpdateItemResponseWrapper(ItemType item, ConflictResults conflictResults)
		{
			this.Item = item;
			this.ConflictResults = conflictResults;
		}

		internal ItemType Item { get; set; }

		internal ConflictResults ConflictResults { get; set; }
	}
}
