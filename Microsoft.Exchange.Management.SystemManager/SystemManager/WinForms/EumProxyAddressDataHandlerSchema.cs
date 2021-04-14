using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EumProxyAddressDataHandlerSchema
	{
		public static int FixedLength = "phone-context=".Length + ";".Length + ProxyAddressPrefix.UM.DisplayName.Length + 1;

		public static readonly ADPropertyDefinition Extension = new ADPropertyDefinition("Extension", ExchangeObjectVersion.Exchange2003, typeof(string), "Extension", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, ProxyAddressBase.MaxLength - EumProxyAddressDataHandlerSchema.FixedLength)
		}, null, null);
	}
}
