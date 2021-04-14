using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobItemSummary
	{
		public bool IsPAW
		{
			get
			{
				return this.Version >= 4L;
			}
		}

		public long Version { get; set; }

		public long ItemsSynced { get; set; }

		public long ItemsSkipped { get; set; }

		public MigrationUserRecipientType RecipientType { get; set; }

		public Guid? JobItemGuid { get; private set; }

		public Guid? BatchGuid { get; private set; }

		public Guid? MailboxGuid { get; set; }

		public string MailboxLegacyDN { get; set; }

		public string Identifier { get; set; }

		public string LocalMailboxIdentifier { get; set; }

		public Guid? MrsId { get; set; }

		public ExDateTime? LastSuccessfulSyncTime { get; set; }

		public ExDateTime? LastSubscriptionCheckTime { get; set; }

		public MigrationUserStatus? Status { get; set; }

		public static MigrationJobItemSummary LoadFromRow(object[] propertyValues)
		{
			if (propertyValues == null)
			{
				return null;
			}
			MigrationJobItemSummary migrationJobItemSummary = new MigrationJobItemSummary();
			for (int i = 0; i < MigrationUser.PropertyDefinitions.Length; i++)
			{
				if (propertyValues[i] != null && !(propertyValues[i] is PropertyError))
				{
					if (MigrationUser.PropertyDefinitions[i] == StoreObjectSchema.ItemClass && !string.Equals((string)propertyValues[i], MigrationBatchMessageSchema.MigrationJobItemClass))
					{
						return null;
					}
					if (MigrationUser.PropertyDefinitions[i] == MigrationUser.BatchIdPropertyDefinition)
					{
						migrationJobItemSummary.BatchGuid = (Guid?)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.IdPropertyDefinition)
					{
						migrationJobItemSummary.JobItemGuid = (Guid?)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.IdentifierPropertyDefinition)
					{
						migrationJobItemSummary.Identifier = (string)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.LocalMailboxIdentifierPropertyDefinition)
					{
						migrationJobItemSummary.LocalMailboxIdentifier = (string)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.RecipientTypePropertyDefinition)
					{
						migrationJobItemSummary.RecipientType = (MigrationUserRecipientType)((int)propertyValues[i]);
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.ItemsSkippedPropertyDefinition)
					{
						migrationJobItemSummary.ItemsSkipped = (long)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.ItemsSyncedPropertyDefinition)
					{
						migrationJobItemSummary.ItemsSynced = (long)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.MailboxGuidPropertyDefinition)
					{
						migrationJobItemSummary.MailboxGuid = new Guid?((Guid)propertyValues[i]);
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.MailboxLegacyDNPropertyDefinition)
					{
						migrationJobItemSummary.MailboxLegacyDN = (string)propertyValues[i];
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.MRSIdPropertyDefinition)
					{
						migrationJobItemSummary.MrsId = new Guid?((Guid)propertyValues[i]);
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.LastSuccessfulSyncTimePropertyDefinition)
					{
						ExDateTime? validExDateTime = MigrationHelperBase.GetValidExDateTime((ExDateTime?)propertyValues[i]);
						if (validExDateTime != null)
						{
							migrationJobItemSummary.LastSuccessfulSyncTime = new ExDateTime?(validExDateTime.Value);
						}
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.StatusPropertyDefinition)
					{
						migrationJobItemSummary.Status = new MigrationUserStatus?((MigrationUserStatus)((int)propertyValues[i]));
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.SubscriptionLastCheckedPropertyDefinition)
					{
						ExDateTime? validExDateTime2 = MigrationHelperBase.GetValidExDateTime((ExDateTime?)propertyValues[i]);
						if (validExDateTime2 != null)
						{
							migrationJobItemSummary.LastSubscriptionCheckTime = new ExDateTime?(validExDateTime2.Value);
						}
					}
					else if (MigrationUser.PropertyDefinitions[i] == MigrationUser.VersionPropertyDefinition)
					{
						migrationJobItemSummary.Version = (long)propertyValues[i];
					}
				}
			}
			return migrationJobItemSummary;
		}
	}
}
