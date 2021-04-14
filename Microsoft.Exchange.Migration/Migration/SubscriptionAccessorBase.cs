using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionAccessorBase : ISubscriptionAccessor
	{
		public SubscriptionAccessorBase()
		{
		}

		public bool IncludeReport { get; set; }

		public static SubscriptionAccessorBase CreateAccessor(IMigrationDataProvider dataProvider, MigrationType migrationType, string jobName, bool isPAW, bool legacyManualSyncs = false)
		{
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				switch (migrationType)
				{
				case MigrationType.IMAP:
					if (isPAW)
					{
						return new MRSImapSyncRequestAccessor(dataProvider, jobName);
					}
					return new SyncSubscriptionRunspaceAccessor(dataProvider);
				case MigrationType.XO1:
					return new MRSXO1SyncRequestAccessor(dataProvider, jobName);
				case MigrationType.IMAP | MigrationType.XO1:
					goto IL_78;
				case MigrationType.ExchangeOutlookAnywhere:
					return new MRSMergeRequestAccessor(dataProvider, jobName, legacyManualSyncs);
				default:
					if (migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_78;
					}
					break;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove)
			{
				if (migrationType == MigrationType.PSTImport)
				{
					return new PSTImportAccessor(dataProvider, jobName);
				}
				if (migrationType != MigrationType.PublicFolder)
				{
					goto IL_78;
				}
				return new MrsPublicFolderAccessor(dataProvider, jobName);
			}
			return new MrsMoveRequestAccessor(dataProvider, jobName, legacyManualSyncs);
			IL_78:
			throw new ArgumentException("No accessor defined for protocol " + migrationType);
		}

		public static SubscriptionAccessorBase CreateAccessor(IMigrationDataProvider dataProvider, MigrationType type, bool isPAW)
		{
			return SubscriptionAccessorBase.CreateAccessor(dataProvider, type, null, isPAW, false);
		}

		public abstract SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem);

		public abstract SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem);

		public abstract SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId);

		public abstract SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId);

		public abstract bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription);

		public abstract bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false);

		public abstract bool SuspendSubscription(ISubscriptionId subscriptionId);

		public abstract bool RemoveSubscription(ISubscriptionId subscriptionId);

		public virtual bool IsSnapshotCompatible(SubscriptionSnapshot subscriptionSnapshot, MigrationJobItem migrationJobItem)
		{
			return false;
		}
	}
}
