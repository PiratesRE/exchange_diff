using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionStreamInvalidParameter : MapiException
	{
		public ExExceptionStreamInvalidParameter(LID lid, string message) : base(lid, message, ErrorCodeValue.StreamInvalidParam)
		{
		}

		public ExExceptionStreamInvalidParameter(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.StreamInvalidParam, innerException)
		{
		}
	}
}
