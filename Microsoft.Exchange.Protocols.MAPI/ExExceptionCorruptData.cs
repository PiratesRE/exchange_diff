using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class ExExceptionCorruptData : MapiException
	{
		public ExExceptionCorruptData(LID lid, string message) : base(lid, message, ErrorCodeValue.CorruptData)
		{
		}

		public ExExceptionCorruptData(LID lid, string message, Exception innerException) : base(lid, message, ErrorCodeValue.CorruptData, innerException)
		{
		}
	}
}
