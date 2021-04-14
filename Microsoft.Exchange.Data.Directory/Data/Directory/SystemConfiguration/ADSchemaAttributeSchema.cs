using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADSchemaAttributeSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition SchemaIDGuid = ADSchemaObjectSchema.SchemaIDGuid;

		public static readonly ADPropertyDefinition RangeUpper = new ADPropertyDefinition("RangeUpper", ExchangeObjectVersion.Exchange2003, typeof(int?), "rangeUpper", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RangeLower = new ADPropertyDefinition("RangeLower", ExchangeObjectVersion.Exchange2003, typeof(int?), "rangeLower", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MapiID = new ADPropertyDefinition("MapiID", ExchangeObjectVersion.Exchange2003, typeof(int), "mAPIID", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LinkID = new ADPropertyDefinition("LinkID", ExchangeObjectVersion.Exchange2003, typeof(int), "linkID", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LdapDisplayName = new ADPropertyDefinition("LdapDisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "lDAPDisplayName", ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RawAttributeSyntax = new ADPropertyDefinition("RawAttributeSyntax", ExchangeObjectVersion.Exchange2003, typeof(string), "AttributeSyntax", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OMSyntax = new ADPropertyDefinition("OMSyntax", ExchangeObjectVersion.Exchange2003, typeof(AttributeSyntax), "oMSyntax", ADPropertyDefinitionFlags.Mandatory, AttributeSyntax.Boolean, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OMObjectClass = new ADPropertyDefinition("OMObjectClass", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "OMObjectClass", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsMemberOfPartialAttributeSet = new ADPropertyDefinition("IsMemberOfPartialAttributeSet", ExchangeObjectVersion.Exchange2003, typeof(bool), "isMemberOfPartialAttributeSet", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsSingleValued = new ADPropertyDefinition("IsSingleValued", ExchangeObjectVersion.Exchange2003, typeof(bool), "isSingleValued", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttributeID = new ADPropertyDefinition("AttributeID", ExchangeObjectVersion.Exchange2003, typeof(string), "attributeID", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DataSyntax = new ADPropertyDefinition("DataSyntax", ExchangeObjectVersion.Exchange2003, typeof(DataSyntax), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.DataSyntax.UnDefined, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADSchemaAttributeSchema.RawAttributeSyntax,
			ADSchemaAttributeSchema.OMSyntax,
			ADSchemaAttributeSchema.OMObjectClass
		}, null, new GetterDelegate(ADSchemaAttributeObject.SyntaxGetter), null, null, null);
	}
}
