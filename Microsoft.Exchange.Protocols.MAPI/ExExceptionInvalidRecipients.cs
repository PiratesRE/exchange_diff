﻿using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionInvalidRecipients : MapiException
	{
		public ExExceptionInvalidRecipients(LID lid, string message) : base(lid, message, ErrorCodeValue.InvalidRecipients)
		{
		}

		public ExExceptionInvalidRecipients(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.InvalidRecipients, innerException)
		{
		}
	}
}
