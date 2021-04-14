using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NullDefaultFolderValidator : DefaultFolderValidator
	{
		internal override bool EnsureIsValid(DefaultFolderContext context, StoreObjectId folderId, Dictionary<string, DefaultFolderManager.FolderData> folderDataDictionary)
		{
			return true;
		}

		protected override bool ValidateInternal(DefaultFolderContext context, PropertyBag propertyBag)
		{
			return true;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
		}

		public NullDefaultFolderValidator() : base(new IValidator[0])
		{
		}
	}
}
