using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class NestedSearchChainTooDeepException : StoreException
	{
		public NestedSearchChainTooDeepException(LID lid, string message) : base(lid, ErrorCodeValue.NestedSearchChainTooDeep, message)
		{
		}

		public NestedSearchChainTooDeepException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.NestedSearchChainTooDeep, message, innerException)
		{
		}
	}
}
