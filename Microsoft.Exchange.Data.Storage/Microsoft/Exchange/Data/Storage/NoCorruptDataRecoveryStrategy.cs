using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NoCorruptDataRecoveryStrategy : CorruptDataRecoveryStrategy
	{
		internal override void Recover(DefaultFolder defaultFolder, Exception e, ref DefaultFolderData defaultFolderData)
		{
			defaultFolder.RemoveForRecover(ref defaultFolderData);
		}
	}
}
