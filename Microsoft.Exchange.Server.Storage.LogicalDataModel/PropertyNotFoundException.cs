using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class PropertyNotFoundException : StoreException
	{
		public PropertyNotFoundException(LID lid, string message) : base(lid, ErrorCodeValue.NotFound, message)
		{
		}

		public PropertyNotFoundException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.NotFound, message, innerException)
		{
		}
	}
}
