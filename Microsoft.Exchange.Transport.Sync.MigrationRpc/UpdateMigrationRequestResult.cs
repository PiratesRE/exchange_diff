using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpdateMigrationRequestResult : MigrationServiceRpcResult
	{
		internal UpdateMigrationRequestResult(MdbefPropertyCollection args, MigrationServiceRpcMethodCode expectedMethodCode) : base(args)
		{
			object obj;
			if (args.TryGetValue(2936274947U, out obj) && obj is int)
			{
				this.responseCode = (SubscriptionStatusChangedResponse)((int)obj);
			}
			else
			{
				this.responseCode = SubscriptionStatusChangedResponse.OK;
			}
			base.ThrowIfVerifyFails(expectedMethodCode);
		}

		internal UpdateMigrationRequestResult(MigrationServiceRpcMethodCode methodCode, SubscriptionStatusChangedResponse responseCode) : base(methodCode)
		{
			this.responseCode = responseCode;
		}

		internal UpdateMigrationRequestResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails) : base(methodCode, resultCode, errorDetails)
		{
			this.responseCode = SubscriptionStatusChangedResponse.OK;
		}

		internal bool IsActionRequired()
		{
			return (this.responseCode & SubscriptionStatusChangedResponse.ActionRequired) != (SubscriptionStatusChangedResponse)0;
		}

		internal bool IsDeleteRequested()
		{
			return this.responseCode == SubscriptionStatusChangedResponse.Delete;
		}

		internal bool IsDisableRequested()
		{
			return this.responseCode == SubscriptionStatusChangedResponse.Disable;
		}

		protected override void WriteTo(MdbefPropertyCollection collection)
		{
			collection[2936274947U] = (int)this.responseCode;
		}

		private readonly SubscriptionStatusChangedResponse responseCode;
	}
}
