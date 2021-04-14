using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchIsSystemFolder : IValidator
	{
		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			SystemFolderFlags? valueAsNullable = propertyBag.GetValueAsNullable<SystemFolderFlags>(InternalSchema.SystemFolderFlags);
			return valueAsNullable != null && (valueAsNullable.Value & SystemFolderFlags.SystemFolder) == SystemFolderFlags.SystemFolder;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			SystemFolderFlags systemFolderFlags = SystemFolderFlags.SystemFolder;
			SystemFolderFlags? valueAsNullable = folder.GetValueAsNullable<SystemFolderFlags>(InternalSchema.SystemFolderFlags);
			if (valueAsNullable != null)
			{
				systemFolderFlags |= valueAsNullable.Value;
			}
			folder[InternalSchema.SystemFolderFlags] = systemFolderFlags;
		}
	}
}
