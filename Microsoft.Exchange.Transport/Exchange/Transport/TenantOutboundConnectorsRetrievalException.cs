using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport
{
	internal class TenantOutboundConnectorsRetrievalException : Exception
	{
		public TenantOutboundConnectorsRetrievalException(ADOperationResult result)
		{
			this.Result = result;
		}

		public ADOperationResult Result { get; private set; }
	}
}
