using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class LegacyMailboxDatabaseSchema : LegacyDatabaseSchema
	{
		public static readonly ADPropertyDefinition JournalRecipient = SharedPropertyDefinitions.JournalRecipient;

		public static readonly ADPropertyDefinition MailboxRetention = new ADPropertyDefinition("MailboxRetention", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "msExchMailboxRetentionPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(2592000.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OfflineAddressBook = SharedPropertyDefinitions.OfflineAddressBook;

		public static readonly ADPropertyDefinition OriginalDatabase = SharedPropertyDefinitions.OriginalDatabase;

		public static readonly ADPropertyDefinition PublicFolderDatabase = SharedPropertyDefinitions.MailboxPublicFolderDatabase;

		public static readonly ADPropertyDefinition ProhibitSendReceiveQuota = new ADPropertyDefinition("ProhibitSendReceiveQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBOverHardQuotaLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Recovery = new ADPropertyDefinition("Recovery", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchRestore", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProhibitSendQuota = new ADPropertyDefinition("ProhibitSendQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBOverQuotaLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IndexEnabled = new ADPropertyDefinition("IndexEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchCIAvailable", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public new static readonly ADPropertyDefinition Name = LegacyDatabaseSchema.Name;
	}
}
