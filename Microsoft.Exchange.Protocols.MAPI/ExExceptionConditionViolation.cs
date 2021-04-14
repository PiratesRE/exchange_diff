using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionConditionViolation : MapiException
	{
		public ExExceptionConditionViolation(LID lid, string message) : base(lid, message, ErrorCodeValue.ConditionViolation)
		{
		}

		public ExExceptionConditionViolation(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.ConditionViolation, innerException)
		{
		}
	}
}
