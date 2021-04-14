using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal abstract class LegacyMoveSubscriptionHandlerBase : LegacyMrsSubscriptionHandlerBase
	{
		protected LegacyMoveSubscriptionHandlerBase(IMigrationDataProvider dataProvider, MigrationJob migrationJob) : base(dataProvider, migrationJob, MoveSubscriptionArbiter.Instance)
		{
		}

		public override bool SupportsAdvancedValidation
		{
			get
			{
				return true;
			}
		}

		protected override MigrationUserStatus PostTestStatus
		{
			get
			{
				return MigrationUserStatus.Queued;
			}
		}

		public override bool CreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			return base.InternalCreate(jobItem, false);
		}

		public override bool TestCreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			return base.InternalCreate(jobItem, true);
		}
	}
}
