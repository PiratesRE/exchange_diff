using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class BufferTooSmall : StoreException
	{
		public BufferTooSmall(LID lid, string message) : base(lid, ErrorCodeValue.BufferTooSmall, message)
		{
		}

		public BufferTooSmall(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.BufferTooSmall, message, innerException)
		{
		}
	}
}
