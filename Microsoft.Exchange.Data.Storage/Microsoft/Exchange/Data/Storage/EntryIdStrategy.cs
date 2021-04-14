using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EntryIdStrategy
	{
		internal EntryIdStrategy()
		{
		}

		internal abstract void GetDependentProperties(object location, IList<StorePropertyDefinition> result);

		internal abstract byte[] GetEntryId(DefaultFolderContext context);

		internal abstract void SetEntryId(DefaultFolderContext context, byte[] entryId);

		internal abstract FolderSaveResult UnsetEntryId(DefaultFolderContext context);

		internal static NoEntryIdStrategy NoEntryId = new NoEntryIdStrategy();
	}
}
