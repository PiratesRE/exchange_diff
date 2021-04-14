using System;
using System.Net;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.MigrationService;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationNotificationRpcStub : IMigrationNotification
	{
		internal MigrationNotificationRpcStub(string mailboxServer) : this(mailboxServer, MigrationNotificationRpcStub.LocalSystemCredential)
		{
		}

		internal MigrationNotificationRpcStub(string mailboxServer, NetworkCredential credential)
		{
			this.mailboxServer = mailboxServer;
			this.credential = credential;
		}

		public UpdateMigrationRequestResult UpdateMigrationRequest(UpdateMigrationRequestArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.SubscriptionStatusChanged;
			MdbefPropertyCollection mdbefPropertyCollection = args.Marshal();
			mdbefPropertyCollection[2684420099U] = (int)methodCode;
			byte[] inputArgBytes = mdbefPropertyCollection.GetBytes();
			UpdateMigrationRequestResult result = null;
			MigrationRpcHelper.InvokeRpcOperation(delegate
			{
				using (IMigrationNotificationRpc migrationNotificationClient = this.GetMigrationNotificationClient(this.mailboxServer, this.credential))
				{
					byte[] array = migrationNotificationClient.UpdateMigrationRequest(1, inputArgBytes);
					MdbefPropertyCollection args2 = MdbefPropertyCollection.Create(array, 0, array.Length);
					result = new UpdateMigrationRequestResult(args2, methodCode);
				}
			});
			return result;
		}

		public RegisterMigrationBatchResult RegisterMigrationBatch(RegisterMigrationBatchArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.RegisterMigrationBatch;
			MdbefPropertyCollection mdbefPropertyCollection = args.Marshal();
			mdbefPropertyCollection[2684420099U] = (int)methodCode;
			byte[] inputArgBytes = mdbefPropertyCollection.GetBytes();
			RegisterMigrationBatchResult result = null;
			MigrationRpcHelper.InvokeRpcOperation(delegate
			{
				using (IMigrationNotificationRpc migrationNotificationClient = this.GetMigrationNotificationClient(this.mailboxServer, null))
				{
					byte[] array = migrationNotificationClient.UpdateMigrationRequest(1, inputArgBytes);
					MdbefPropertyCollection args2 = MdbefPropertyCollection.Create(array, 0, array.Length);
					result = new RegisterMigrationBatchResult(args2, methodCode);
				}
			});
			return result;
		}

		private IMigrationNotificationRpc GetMigrationNotificationClient(string mailboxServer, NetworkCredential credential)
		{
			return new MigrationNotificationRpcClient(this.mailboxServer, credential);
		}

		private const int CurrentVersion = 1;

		private static readonly NetworkCredential LocalSystemCredential = new NetworkCredential(Environment.MachineName + "$", string.Empty, string.Empty);

		private string mailboxServer;

		private NetworkCredential credential;
	}
}
