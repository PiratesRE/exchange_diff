using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class InterceptorRuleSchema : ADConfigurationObjectSchema
	{
		private static void ExpireTimeUtcSetter(object value, IPropertyBag propertybag)
		{
			DateTime dateTime = (DateTime)value;
			propertybag[InterceptorRuleSchema.ExpireTime] = dateTime.Ticks;
		}

		private static object ExpireTimeUtcGetter(IPropertyBag propertyBag)
		{
			long ticks = (long)propertyBag[InterceptorRuleSchema.ExpireTime];
			return new DateTime(ticks, DateTimeKind.Utc);
		}

		public static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("RulePriority", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportRulePriority", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Xml = new ADPropertyDefinition("RuleXml", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportRuleXml", ADPropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ImmutableId = new ADPropertyDefinition("ImmutableId", ExchangeObjectVersion.Exchange2007, typeof(Guid), "wWWHomePage", ADPropertyDefinitionFlags.WriteOnce, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Version = new ADPropertyDefinition("RuleVersion", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchTransportRuleVersion", ADPropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Target = new ADPropertyDefinition("RuleTarget", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchTransportRuleTargetLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExpireTime = new ADPropertyDefinition("RuleExpireTime", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchTransportRuleExpireTime", ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExpireTimeUtc = new ADPropertyDefinition("ExpireTimeUtc", ExchangeObjectVersion.Exchange2010, typeof(DateTime), null, ADPropertyDefinitionFlags.Calculated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			InterceptorRuleSchema.ExpireTime
		}, null, new GetterDelegate(InterceptorRuleSchema.ExpireTimeUtcGetter), new SetterDelegate(InterceptorRuleSchema.ExpireTimeUtcSetter), null, null);
	}
}
