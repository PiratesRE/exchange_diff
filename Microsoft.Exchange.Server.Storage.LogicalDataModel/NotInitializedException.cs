using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class NotInitializedException : StoreException
	{
		public NotInitializedException(LID lid, string message) : base(lid, ErrorCodeValue.NotInitialized, message)
		{
		}

		public NotInitializedException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.NotInitialized, message, innerException)
		{
		}
	}
}
