using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiGetNewDsaRpcArgs : MigrationNspiRpcArgs
	{
		public MigrationNspiGetNewDsaRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint) : base(endpoint, MigrationProxyRpcType.GetNewDSA)
		{
			base.RpcHostServer = endpoint.ExchangeServer;
		}

		public MigrationNspiGetNewDsaRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.GetNewDSA)
		{
		}
	}
}
