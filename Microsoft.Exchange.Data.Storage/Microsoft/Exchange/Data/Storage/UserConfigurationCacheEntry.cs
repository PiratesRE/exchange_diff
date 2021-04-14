using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class UserConfigurationCacheEntry
	{
		public UserConfigurationCacheEntry(string configName, StoreObjectId folderId, StoreObjectId itemId)
		{
			Util.ThrowOnNullArgument(configName, "configName");
			this.configName = configName;
			this.folderId = folderId;
			this.itemId = itemId;
		}

		public bool CheckMatch(string configName, StoreObjectId folderId)
		{
			return string.Equals(configName, this.configName, StringComparison.OrdinalIgnoreCase) && folderId.Equals(this.folderId);
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string ConfigurationName
		{
			get
			{
				return this.configName;
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		private readonly string configName;

		private readonly StoreObjectId folderId;

		private readonly StoreObjectId itemId;
	}
}
