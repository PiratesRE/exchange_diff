using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SmtpVirtualServerConfigurationSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition SmtpFqdn = new ADPropertyDefinition("SmtpFqdn", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSmtpFullyQualifiedDomainName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
