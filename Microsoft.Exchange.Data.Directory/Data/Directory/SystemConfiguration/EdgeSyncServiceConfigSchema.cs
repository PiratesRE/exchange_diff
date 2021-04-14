using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class EdgeSyncServiceConfigSchema : ADContainerSchema
	{
		private const string DefaultLogPath = "TransportRoles\\Logs\\EdgeSync\\";

		public static readonly ADPropertyDefinition ConfigurationSyncInterval = new ADPropertyDefinition("ConfigurationSyncInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncConfigurationSyncInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(3.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientSyncInterval = new ADPropertyDefinition("RecipientSyncInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncRecipientSyncInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockDuration = new ADPropertyDefinition("LockDuration", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncLockDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(6.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockRenewalDuration = new ADPropertyDefinition("LockRenewalDuration", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncLockRenewalDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(4.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OptionDuration = new ADPropertyDefinition("OptionDuration", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncOptionDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CookieValidDuration = new ADPropertyDefinition("CookieValidDuration", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncCookieValidDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(21.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FailoverDCInterval = new ADPropertyDefinition("FailoverDCInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncFailoverDCInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneMinute, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneMinute)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogEnabled = new ADPropertyDefinition("LogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchEdgeSyncLogEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogMaxAge = new ADPropertyDefinition("LogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEdgeSyncLogMaxAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogMaxDirectorySize = new ADPropertyDefinition("LogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchEdgeSyncLogMaxDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogMaxFileSize = new ADPropertyDefinition("LogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchEdgeSyncLogMaxFileSize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogLevel = new ADPropertyDefinition("LogLevel", ExchangeObjectVersion.Exchange2007, typeof(EdgeSyncLoggingLevel), "msExchEdgeSyncLogLevel", ADPropertyDefinitionFlags.None, EdgeSyncLoggingLevel.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(EdgeSyncLoggingLevel))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogPath = new ADPropertyDefinition("LogPath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncLogPath", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, "TransportRoles\\Logs\\EdgeSync\\", PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
