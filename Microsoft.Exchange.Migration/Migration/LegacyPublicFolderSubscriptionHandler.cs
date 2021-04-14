using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyPublicFolderSubscriptionHandler : LegacyMrsSubscriptionHandlerBase
	{
		public LegacyPublicFolderSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob migrationJob) : base(dataProvider, migrationJob, MoveSubscriptionArbiter.Instance)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LegacyPublicFolderSubscriptionHandler>(this);
		}

		public override bool SupportsAdvancedValidation
		{
			get
			{
				return true;
			}
		}

		public override MigrationType SupportedMigrationType
		{
			get
			{
				return MigrationType.PublicFolder;
			}
		}

		public override bool CreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			base.InternalCreate(jobItem, false);
			return true;
		}

		public override bool TestCreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			base.InternalCreate(jobItem, true);
			return true;
		}

		protected override MigrationUserStatus PostTestStatus
		{
			get
			{
				return MigrationUserStatus.Queued;
			}
		}
	}
}
