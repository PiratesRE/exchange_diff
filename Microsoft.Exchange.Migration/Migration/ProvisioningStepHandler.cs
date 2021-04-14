using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ProvisioningStepHandler : ProvisioningStepHandlerBase
	{
		public ProvisioningStepHandler(IMigrationDataProvider dataProvider) : base(dataProvider)
		{
		}

		public override MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null)
		{
			MigrationUserStatus? migrationUserStatus = MigrationJobItem.ResolveFlagStatus(flags);
			if (migrationUserStatus != null)
			{
				return migrationUserStatus.Value;
			}
			return MigrationUserStatus.Provisioning;
		}

		protected override IProvisioningData GetProvisioningData(MigrationJobItem jobItem)
		{
			switch (jobItem.MigrationType)
			{
			case MigrationType.XO1:
				return XO1ProvisioningDataFactory.GetProvisioningData(jobItem.LocalMailboxIdentifier, jobItem.ProvisioningData);
			case MigrationType.ExchangeOutlookAnywhere:
				return ExchangeProvisioningDataFactory.GetProvisioningData(base.DataProvider, jobItem.MigrationJob, jobItem);
			}
			return null;
		}

		protected override ProvisioningType GetProvisioningType(MigrationJobItem jobItem)
		{
			switch (jobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				return ProvisioningType.User;
			case MigrationUserRecipientType.Contact:
				return ProvisioningType.Contact;
			case MigrationUserRecipientType.Group:
				return ProvisioningType.Group;
			case MigrationUserRecipientType.Mailuser:
				return ProvisioningType.MailEnabledUser;
			}
			throw new NotSupportedException("recipient type not supported!");
		}
	}
}
