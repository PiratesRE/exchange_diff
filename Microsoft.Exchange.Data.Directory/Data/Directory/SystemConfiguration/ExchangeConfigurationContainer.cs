using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ExchangeConfigurationContainer : ADContainer
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchConfigurationContainer";
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeConfigurationContainer.schema;
			}
		}

		private const string MostDerivedObjectClassInternal = "msExchConfigurationContainer";

		private static ADContainerSchema schema = ObjectSchema.GetInstance<ADContainerSchema>();
	}
}
