using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ReceiveFolderInfo
	{
		public ReceiveFolderInfo(byte[] entryId, string messageClass, ExDateTime lastModification)
		{
			this = default(ReceiveFolderInfo);
			this.FolderId = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Folder);
			this.MessageClass = messageClass;
			this.LastModification = lastModification;
		}

		public StoreObjectId FolderId { get; private set; }

		public string MessageClass { get; private set; }

		public ExDateTime LastModification { get; private set; }
	}
}
