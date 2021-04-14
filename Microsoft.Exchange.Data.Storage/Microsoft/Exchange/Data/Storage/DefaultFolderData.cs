using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultFolderData
	{
		public DefaultFolderData(StoreObjectId folderId, bool idInitialized, bool hasInitialized)
		{
			this.folderId = folderId;
			this.idInitialized = idInitialized;
			this.hasInitialized = hasInitialized;
		}

		public DefaultFolderData(StoreObjectId folderId) : this(folderId, folderId != null, folderId != null)
		{
		}

		public DefaultFolderData(bool isInitialized)
		{
			this.hasInitialized = isInitialized;
		}

		public bool HasInitialized
		{
			get
			{
				return this.hasInitialized;
			}
		}

		public bool IdInitialized
		{
			get
			{
				return this.idInitialized;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		private readonly bool hasInitialized;

		private readonly bool idInitialized;

		private readonly StoreObjectId folderId;
	}
}
