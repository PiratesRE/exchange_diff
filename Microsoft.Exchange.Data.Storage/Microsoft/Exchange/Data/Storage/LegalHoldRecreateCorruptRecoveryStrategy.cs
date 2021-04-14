using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LegalHoldRecreateCorruptRecoveryStrategy : CorruptDataRecoveryStrategy
	{
		internal override void Recover(DefaultFolder defaultFolder, Exception e, ref DefaultFolderData defaultFolderData)
		{
			if (e is DefaultFolderPropertyValidationException && defaultFolderData.FolderId != null)
			{
				using (Folder folder = Folder.Bind(defaultFolder.Session, defaultFolderData.FolderId))
				{
					defaultFolder.SetProperties(folder);
				}
				return;
			}
			COWSettings cowsettings = new COWSettings(defaultFolder.Session);
			if (cowsettings.HoldEnabled())
			{
				CorruptDataRecoveryStrategy.Throw.Recover(defaultFolder, e, ref defaultFolderData);
				return;
			}
			CorruptDataRecoveryStrategy.Recreate.Recover(defaultFolder, e, ref defaultFolderData);
		}
	}
}
