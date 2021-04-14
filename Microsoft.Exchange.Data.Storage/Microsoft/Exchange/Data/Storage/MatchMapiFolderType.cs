using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchMapiFolderType : IValidator
	{
		internal MatchMapiFolderType(FolderType folderType)
		{
			this.folderType = folderType;
		}

		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			FolderType? valueAsNullable = propertyBag.GetValueAsNullable<FolderType>(InternalSchema.MapiFolderType);
			return valueAsNullable != null && valueAsNullable.Value == this.folderType;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
		}

		private FolderType folderType;
	}
}
