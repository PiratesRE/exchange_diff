using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RegisterMigrationBatchResult : MigrationServiceRpcResult
	{
		internal RegisterMigrationBatchResult(MdbefPropertyCollection args, MigrationServiceRpcMethodCode expectedMethodCode) : base(args)
		{
			base.ThrowIfVerifyFails(expectedMethodCode);
		}

		internal RegisterMigrationBatchResult(MigrationServiceRpcMethodCode methodCode) : base(methodCode)
		{
		}

		internal RegisterMigrationBatchResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails) : base(methodCode, resultCode, errorDetails)
		{
		}

		protected override void WriteTo(MdbefPropertyCollection collection)
		{
		}
	}
}
