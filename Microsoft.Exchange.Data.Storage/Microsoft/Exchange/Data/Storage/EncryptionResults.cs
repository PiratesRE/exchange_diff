using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EncryptionResults
	{
		public EncryptedSharedFolderData[] EncryptedSharedFolderDataCollection { get; private set; }

		public InvalidRecipient[] InvalidRecipients { get; private set; }

		internal EncryptionResults(EncryptedSharedFolderData[] encryptedSharedFolderDataCollection, InvalidRecipient[] invalidRecipients)
		{
			this.EncryptedSharedFolderDataCollection = encryptedSharedFolderDataCollection;
			this.InvalidRecipients = invalidRecipients;
		}
	}
}
