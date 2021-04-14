using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionMailboxInTransit : MapiException
	{
		public ExExceptionMailboxInTransit(LID lid, string message) : base(lid, message, ErrorCodeValue.MailboxInTransit)
		{
		}

		public ExExceptionMailboxInTransit(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.MailboxInTransit, innerException)
		{
		}
	}
}
