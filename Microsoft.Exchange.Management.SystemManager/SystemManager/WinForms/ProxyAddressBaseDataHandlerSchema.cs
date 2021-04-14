using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ProxyAddressBaseDataHandlerSchema
	{
		public static readonly ADPropertyDefinition Address = new ADPropertyDefinition("Address", ExchangeObjectVersion.Exchange2003, typeof(string), "address", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, ProxyAddressBase.MaxLength - 2)
		}, null, null);

		public static readonly ADPropertyDefinition Prefix = new ADPropertyDefinition("Prefix", ExchangeObjectVersion.Exchange2003, typeof(string), "prefix", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new AsciiCharactersOnlyConstraint(),
			new StringLengthConstraint(1, 9)
		}, null, null);
	}
}
