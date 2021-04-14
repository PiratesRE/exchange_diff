using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADServerSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition DnsHostName = new ADPropertyDefinition("DnsHostName", ExchangeObjectVersion.Exchange2003, typeof(string), "dnsHostName", ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Sid = new ADPropertyDefinition("Sid", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "objectSid", ADPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerReference = new ADPropertyDefinition("ServerReference", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "serverReference", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
