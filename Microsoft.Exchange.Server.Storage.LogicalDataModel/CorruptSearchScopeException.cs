using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class CorruptSearchScopeException : StoreException
	{
		public CorruptSearchScopeException(LID lid, string message) : base(lid, ErrorCodeValue.CorruptSearchScope, message)
		{
		}

		public CorruptSearchScopeException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CorruptSearchScope, message, innerException)
		{
		}
	}
}
