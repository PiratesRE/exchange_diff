using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Search
{
	internal class SearchDocumentFormatInfoSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition DocumentClass = new SimpleProviderPropertyDefinition("DocumentClass", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Enabled = new SimpleProviderPropertyDefinition("Enabled", ExchangeObjectVersion.Current, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Extension = new SimpleProviderPropertyDefinition("Extension", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition FormatHandler = new SimpleProviderPropertyDefinition("FormatHandler", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition IsBindUserDefined = new SimpleProviderPropertyDefinition("IsBindUserDefined", ExchangeObjectVersion.Current, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition IsFormatUserDefined = new SimpleProviderPropertyDefinition("IsFormatUserDefined", ExchangeObjectVersion.Current, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition MimeType = new SimpleProviderPropertyDefinition("MimeType", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
