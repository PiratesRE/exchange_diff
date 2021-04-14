using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ProvisioningUpdateStepHandler : ProvisioningStepHandlerBase
	{
		public ProvisioningUpdateStepHandler(IMigrationDataProvider dataProvider) : base(dataProvider)
		{
		}

		public override MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null)
		{
			MigrationUserStatus? migrationUserStatus = MigrationJobItem.ResolveFlagStatus(flags);
			if (migrationUserStatus != null)
			{
				return migrationUserStatus.Value;
			}
			return MigrationUserStatus.ProvisionUpdating;
		}

		protected override IProvisioningData GetProvisioningData(MigrationJobItem jobItem)
		{
			MigrationType migrationType = jobItem.MigrationType;
			if (migrationType == MigrationType.ExchangeOutlookAnywhere)
			{
				return ExchangeProvisioningDataFactory.GetProvisioningUpdateData(base.DataProvider, jobItem.MigrationJob, jobItem);
			}
			return null;
		}

		protected override ProvisioningType GetProvisioningType(MigrationJobItem jobItem)
		{
			switch (jobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				return ProvisioningType.UserUpdate;
			case MigrationUserRecipientType.Contact:
				return ProvisioningType.ContactUpdate;
			case MigrationUserRecipientType.Group:
				return ProvisioningType.GroupMember;
			case MigrationUserRecipientType.Mailuser:
				return ProvisioningType.MailEnabledUserUpdate;
			}
			throw new NotSupportedException("recipient type not supported!");
		}
	}
}
