using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationUserSchema : SimpleProviderObjectSchema
	{
		public new static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(MigrationUserId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition BatchId = new SimpleProviderPropertyDefinition("BatchId", ExchangeObjectVersion.Exchange2010, typeof(MigrationBatchId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition EmailAddress = new SimpleProviderPropertyDefinition("EmailAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition RecipientType = new SimpleProviderPropertyDefinition("RecipientType", ExchangeObjectVersion.Exchange2010, typeof(MigrationUserRecipientType), PropertyDefinitionFlags.PersistDefaultValue, MigrationUserRecipientType.Mailbox, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SkippedItemCount = new SimpleProviderPropertyDefinition("SkippedItemCount", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SyncedItemCount = new SimpleProviderPropertyDefinition("SyncedItemCount", ExchangeObjectVersion.Exchange2010, typeof(long), PropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MailboxGuid = new SimpleProviderPropertyDefinition("MailboxGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MailboxLegacyDN = new SimpleProviderPropertyDefinition("MailboxLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MRSId = new SimpleProviderPropertyDefinition("MRSId", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSuccessfulSyncTime = new SimpleProviderPropertyDefinition("LastSuccessfulSyncTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(MigrationUserStatus), PropertyDefinitionFlags.PersistDefaultValue, MigrationUserStatus.Corrupted, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StatusSummary = new SimpleProviderPropertyDefinition("StatusSummary", ExchangeObjectVersion.Exchange2010, typeof(MigrationUserStatusSummary), PropertyDefinitionFlags.PersistDefaultValue, MigrationUserStatusSummary.Failed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SubscriptionLastChecked = new SimpleProviderPropertyDefinition("SubscriptionLastChecked", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
