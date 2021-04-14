using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class AttachmentOpenException : StoragePermanentException
	{
		public AttachmentOpenException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AttachmentOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
