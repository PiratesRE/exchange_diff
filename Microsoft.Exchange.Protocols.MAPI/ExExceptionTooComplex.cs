using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionTooComplex : MapiException
	{
		public ExExceptionTooComplex(LID lid, string message) : base(lid, message, ErrorCodeValue.TooComplex)
		{
		}

		public ExExceptionTooComplex(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.TooComplex, innerException)
		{
		}
	}
}
