using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ServersContainerSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition ContainerInfo = new ADPropertyDefinition("ContainerInfo", ExchangeObjectVersion.Exchange2003, typeof(ContainerInfo), "containerInfo", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.ContainerInfo.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
