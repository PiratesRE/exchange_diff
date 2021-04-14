using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RecreateCorruptDataRecoveryStrategy : CorruptDataRecoveryStrategy
	{
		internal override void Recover(DefaultFolder defaultFolder, Exception e, ref DefaultFolderData defaultFolderData)
		{
			try
			{
				defaultFolder.RemoveForRecover(ref defaultFolderData);
				defaultFolder.CreateInternal(ref defaultFolderData);
			}
			catch (StoragePermanentException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExCorruptDataRecoverError(defaultFolder.ToString()), innerException);
			}
			catch (StorageTransientException innerException2)
			{
				throw new CorruptDataException(ServerStrings.ExCorruptDataRecoverError(defaultFolder.ToString()), innerException2);
			}
		}
	}
}
