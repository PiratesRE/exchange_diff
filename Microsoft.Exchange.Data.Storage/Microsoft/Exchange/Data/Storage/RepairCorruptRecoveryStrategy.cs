using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RepairCorruptRecoveryStrategy : CorruptDataRecoveryStrategy
	{
		internal override void Recover(DefaultFolder defaultFolder, Exception e, ref DefaultFolderData defaultFolderData)
		{
			if (defaultFolderData.FolderId != null)
			{
				using (Folder folder = Folder.Bind(defaultFolder.Session, defaultFolderData.FolderId))
				{
					defaultFolder.SetProperties(folder);
				}
			}
		}
	}
}
