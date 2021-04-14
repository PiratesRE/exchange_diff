using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointBindingSchema : Schema
	{
		public new static SharepointBindingSchema Instance
		{
			get
			{
				if (SharepointBindingSchema.instance == null)
				{
					SharepointBindingSchema.instance = new SharepointBindingSchema();
				}
				return SharepointBindingSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition ProviderGuid = InternalSchema.SharingProviderGuid;

		public static readonly StorePropertyDefinition SharepointFolder = InternalSchema.SharingRemotePath;

		public static readonly StorePropertyDefinition SharepointFolderDisplayName = InternalSchema.SharingRemoteName;

		private static SharepointBindingSchema instance = null;
	}
}
