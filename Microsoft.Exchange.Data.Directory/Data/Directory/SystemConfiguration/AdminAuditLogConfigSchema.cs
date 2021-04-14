using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AdminAuditLogConfigSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition AdminLogFlags = new ADPropertyDefinition("AdminAuditLogFlags", ExchangeObjectVersion.Exchange2010, typeof(AdminAuditLogFlags), "msExchAdminAuditLogFlags", ADPropertyDefinitionFlags.None, AdminAuditLogFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogCmdlets = new ADPropertyDefinition("AdminAuditLogCmdlets", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAdminAuditLogCmdlets", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogParameters = new ADPropertyDefinition("AdminAuditLogParameters", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAdminAuditLogParameters", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogExcludedCmdlets = new ADPropertyDefinition("AdminAuditLogExcludedCmdlets", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAdminAuditLogExcludedCmdlets", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogAgeLimit = new ADPropertyDefinition("AdminAuditLogAgeLimit", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchAdminAuditLogAgeLimit", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(90.0), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogMailbox = new ADPropertyDefinition("AdminAuditLogMailbox", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), "msExchAdminAuditLogMailbox", ADPropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition AdminAuditLogEnabled = new ADPropertyDefinition("AdminAuditLogEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AdminAuditLogConfigSchema.AdminLogFlags
		}, null, (IPropertyBag propertyBag) => AdminAuditLogConfig.GetValueFromFlags(propertyBag, AdminAuditLogFlags.AdminAuditLogEnabled), delegate(object value, IPropertyBag propertyBag)
		{
			AdminAuditLogConfig.SetFlags(propertyBag, AdminAuditLogFlags.AdminAuditLogEnabled, (bool)value);
		}, null, null);

		public static readonly ADPropertyDefinition TestCmdletLoggingEnabled = new ADPropertyDefinition("TestCmdletsLoggingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AdminAuditLogConfigSchema.AdminLogFlags
		}, null, (IPropertyBag propertyBag) => AdminAuditLogConfig.GetValueFromFlags(propertyBag, AdminAuditLogFlags.TestCmdletLoggingEnabled), delegate(object value, IPropertyBag propertyBag)
		{
			AdminAuditLogConfig.SetFlags(propertyBag, AdminAuditLogFlags.TestCmdletLoggingEnabled, (bool)value);
		}, null, null);

		public static readonly ADPropertyDefinition CaptureDetailsEnabled = new ADPropertyDefinition("CaptureDetailsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AdminAuditLogConfigSchema.AdminLogFlags
		}, null, (IPropertyBag propertyBag) => AdminAuditLogConfig.GetValueFromFlags(propertyBag, AdminAuditLogFlags.CaptureDetailsEnabled), delegate(object value, IPropertyBag propertyBag)
		{
			AdminAuditLogConfig.SetFlags(propertyBag, AdminAuditLogFlags.CaptureDetailsEnabled, (bool)value);
		}, null, null);
	}
}
