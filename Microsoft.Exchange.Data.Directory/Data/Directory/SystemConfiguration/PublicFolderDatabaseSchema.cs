using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class PublicFolderDatabaseSchema : DatabaseSchema
	{
		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition FirstInstance = new ADPropertyDefinition("FirstInstance", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchFirstInstance", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HomeMta = ADRecipientSchema.HomeMTA;

		public static readonly ADPropertyDefinition MaxItemSize = new ADPropertyDefinition("MaxItemSize", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "messageSizeLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ItemRetentionPeriod = new ADPropertyDefinition("ItemRetentionPeriod", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<EnhancedTimeSpan>), "msExchOverallAgeLimit", ADPropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationPeriod = new ADPropertyDefinition("ReplicationPeriod", ExchangeObjectVersion.Exchange2003, typeof(uint), "msExchPollInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 15U, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<uint>(1U, 2147483647U)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PoliciesIncluded = ADRecipientSchema.PoliciesIncluded;

		public static readonly ADPropertyDefinition PublicFolderHierarchy = SharedPropertyDefinitions.PublicFolderHierarchy;

		public static readonly ADPropertyDefinition ReplicationMessageSize = new ADPropertyDefinition("ReplicationMessageSize", ExchangeObjectVersion.Exchange2003, typeof(ByteQuantifiedSize), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchReplicationMsgSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(300UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(1UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationMode = new ADPropertyDefinition("ReplicationMode", ExchangeObjectVersion.Exchange2003, typeof(ScheduleMode), "msExchReplicationStyle", ADPropertyDefinitionFlags.PersistDefaultValue, ScheduleMode.Always, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScheduleMode))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationScheduleBitmaps = SharedPropertyDefinitions.ReplicationScheduleBitmaps;

		public static readonly ADPropertyDefinition ProhibitPostQuota = new ADPropertyDefinition("ProhibitPostQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBOverHardQuotaLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseCustomReferralServerList = new ADPropertyDefinition("UseCustomReferralServerList", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CustomReferralServerList = new ADPropertyDefinition("CustomReferralServerList", ExchangeObjectVersion.Exchange2003, typeof(ServerCostPair), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Organizations = new ADPropertyDefinition("Organizations", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDefaultPublicMDBBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationSchedule = new ADPropertyDefinition("ReplicationSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), null, ADPropertyDefinitionFlags.Calculated, Schedule.Always, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PublicFolderDatabaseSchema.ReplicationScheduleBitmaps,
			PublicFolderDatabaseSchema.ReplicationMode
		}, null, new GetterDelegate(PublicFolderDatabase.ReplicationScheduleGetter), new SetterDelegate(PublicFolderDatabase.ReplicationScheduleSetter), null, null);

		public static readonly ADPropertyDefinition WindowsEmailAddress = ADRecipientSchema.WindowsEmailAddress;

		public new static readonly ADPropertyDefinition Name = DatabaseSchema.Name;
	}
}
