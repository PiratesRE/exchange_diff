using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SmimeConfigurationContainerSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition SmimeConfigurationXML = XMLSerializableBase.ConfigurationXmlRawProperty();
	}
}
