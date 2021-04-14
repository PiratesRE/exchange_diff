using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ItemAttachmentSchema : AttachmentSchema
	{
		public new static ItemAttachmentSchema Instance
		{
			get
			{
				if (ItemAttachmentSchema.instance == null)
				{
					ItemAttachmentSchema.instance = new ItemAttachmentSchema();
				}
				return ItemAttachmentSchema.instance;
			}
		}

		private static ItemAttachmentSchema instance;
	}
}
