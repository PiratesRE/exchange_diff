using System;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADClientAccessRuleSchema : ADConfigurationObjectSchema
	{
		internal static ClientAccessRule GetRuleFromADProperties(IPropertyBag propertyBag)
		{
			if (string.IsNullOrEmpty((string)propertyBag[ADClientAccessRuleSchema.Xml]))
			{
				return new ClientAccessRule((string)propertyBag[ADObjectSchema.RawName]);
			}
			return ClientAccessRule.FromADProperties((string)propertyBag[ADClientAccessRuleSchema.Xml], null, (string)propertyBag[ADObjectSchema.RawName], (int)propertyBag[ADClientAccessRuleSchema.InternalPriority], (bool)propertyBag[ADClientAccessRuleSchema.EnabledBit], (bool)propertyBag[ADClientAccessRuleSchema.DatacenterAdminsOnlyBit], true);
		}

		internal static ADPropertyDefinition GetClientAccessRulePropertyDefinition<PropertyType, SingleType>(string name, PropertyType defaultValue, Func<ClientAccessRule, PropertyType> getter, Action<ClientAccessRule, PropertyType, IPropertyBag> setter, ADPropertyDefinitionFlags flags, PropertyDefinitionConstraint[] writeConstraints)
		{
			return new ADPropertyDefinition(name, ExchangeObjectVersion.Exchange2012, typeof(SingleType), null, flags, defaultValue, PropertyDefinitionConstraint.None, writeConstraints, new ProviderPropertyDefinition[]
			{
				ADClientAccessRuleSchema.InternalPriority,
				ADClientAccessRuleSchema.RuleFlags,
				ADObjectSchema.RawName,
				ADClientAccessRuleSchema.Xml
			}, null, (IPropertyBag propertyBag) => getter(ADClientAccessRuleSchema.GetRuleFromADProperties(propertyBag)), delegate(object value, IPropertyBag propertyBag)
			{
				ClientAccessRule ruleFromADProperties = ADClientAccessRuleSchema.GetRuleFromADProperties(propertyBag);
				setter(ruleFromADProperties, (PropertyType)((object)value), propertyBag);
				propertyBag[ADClientAccessRuleSchema.Xml] = ruleFromADProperties.Xml;
			}, null, null);
		}

		private const int FLAGS_ENABLED_BIT = 0;

		private const int FLAGS_DATACENTER_ONLY_BIT = 1;

		public static readonly ADPropertyDefinition InternalPriority = new ADPropertyDefinition("InternalPriority", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchTransportRulePriority", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RuleFlags = new ADPropertyDefinition("RuleFlags", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchProvisioningFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Xml = new ADPropertyDefinition("RuleXml", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchTransportRuleXml", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EnabledBit = ADObject.BitfieldProperty("EnabledBit", 0, ADClientAccessRuleSchema.RuleFlags);

		public static readonly ADPropertyDefinition DatacenterAdminsOnlyBit = ADObject.BitfieldProperty("DatacenterAdminsOnlyBit", 1, ADClientAccessRuleSchema.RuleFlags);

		public static readonly ADPropertyDefinition RuleName = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<string, string>("RuleName", string.Empty, (ClientAccessRule rule) => rule.Name, delegate(ClientAccessRule rule, string name, IPropertyBag propertyBag)
		{
			rule.Name = name;
		}, ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition Enabled = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<bool, bool>("Enabled", true, (ClientAccessRule rule) => rule.Enabled == RuleState.Enabled, delegate(ClientAccessRule rule, bool enabled, IPropertyBag propertyBag)
		{
			propertyBag[ADClientAccessRuleSchema.EnabledBit] = enabled;
			rule.Enabled = (enabled ? RuleState.Enabled : RuleState.Disabled);
		}, ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition DatacenterAdminsOnly = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<bool, bool>("DatacenterAdminsOnly", false, (ClientAccessRule rule) => rule.DatacenterAdminsOnly, delegate(ClientAccessRule rule, bool datacenterAdminsOnly, IPropertyBag propertyBag)
		{
			propertyBag[ADClientAccessRuleSchema.DatacenterAdminsOnlyBit] = datacenterAdminsOnly;
			rule.DatacenterAdminsOnly = datacenterAdminsOnly;
		}, ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition Action = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<ClientAccessRulesAction, ClientAccessRulesAction>("Action", ClientAccessRulesAction.AllowAccess, (ClientAccessRule rule) => rule.Action, delegate(ClientAccessRule rule, ClientAccessRulesAction action, IPropertyBag propertyBag)
		{
			rule.Action = action;
		}, ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition AnyOfClientIPAddressesOrRanges = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<IPRange>, IPRange>("AnyOfClientIPAddressesOrRanges", null, (ClientAccessRule rule) => rule.AnyOfClientIPAddressesOrRanges, delegate(ClientAccessRule rule, MultiValuedProperty<IPRange> ranges, IPropertyBag propertyBag)
		{
			rule.AnyOfClientIPAddressesOrRanges = ranges.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition ExceptAnyOfClientIPAddressesOrRanges = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<IPRange>, IPRange>("ExceptAnyOfClientIPAddressesOrRanges", null, (ClientAccessRule rule) => rule.ExceptAnyOfClientIPAddressesOrRanges, delegate(ClientAccessRule rule, MultiValuedProperty<IPRange> ranges, IPropertyBag propertyBag)
		{
			rule.ExceptAnyOfClientIPAddressesOrRanges = ranges.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition AnyOfSourceTcpPortNumbers = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<IntRange>, IntRange>("AnyOfSourceTcpPortNumbers", null, (ClientAccessRule rule) => rule.AnyOfSourceTcpPortNumbers, delegate(ClientAccessRule rule, MultiValuedProperty<IntRange> ranges, IPropertyBag propertyBag)
		{
			rule.AnyOfSourceTcpPortNumbers = ranges.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, new PropertyDefinitionConstraint[]
		{
			new IntRangeConstraint(1, 65535)
		});

		public static readonly ADPropertyDefinition ExceptAnyOfSourceTcpPortNumbers = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<IntRange>, IntRange>("ExceptAnyOfSourceTcpPortNumbers", null, (ClientAccessRule rule) => rule.ExceptAnyOfSourceTcpPortNumbers, delegate(ClientAccessRule rule, MultiValuedProperty<IntRange> ranges, IPropertyBag propertyBag)
		{
			rule.ExceptAnyOfSourceTcpPortNumbers = ranges.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, new PropertyDefinitionConstraint[]
		{
			new IntRangeConstraint(1, 65535)
		});

		public static readonly ADPropertyDefinition UsernameMatchesAnyOfPatterns = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<string>, string>("UsernameMatchesAnyOfPatterns", null, (ClientAccessRule rule) => rule.UsernameMatchesAnyOfPatterns, delegate(ClientAccessRule rule, MultiValuedProperty<string> patterns, IPropertyBag propertyBag)
		{
			rule.UsernameMatchesAnyOfPatterns = patterns.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition ExceptUsernameMatchesAnyOfPatterns = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<string>, string>("ExceptUsernameMatchesAnyOfPatterns", null, (ClientAccessRule rule) => rule.ExceptUsernameMatchesAnyOfPatterns, delegate(ClientAccessRule rule, MultiValuedProperty<string> patterns, IPropertyBag propertyBag)
		{
			rule.ExceptUsernameMatchesAnyOfPatterns = patterns.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition UserIsMemberOf = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<string>, string>("UserIsMemberOf", null, (ClientAccessRule rule) => rule.UserIsMemberOf, delegate(ClientAccessRule rule, MultiValuedProperty<string> groups, IPropertyBag propertyBag)
		{
			rule.UserIsMemberOf = groups.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition ExceptUserIsMemberOf = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<string>, string>("ExceptUserIsMemberOf", null, (ClientAccessRule rule) => rule.ExceptUserIsMemberOf, delegate(ClientAccessRule rule, MultiValuedProperty<string> groups, IPropertyBag propertyBag)
		{
			rule.ExceptUserIsMemberOf = groups.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition AnyOfAuthenticationTypes = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<ClientAccessAuthenticationMethod>, ClientAccessAuthenticationMethod>("AnyOfAuthenticationTypes", null, (ClientAccessRule rule) => new MultiValuedProperty<ClientAccessAuthenticationMethod>(rule.AnyOfAuthenticationTypes), delegate(ClientAccessRule rule, MultiValuedProperty<ClientAccessAuthenticationMethod> methods, IPropertyBag propertyBag)
		{
			rule.AnyOfAuthenticationTypes = methods.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition ExceptAnyOfAuthenticationTypes = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<ClientAccessAuthenticationMethod>, ClientAccessAuthenticationMethod>("ExceptAnyOfAuthenticationTypes", null, (ClientAccessRule rule) => new MultiValuedProperty<ClientAccessAuthenticationMethod>(rule.ExceptAnyOfAuthenticationTypes), delegate(ClientAccessRule rule, MultiValuedProperty<ClientAccessAuthenticationMethod> methods, IPropertyBag propertyBag)
		{
			rule.ExceptAnyOfAuthenticationTypes = methods.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition AnyOfProtocols = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<ClientAccessProtocol>, ClientAccessProtocol>("AnyOfProtocols", null, (ClientAccessRule rule) => new MultiValuedProperty<ClientAccessProtocol>(rule.AnyOfProtocols), delegate(ClientAccessRule rule, MultiValuedProperty<ClientAccessProtocol> methods, IPropertyBag propertyBag)
		{
			rule.AnyOfProtocols = methods.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition ExceptAnyOfProtocols = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<MultiValuedProperty<ClientAccessProtocol>, ClientAccessProtocol>("ExceptAnyOfProtocols", null, (ClientAccessRule rule) => new MultiValuedProperty<ClientAccessProtocol>(rule.ExceptAnyOfProtocols), delegate(ClientAccessRule rule, MultiValuedProperty<ClientAccessProtocol> methods, IPropertyBag propertyBag)
		{
			rule.ExceptAnyOfProtocols = methods.ToArray();
		}, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition UserRecipientFilter = ADClientAccessRuleSchema.GetClientAccessRulePropertyDefinition<string, string>("UserRecipientFilter", null, (ClientAccessRule rule) => rule.UserRecipientFilter, delegate(ClientAccessRule rule, string filter, IPropertyBag propertyBag)
		{
			rule.UserRecipientFilter = filter;
		}, ADPropertyDefinitionFlags.Calculated, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2012, typeof(int), null, ADPropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
