using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidParentFolderException : StoragePermanentException
	{
		public InvalidParentFolderException(LocalizedString message) : base(message)
		{
		}

		protected InvalidParentFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
