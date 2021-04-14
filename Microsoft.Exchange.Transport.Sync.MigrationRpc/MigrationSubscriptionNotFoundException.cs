using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationSubscriptionNotFoundException : MigrationServiceRpcException
	{
		internal MigrationSubscriptionNotFoundException(MigrationServiceRpcResultCode resultCode, string message) : base(resultCode, message)
		{
		}
	}
}
