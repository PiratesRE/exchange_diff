using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CorruptDataException : StoreException
	{
		public CorruptDataException(LID lid, string message) : base(lid, ErrorCodeValue.CorruptData, message)
		{
		}

		public CorruptDataException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CorruptData, message, innerException)
		{
		}
	}
}
