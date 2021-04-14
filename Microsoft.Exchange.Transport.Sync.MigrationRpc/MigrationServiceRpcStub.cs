using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.MigrationService;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationServiceRpcStub : IMigrationService
	{
		internal MigrationServiceRpcStub(string mailboxServer)
		{
			this.mailboxServer = mailboxServer;
		}

		public CreateSyncSubscriptionResult CreateSyncSubscription(AbstractCreateSyncSubscriptionArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.CreateSyncSubscription;
			MdbefPropertyCollection mdbefPropertyCollection = args.Marshal();
			mdbefPropertyCollection[2684420099U] = (int)methodCode;
			byte[] inputArgsBytes = mdbefPropertyCollection.GetBytes();
			CreateSyncSubscriptionResult result = null;
			MigrationRpcHelper.InvokeRpcOperation(delegate
			{
				using (IMigrationServiceRpc migrationServiceClient = this.GetMigrationServiceClient(this.mailboxServer))
				{
					int version = 2;
					byte[] array = migrationServiceClient.InvokeMigrationServiceEndPoint(version, inputArgsBytes);
					MdbefPropertyCollection args2 = MdbefPropertyCollection.Create(array, 0, array.Length);
					result = new CreateSyncSubscriptionResult(args2, methodCode);
				}
			});
			return result;
		}

		public UpdateSyncSubscriptionResult UpdateSyncSubscription(UpdateSyncSubscriptionArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.UpdateSyncSubscription;
			MdbefPropertyCollection mdbefPropertyCollection = args.Marshal();
			mdbefPropertyCollection[2684420099U] = (int)methodCode;
			byte[] inputArgsBytes = mdbefPropertyCollection.GetBytes();
			UpdateSyncSubscriptionResult result = null;
			MigrationRpcHelper.InvokeRpcOperation(delegate
			{
				using (IMigrationServiceRpc migrationServiceClient = this.GetMigrationServiceClient(this.mailboxServer))
				{
					byte[] array = migrationServiceClient.InvokeMigrationServiceEndPoint(1, inputArgsBytes);
					MdbefPropertyCollection args2 = MdbefPropertyCollection.Create(array, 0, array.Length);
					result = new UpdateSyncSubscriptionResult(args2, methodCode);
				}
			});
			return result;
		}

		public GetSyncSubscriptionStateResult GetSyncSubscriptionState(GetSyncSubscriptionStateArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.GetSyncSubscriptionState;
			MdbefPropertyCollection mdbefPropertyCollection = args.Marshal();
			mdbefPropertyCollection[2684420099U] = (int)methodCode;
			byte[] inputArgBytes = mdbefPropertyCollection.GetBytes();
			GetSyncSubscriptionStateResult result = null;
			MigrationRpcHelper.InvokeRpcOperation(delegate
			{
				using (IMigrationServiceRpc migrationServiceClient = this.GetMigrationServiceClient(this.mailboxServer))
				{
					byte[] array = migrationServiceClient.InvokeMigrationServiceEndPoint(1, inputArgBytes);
					MdbefPropertyCollection args2 = MdbefPropertyCollection.Create(array, 0, array.Length);
					result = new GetSyncSubscriptionStateResult(args2, methodCode);
				}
			});
			return result;
		}

		private IMigrationServiceRpc GetMigrationServiceClient(string mailboxServer)
		{
			return new MigrationServiceRpcClient(this.mailboxServer);
		}

		internal const int CurrentVersion = 2;

		internal const int R4Version = 1;

		private string mailboxServer;
	}
}
