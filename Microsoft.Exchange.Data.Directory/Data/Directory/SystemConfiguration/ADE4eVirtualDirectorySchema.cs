using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADE4eVirtualDirectorySchema : ExchangeWebAppVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition E4EConfigurationXML = new ADPropertyDefinition("E4EConfigurationXML", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchConfigurationXML", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
