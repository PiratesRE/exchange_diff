using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class UpdateItemInRecoverableItemsResponseWrapper
	{
		internal UpdateItemInRecoverableItemsResponseWrapper(ItemType item, AttachmentType[] attachments, ConflictResults conflictResults)
		{
			this.Item = item;
			this.Attachments = attachments;
			this.ConflictResults = conflictResults;
		}

		internal ItemType Item { get; set; }

		internal AttachmentType[] Attachments { get; set; }

		internal ConflictResults ConflictResults { get; set; }
	}
}
