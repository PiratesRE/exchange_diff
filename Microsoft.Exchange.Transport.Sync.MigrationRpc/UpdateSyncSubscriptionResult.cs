using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateSyncSubscriptionResult : MigrationServiceRpcResult
	{
		internal UpdateSyncSubscriptionResult(MdbefPropertyCollection args, MigrationServiceRpcMethodCode expectedMethodCode) : base(args)
		{
			base.ThrowIfVerifyFails(expectedMethodCode);
		}

		internal UpdateSyncSubscriptionResult(MigrationServiceRpcMethodCode methodCode) : base(methodCode)
		{
		}

		internal UpdateSyncSubscriptionResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails) : base(methodCode, resultCode, errorDetails)
		{
		}

		protected override void WriteTo(MdbefPropertyCollection collection)
		{
		}
	}
}
