using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidFolderTypeException : StoragePermanentException
	{
		public InvalidFolderTypeException(LocalizedString message) : base(message)
		{
		}

		public InvalidFolderTypeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidFolderTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
