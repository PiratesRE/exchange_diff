using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.AttachmentBlob
{
	public class InvalidAttachmentBlobException : InvalidSerializedFormatException
	{
		public InvalidAttachmentBlobException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public InvalidAttachmentBlobException(string message) : base(message)
		{
		}
	}
}
