using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CannotPreserveMailboxSignature : StoreException
	{
		public CannotPreserveMailboxSignature(LID lid, string message) : base(lid, ErrorCodeValue.CannotPreserveMailboxSignature, message)
		{
		}

		public CannotPreserveMailboxSignature(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CannotPreserveMailboxSignature, message, innerException)
		{
		}
	}
}
