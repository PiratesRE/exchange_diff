using System;
using Microsoft.Exchange.Data.Directory.DirSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class BackSyncRecipient : ADDirSyncResult
	{
		public BackSyncRecipient()
		{
		}

		private BackSyncRecipient(PropertyBag bag) : base((ADPropertyBag)bag)
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SyncSchema.Instance;
			}
		}

		internal override ADDirSyncResult CreateInstance(PropertyBag propertyBag)
		{
			return new BackSyncRecipient(propertyBag);
		}
	}
}
