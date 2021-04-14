using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionTooManyRecipients : MapiException
	{
		public ExExceptionTooManyRecipients(LID lid, string message) : base(lid, message, ErrorCodeValue.TooManyRecips)
		{
		}

		public ExExceptionTooManyRecipients(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.TooManyRecips, innerException)
		{
		}
	}
}
