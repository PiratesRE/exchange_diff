using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class StoreOperationCanceledException : StoreException
	{
		public StoreOperationCanceledException(LID lid, string message) : base(lid, ErrorCodeValue.Cancel, message)
		{
		}

		public StoreOperationCanceledException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.Cancel, message, innerException)
		{
		}
	}
}
