using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionRequiresRefResolve : MapiException
	{
		public ExExceptionRequiresRefResolve(LID lid, string message) : base(lid, message, ErrorCodeValue.RequiresRefResolve)
		{
		}

		public ExExceptionRequiresRefResolve(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.RequiresRefResolve, innerException)
		{
		}
	}
}
