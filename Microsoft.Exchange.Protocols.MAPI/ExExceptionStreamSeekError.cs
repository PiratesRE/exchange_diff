using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionStreamSeekError : MapiException
	{
		public ExExceptionStreamSeekError(LID lid, string message) : base(lid, message, ErrorCodeValue.StreamSeekError)
		{
		}

		public ExExceptionStreamSeekError(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.StreamSeekError, innerException)
		{
		}
	}
}
