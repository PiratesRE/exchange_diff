using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionMaxSubmissionExceeded : MapiException
	{
		public ExExceptionMaxSubmissionExceeded(LID lid, string message) : base(lid, message, ErrorCodeValue.MaxSubmissionExceeded)
		{
		}

		public ExExceptionMaxSubmissionExceeded(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.MaxSubmissionExceeded, innerException)
		{
		}
	}
}
