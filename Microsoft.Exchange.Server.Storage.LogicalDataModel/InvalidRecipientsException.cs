using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class InvalidRecipientsException : StoreException
	{
		public InvalidRecipientsException(LID lid, string message) : base(lid, ErrorCodeValue.InvalidRecipients, message)
		{
		}

		public InvalidRecipientsException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.InvalidRecipients, message, innerException)
		{
		}
	}
}
