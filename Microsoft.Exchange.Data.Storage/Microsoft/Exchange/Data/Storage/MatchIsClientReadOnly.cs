using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchIsClientReadOnly : IValidator
	{
		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			ExtendedFolderFlags? valueAsNullable = propertyBag.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
			return valueAsNullable != null && (valueAsNullable.Value & ExtendedFolderFlags.ReadOnly) == ExtendedFolderFlags.ReadOnly;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			ExtendedFolderFlags? valueAsNullable = folder.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
			if (valueAsNullable == null)
			{
				folder[FolderSchema.ExtendedFolderFlags] = ExtendedFolderFlags.ReadOnly;
				return;
			}
			folder[FolderSchema.ExtendedFolderFlags] = (ExtendedFolderFlags.ReadOnly | valueAsNullable.Value);
		}
	}
}
