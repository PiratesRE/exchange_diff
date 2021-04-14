using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PropertyBlob
{
	public class InvalidBlobException : InvalidSerializedFormatException
	{
		public InvalidBlobException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public InvalidBlobException(string message) : base(message)
		{
		}
	}
}
