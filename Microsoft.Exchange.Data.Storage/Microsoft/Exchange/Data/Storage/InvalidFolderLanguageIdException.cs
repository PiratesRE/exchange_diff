using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidFolderLanguageIdException : StoragePermanentException
	{
		public InvalidFolderLanguageIdException(LocalizedString message) : base(message)
		{
		}

		public InvalidFolderLanguageIdException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidFolderLanguageIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
