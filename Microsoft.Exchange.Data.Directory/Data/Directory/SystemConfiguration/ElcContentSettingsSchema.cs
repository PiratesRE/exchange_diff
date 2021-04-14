using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ElcContentSettingsSchema : ADConfigurationObjectSchema
	{
		internal static readonly ADPropertyDefinition MessageClassString = new ADPropertyDefinition("MessageClassString", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchELCMessageClass", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MessageClassArray = new ADPropertyDefinition("MessageClassArray", ExchangeObjectVersion.Exchange2007, typeof(string), "ExtensionName", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageClass = new ADPropertyDefinition("MessageClass", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1023)
		}, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.MessageClassString,
			ElcContentSettingsSchema.MessageClassArray
		}, null, new GetterDelegate(ElcContentSettings.ELCMessageClassGetter), new SetterDelegate(ElcContentSettings.ELCMessageClassSetter), null, null);

		public static readonly ADPropertyDefinition ElcFlags = new ADPropertyDefinition("ELCFlags", ExchangeObjectVersion.Exchange2007, typeof(ElcContentSettingFlags), "msExchELCFlags", ADPropertyDefinitionFlags.None, ElcContentSettingFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetentionAction = new ADPropertyDefinition("RetentionAction", ExchangeObjectVersion.Exchange2007, typeof(RetentionActionType), "msExchELCExpiryAction", ADPropertyDefinitionFlags.None, RetentionActionType.MoveToDeletedItems, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RetentionActionType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AgeLimitForRetention = new ADPropertyDefinition("AgeLimitForRetention", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan?), "msExchELCExpiryAgeLimit", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(86400.0), EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MoveToDestinationFolder = new ADPropertyDefinition("MoveToDestinationFolder", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchELCExpiryDestinationLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AddressForJournaling = new ADPropertyDefinition("AddressForJournaling", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchELCAutoCopyAddressLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LabelForJournaling = new ADPropertyDefinition("LabelForJournaling", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchELCLabel", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		}, null, null);

		public static readonly ADPropertyDefinition RetentionEnabled = new ADPropertyDefinition("RetentionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.ElcFlags
		}, null, (IPropertyBag propertyBag) => ElcContentSettings.GetValueFromFlags(propertyBag, ElcContentSettingFlags.RetentionEnabled), delegate(object value, IPropertyBag propertyBag)
		{
			ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.RetentionEnabled, (bool)value);
		}, null, null);

		public static readonly ADPropertyDefinition TriggerForRetention = new ADPropertyDefinition("TriggerForRetention", ExchangeObjectVersion.Exchange2007, typeof(RetentionDateType), null, ADPropertyDefinitionFlags.Calculated, RetentionDateType.WhenDelivered, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.ElcFlags
		}, null, delegate(IPropertyBag propertyBag)
		{
			bool valueFromFlags = ElcContentSettings.GetValueFromFlags(propertyBag, ElcContentSettingFlags.MoveDateBasedRetention);
			if (valueFromFlags)
			{
				return RetentionDateType.WhenMoved;
			}
			return RetentionDateType.WhenDelivered;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			RetentionDateType retentionDateType = (RetentionDateType)value;
			if (retentionDateType == RetentionDateType.WhenMoved)
			{
				ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.MoveDateBasedRetention, true);
				return;
			}
			ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.MoveDateBasedRetention, false);
		}, null, null);

		public static readonly ADPropertyDefinition JournalingEnabled = new ADPropertyDefinition("JournalingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.ElcFlags
		}, null, (IPropertyBag propertyBag) => ElcContentSettings.GetValueFromFlags(propertyBag, ElcContentSettingFlags.JournalingEnabled), delegate(object value, IPropertyBag propertyBag)
		{
			ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.JournalingEnabled, (bool)value);
		}, null, null);

		public static readonly ADPropertyDefinition MessageClassDisplayName = new ADPropertyDefinition("MessageClassDisplayName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.MessageClassString,
			ElcContentSettingsSchema.MessageClassArray
		}, null, new GetterDelegate(ElcContentSettings.MessageClassDisplayNameGetter), null, null, null);

		public static readonly ADPropertyDefinition ManagedFolder = new ADPropertyDefinition("ManagedFolder", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ElcContentSettings.ELCFolderGetter), null, null, null);

		public static readonly ADPropertyDefinition ManagedFolderName = new ADPropertyDefinition("ManagedFolderName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ElcContentSettings.ELCFolderNameGetter), null, null, null);

		public static readonly ADPropertyDefinition MessageFormatForJournaling = new ADPropertyDefinition("MessageFormatForJournaling", ExchangeObjectVersion.Exchange2007, typeof(JournalingFormat), null, ADPropertyDefinitionFlags.Calculated, JournalingFormat.UseTnef, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ElcContentSettingsSchema.ElcFlags
		}, null, delegate(IPropertyBag propertyBag)
		{
			bool valueFromFlags = ElcContentSettings.GetValueFromFlags(propertyBag, ElcContentSettingFlags.JournalAsMSG);
			if (valueFromFlags)
			{
				return JournalingFormat.UseMsg;
			}
			return JournalingFormat.UseTnef;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			if ((JournalingFormat)value == JournalingFormat.UseMsg)
			{
				ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.JournalAsMSG, true);
				return;
			}
			ElcContentSettings.SetFlags(propertyBag, ElcContentSettingFlags.JournalAsMSG, false);
		}, null, null);
	}
}
