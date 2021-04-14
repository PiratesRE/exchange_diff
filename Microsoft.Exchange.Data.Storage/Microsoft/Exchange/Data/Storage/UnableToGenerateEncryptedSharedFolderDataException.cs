using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class UnableToGenerateEncryptedSharedFolderDataException : StoragePermanentException
	{
		public UnableToGenerateEncryptedSharedFolderDataException(Exception innerException) : base(ServerStrings.SharingUnableToGenerateEncryptedSharedFolderData, innerException)
		{
		}
	}
}
