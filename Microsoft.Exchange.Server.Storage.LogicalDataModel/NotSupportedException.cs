using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class NotSupportedException : StoreException
	{
		public NotSupportedException(LID lid, string message) : base(lid, ErrorCodeValue.NotSupported, message)
		{
		}

		public NotSupportedException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.NotSupported, message, innerException)
		{
		}
	}
}
