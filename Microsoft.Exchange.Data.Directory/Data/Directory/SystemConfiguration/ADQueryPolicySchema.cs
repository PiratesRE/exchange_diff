using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADQueryPolicySchema : ADNonExchangeObjectSchema
	{
		private const string MaxNotificationPerConnStr = "MaxNotificationPerConn";

		public new static readonly ADPropertyDefinition ExchangeVersion = new ADPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), null, ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.DoNotProvisionalClone, ExchangeObjectVersion.Exchange2003, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LDAPAdminLimits = new ADPropertyDefinition("lDAPAdminLimits", ExchangeObjectVersion.Exchange2003, typeof(LdapPolicy), "lDAPAdminLimits", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxNotificationPerConn = new ADPropertyDefinition("MaxNotificationPerConn", ExchangeObjectVersion.Exchange2003, typeof(int?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(5, int.MaxValue)
		}, new ProviderPropertyDefinition[]
		{
			ADQueryPolicySchema.LDAPAdminLimits
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<LdapPolicy> multiValuedProperty = (MultiValuedProperty<LdapPolicy>)propertyBag[ADQueryPolicySchema.LDAPAdminLimits];
			LdapPolicy ldapPolicy = multiValuedProperty.Find((LdapPolicy x) => x.PolicyName.Equals("MaxNotificationPerConn", StringComparison.OrdinalIgnoreCase));
			if (ldapPolicy == null)
			{
				return null;
			}
			return ldapPolicy.Value;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			int value2 = (int)value;
			MultiValuedProperty<LdapPolicy> multiValuedProperty = (MultiValuedProperty<LdapPolicy>)propertyBag[ADQueryPolicySchema.LDAPAdminLimits];
			LdapPolicy ldapPolicy = multiValuedProperty.Find((LdapPolicy x) => x.PolicyName.Equals("MaxNotificationPerConn", StringComparison.OrdinalIgnoreCase));
			if (ldapPolicy != null)
			{
				multiValuedProperty.Remove(ldapPolicy);
			}
			multiValuedProperty.Add(new LdapPolicy("MaxNotificationPerConn", value2));
		}, null, null);
	}
}
