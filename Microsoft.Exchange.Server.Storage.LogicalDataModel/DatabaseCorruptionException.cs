using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class DatabaseCorruptionException : StoreException
	{
		public DatabaseCorruptionException(LID lid, string message) : base(lid, ErrorCodeValue.CallFailed, message)
		{
		}

		public DatabaseCorruptionException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CallFailed, message, innerException)
		{
		}
	}
}
