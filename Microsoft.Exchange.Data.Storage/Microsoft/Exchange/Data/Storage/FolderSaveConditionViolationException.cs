using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class FolderSaveConditionViolationException : StoragePermanentException
	{
		public FolderSaveConditionViolationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FolderSaveConditionViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
