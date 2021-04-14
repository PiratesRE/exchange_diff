using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionOutOfMemory : MapiException
	{
		public ExExceptionOutOfMemory(LID lid, string message) : base(lid, message, ErrorCodeValue.NotEnoughMemory)
		{
		}

		public ExExceptionOutOfMemory(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NotEnoughMemory, innerException)
		{
		}
	}
}
