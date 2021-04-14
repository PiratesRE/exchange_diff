using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UndeleteContext
	{
		internal UndeleteContext(StoreObjectId parentId, byte[] searchKey)
		{
			this.searchKey = searchKey;
			this.parentFolderId = parentId;
		}

		internal StoreObjectId ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		internal byte[] SearchKey
		{
			get
			{
				return this.searchKey;
			}
		}

		private StoreObjectId parentFolderId;

		private byte[] searchKey;
	}
}
