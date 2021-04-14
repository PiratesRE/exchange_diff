using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ConfigurationContainer : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ConfigurationContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ConfigurationContainer.mostDerivedClass;
			}
		}

		internal override QueryFilter VersioningFilter
		{
			get
			{
				return null;
			}
		}

		private static ConfigurationContainerSchema schema = ObjectSchema.GetInstance<ConfigurationContainerSchema>();

		private static string mostDerivedClass = "configuration";
	}
}
