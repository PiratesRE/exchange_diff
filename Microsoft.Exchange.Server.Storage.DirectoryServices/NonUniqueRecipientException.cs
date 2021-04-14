using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class NonUniqueRecipientException : StoreException
	{
		public NonUniqueRecipientException(LID lid, string message) : base(lid, ErrorCodeValue.ADPropertyError, message)
		{
		}

		public NonUniqueRecipientException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.ADPropertyError, message, innerException)
		{
		}
	}
}
