using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class InvalidParameterException : StoreException
	{
		public InvalidParameterException(LID lid, string message) : base(lid, ErrorCodeValue.InvalidParameter, message)
		{
		}

		public InvalidParameterException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.InvalidParameter, message, innerException)
		{
		}
	}
}
