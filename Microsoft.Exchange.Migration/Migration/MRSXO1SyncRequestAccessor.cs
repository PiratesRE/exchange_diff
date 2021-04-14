using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSXO1SyncRequestAccessor : MRSSyncRequestAccessorBase
	{
		public MRSXO1SyncRequestAccessor(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider, batchName)
		{
		}

		public override bool IsSnapshotCompatible(SubscriptionSnapshot subscriptionSnapshot, MigrationJobItem migrationJobItem)
		{
			if (!base.IsSnapshotCompatible(subscriptionSnapshot, migrationJobItem))
			{
				return false;
			}
			SyncSubscriptionSnapshot syncSubscriptionSnapshot = subscriptionSnapshot as SyncSubscriptionSnapshot;
			return syncSubscriptionSnapshot != null && syncSubscriptionSnapshot.Protocol == SyncProtocol.Olc;
		}

		protected override void ApplySubscriptionSettings(NewSyncRequestCommandBase command, string identifier, IMailboxData localMailbox, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings)
		{
			NewSyncRequestCommand newSyncRequestCommand = command as NewSyncRequestCommand;
			if (newSyncRequestCommand != null)
			{
				newSyncRequestCommand.Mailbox = localMailbox.GetIdParameter<MailboxIdParameter>();
				newSyncRequestCommand.Olc = true;
				newSyncRequestCommand.WorkloadType = RequestWorkloadType.Onboarding;
				newSyncRequestCommand.SkipMerging = "InitialConnectionValidation";
			}
		}
	}
}
