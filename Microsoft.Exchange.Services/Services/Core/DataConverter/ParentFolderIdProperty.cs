using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ParentFolderIdProperty : FolderIdPropertyBase
	{
		private ParentFolderIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		protected override StoreId GetIdFromObject(StoreObject storeObject)
		{
			if (storeObject.Id != null && storeObject.Id.ObjectId.ProviderLevelItemId.Length > 0)
			{
				return storeObject.ParentId;
			}
			return null;
		}

		public static ParentFolderIdProperty CreateCommand(CommandContext commandContext)
		{
			return new ParentFolderIdProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			if (!base.TryCheckAndConvertToPublicFolderIdFromStoreObject(CommandOptions.ConvertParentFolderIdToPublicFolderId))
			{
				base.ToServiceObject();
			}
		}

		public override void ToServiceObjectForPropertyBag()
		{
			if (!base.TryCheckAndConvertToPublicFolderIdFromPropertyBag(CommandOptions.ConvertParentFolderIdToPublicFolderId, StoreObjectSchema.ParentItemId))
			{
				base.ToServiceObjectForPropertyBag();
			}
		}
	}
}
