using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[Serializable]
	public class PublicFolderSyncTransientException : StorageTransientException
	{
		public PublicFolderSyncTransientException(LocalizedString message) : base(message)
		{
		}

		public PublicFolderSyncTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PublicFolderSyncTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
