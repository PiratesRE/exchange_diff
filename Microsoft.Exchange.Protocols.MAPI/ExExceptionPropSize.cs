using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionPropSize : MapiException
	{
		public ExExceptionPropSize(LID lid, string message) : base(lid, message, ErrorCodeValue.NotEnoughMemory)
		{
		}

		public ExExceptionPropSize(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.NotEnoughMemory, innerException)
		{
		}
	}
}
