using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class AttachmentExceededException : StoragePermanentException
	{
		public AttachmentExceededException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AttachmentExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
