using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADPasswordSettingsSchema : ADNonExchangeObjectSchema
	{
		public new static readonly ADPropertyDefinition SystemFlags = new ADPropertyDefinition("SystemFlags", ExchangeObjectVersion.Exchange2003, typeof(SystemFlagsEnum), "systemFlags", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, SystemFlagsEnum.Unrenameable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AppliesTo = new ADPropertyDefinition("AppliesTo", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msDS-PSOAppliesTo", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordSettingsPrecedence = new ADPropertyDefinition("PasswordSettingsPrecedence", ExchangeObjectVersion.Exchange2003, typeof(int), "msDS-PasswordSettingsPrecedence", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 20, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordHistoryLength = new ADPropertyDefinition("PasswordHistoryLength", ExchangeObjectVersion.Exchange2003, typeof(int), "msDS-PasswordHistoryLength", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordComplexityEnabled = new ADPropertyDefinition("PasswordComplexityEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msDS-PasswordComplexityEnabled", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordReversibleEncryptionEnabled = new ADPropertyDefinition("PasswordReversibleEncryptionEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msDS-PasswordReversibleEncryptionEnabled", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockoutObservationWindow = new ADPropertyDefinition("LockoutObservationWindow", ExchangeObjectVersion.Exchange2003, typeof(long), "msDS-LockoutObservationWindow", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, -18000000000L, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<long>(long.MinValue, 0L)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockoutDuration = new ADPropertyDefinition("LockoutDuration", ExchangeObjectVersion.Exchange2003, typeof(long), "msDS-LockoutDuration", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, -18000000000L, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<long>(long.MinValue, 0L)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LockoutThreshold = new ADPropertyDefinition("LockoutThreshold", ExchangeObjectVersion.Exchange2003, typeof(int), "msDS-LockoutThreshold", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinimumPasswordAge = new ADPropertyDefinition("MinimumPasswordAge", ExchangeObjectVersion.Exchange2003, typeof(long), "msDS-MinimumPasswordAge", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<long>(long.MinValue, 0L)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumPasswordAge = new ADPropertyDefinition("MaximumPasswordAge", ExchangeObjectVersion.Exchange2003, typeof(long), "msDS-MaximumPasswordAge", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, long.MinValue, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<long>(long.MinValue, 0L)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinimumPasswordLength = new ADPropertyDefinition("MinimumPasswordLength", ExchangeObjectVersion.Exchange2003, typeof(int), "msDS-MinimumPasswordLength", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 255)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
