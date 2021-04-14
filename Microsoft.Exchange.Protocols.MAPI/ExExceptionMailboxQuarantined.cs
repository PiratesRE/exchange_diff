using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionMailboxQuarantined : MapiException
	{
		public ExExceptionMailboxQuarantined(LID lid, string message) : base(lid, message, ErrorCodeValue.MailboxQuarantined)
		{
		}

		public ExExceptionMailboxQuarantined(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.MailboxQuarantined, innerException)
		{
		}
	}
}
