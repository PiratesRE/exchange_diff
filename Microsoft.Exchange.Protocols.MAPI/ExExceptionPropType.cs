using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionPropType : MapiException
	{
		public ExExceptionPropType(LID lid, string message) : base(lid, message, ErrorCodeValue.UnexpectedType)
		{
		}

		public ExExceptionPropType(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.UnexpectedType, innerException)
		{
		}
	}
}
