using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ForwardSyncCookieHeaderSchema : ADConfigurationObjectSchema
	{
		protected const int CookieTypeBitPosition = 0;

		protected const int CookieTypeBitLength = 4;

		protected const int IsSyncPropertySetUpgradingBitPosition = 4;

		internal static readonly ADPropertyDefinition TimestampRaw = new ADPropertyDefinition("TimestampRaw", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchMSOForwardSyncCookieTimestamp", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition Timestamp = new ADPropertyDefinition("Timestamp", ExchangeObjectVersion.Exchange2010, typeof(DateTime), null, ADPropertyDefinitionFlags.Calculated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ForwardSyncCookieHeaderSchema.TimestampRaw
		}, null, (IPropertyBag bag) => DateTime.FromFileTimeUtc((long)bag[ForwardSyncCookieHeaderSchema.TimestampRaw]), delegate(object value, IPropertyBag bag)
		{
			bag[ForwardSyncCookieHeaderSchema.TimestampRaw] = ((DateTime)value).ToFileTimeUtc();
		}, null, null);

		internal static readonly ADPropertyDefinition Type = ADObject.BitfieldProperty("Type", 0, 4, SharedPropertyDefinitions.ProvisioningFlags);
	}
}
