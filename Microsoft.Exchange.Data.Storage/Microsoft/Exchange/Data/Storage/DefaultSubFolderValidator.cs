using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultSubFolderValidator : DefaultFolderValidator
	{
		internal DefaultSubFolderValidator(DefaultFolderType parentFolderType, params IValidator[] validators) : base(validators)
		{
			this.parentFolderType = parentFolderType;
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			if (!base.EnsureIsValid(context, folder.PropertyBag))
			{
				return false;
			}
			StoreObjectId storeObjectId = context[this.parentFolderType];
			return storeObjectId == null || storeObjectId.Equals(folder.ParentId);
		}

		private DefaultFolderType parentFolderType;
	}
}
