using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class DirectoryPermanentErrorException : StoreException
	{
		public DirectoryPermanentErrorException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.AdUnavailable, message, innerException)
		{
		}
	}
}
