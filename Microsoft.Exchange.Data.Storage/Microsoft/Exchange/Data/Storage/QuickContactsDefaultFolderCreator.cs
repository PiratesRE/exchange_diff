using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class QuickContactsDefaultFolderCreator : MessageClassBasedDefaultFolderCreator
	{
		internal QuickContactsDefaultFolderCreator() : base(DefaultFolderType.Contacts, "IPF.Contact.MOC.QuickContacts", true)
		{
		}

		protected override void StampExtraPropertiesOnNewlyCreatedFolder(Folder folder)
		{
			folder[FolderSchema.IsHidden] = true;
			folder.Save();
			folder.Load(null);
		}
	}
}
