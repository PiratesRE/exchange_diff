using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[Serializable]
	public class PublicFolderSyncPermanentException : StoragePermanentException
	{
		public PublicFolderSyncPermanentException(LocalizedString message) : base(message)
		{
		}

		public PublicFolderSyncPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PublicFolderSyncPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
