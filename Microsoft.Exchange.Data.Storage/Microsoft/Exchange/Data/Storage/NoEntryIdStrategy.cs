using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NoEntryIdStrategy : EntryIdStrategy
	{
		internal override void GetDependentProperties(object location, IList<StorePropertyDefinition> result)
		{
		}

		internal override byte[] GetEntryId(DefaultFolderContext context)
		{
			return null;
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			throw new NotSupportedException("NoEntryIdStrategy does not support Set.");
		}

		internal override FolderSaveResult UnsetEntryId(DefaultFolderContext context)
		{
			throw new NotSupportedException("NoEntryIdStrategy does not support Unset.");
		}
	}
}
