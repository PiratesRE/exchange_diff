using System;

namespace Microsoft.Exchange.Data
{
	internal class BinaryFileDataObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition FileData = new SimpleProviderPropertyDefinition("FileData", ExchangeObjectVersion.Exchange2003, typeof(byte[]), PropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
