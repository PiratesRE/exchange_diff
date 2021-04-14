using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateSyncSubscriptionResult : MigrationServiceRpcResult
	{
		internal CreateSyncSubscriptionResult(MdbefPropertyCollection args, MigrationServiceRpcMethodCode expectedMethodCode) : base(args)
		{
			object obj;
			if (args.TryGetValue(2936078594U, out obj))
			{
				byte[] byteArray = (byte[])obj;
				this.subscriptionMessageId = StoreObjectId.Deserialize(byteArray);
			}
			if (args.TryGetValue(2936799304U, out obj))
			{
				this.subscriptionGuid = new Guid?((Guid)obj);
			}
			base.ThrowIfVerifyFails(expectedMethodCode);
		}

		internal CreateSyncSubscriptionResult(MigrationServiceRpcMethodCode methodCode, StoreObjectId subscriptionMessageId, Guid subscriptionGuid) : base(methodCode)
		{
			this.subscriptionMessageId = subscriptionMessageId;
			this.subscriptionGuid = new Guid?(subscriptionGuid);
		}

		internal CreateSyncSubscriptionResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails) : base(methodCode, resultCode, errorDetails)
		{
		}

		internal StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		internal Guid? SubscriptionGuid
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		public override string ToString()
		{
			if (this.subscriptionGuid != null && this.subscriptionGuid.Value != Guid.Empty)
			{
				return this.subscriptionGuid.ToString();
			}
			if (this.subscriptionMessageId != null)
			{
				return this.subscriptionMessageId.ToString();
			}
			return string.Empty;
		}

		protected override void WriteTo(MdbefPropertyCollection collection)
		{
			if (this.SubscriptionMessageId != null)
			{
				collection[2936078594U] = this.SubscriptionMessageId.GetBytes();
			}
			if (this.subscriptionGuid != null && this.subscriptionGuid.Value != Guid.Empty)
			{
				collection[2936799304U] = this.subscriptionGuid;
			}
		}

		private readonly StoreObjectId subscriptionMessageId;

		private readonly Guid? subscriptionGuid;
	}
}
