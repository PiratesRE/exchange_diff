using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class FolderIdProperty : FolderIdPropertyBase
	{
		protected FolderIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static FolderIdProperty CreateCommand(CommandContext commandContext)
		{
			return new FolderIdProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			if (!base.TryCheckAndConvertToPublicFolderIdFromStoreObject(CommandOptions.ConvertFolderIdToPublicFolderId))
			{
				base.ToServiceObject();
			}
		}

		public override void ToServiceObjectForPropertyBag()
		{
			if (!base.TryCheckAndConvertToPublicFolderIdFromPropertyBag(CommandOptions.ConvertFolderIdToPublicFolderId, CoreFolderSchema.Id))
			{
				base.ToServiceObjectForPropertyBag();
			}
		}
	}
}
