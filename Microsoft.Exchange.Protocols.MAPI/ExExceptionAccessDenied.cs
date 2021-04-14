using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionAccessDenied : MapiException
	{
		public ExExceptionAccessDenied(LID lid, string message) : base(lid, message, ErrorCodeValue.NoAccess)
		{
		}

		public ExExceptionAccessDenied(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NoAccess, innerException)
		{
		}
	}
}
