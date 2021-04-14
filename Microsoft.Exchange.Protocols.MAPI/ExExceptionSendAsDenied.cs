using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionSendAsDenied : MapiException
	{
		public ExExceptionSendAsDenied(LID lid, string message) : base(lid, message, ErrorCodeValue.SendAsDenied)
		{
		}

		public ExExceptionSendAsDenied(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.SendAsDenied, innerException)
		{
		}
	}
}
