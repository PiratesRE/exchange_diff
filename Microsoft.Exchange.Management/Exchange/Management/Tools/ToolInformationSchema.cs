using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tools
{
	internal class ToolInformationSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(ToolId), PropertyDefinitionFlags.None, ToolId.CSVParser, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition VersionStatus = new SimpleProviderPropertyDefinition("VersionStatus", ExchangeObjectVersion.Exchange2010, typeof(ToolVersionStatus), PropertyDefinitionFlags.None, ToolVersionStatus.LatestVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MinimumSupportedVersion = new SimpleProviderPropertyDefinition("MinimumSupportedVersion", ExchangeObjectVersion.Exchange2010, typeof(Version), PropertyDefinitionFlags.None, SupportedToolsData.MinimumVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LatestVersion = new SimpleProviderPropertyDefinition("LatestVersion", ExchangeObjectVersion.Exchange2010, typeof(Version), PropertyDefinitionFlags.None, SupportedToolsData.MaximumVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UpdateInformationUrl = new SimpleProviderPropertyDefinition("UpdateInformationUrl", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
