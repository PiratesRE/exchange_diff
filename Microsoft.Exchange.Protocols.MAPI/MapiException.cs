using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public abstract class MapiException : StoreException
	{
		public MapiException(LID lid, string message, ErrorCodeValue errorCode) : base(lid, errorCode, message)
		{
		}

		public MapiException(LID lid, string message, ErrorCodeValue errorCode, Exception innerException) : base(lid, errorCode, message, innerException)
		{
		}
	}
}
