using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	internal class ExchangeTransportConfigContainer : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeTransportConfigContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeTransportConfigContainer.mostDerivedClass;
			}
		}

		private static ExchangeTransportConfigContainerSchema schema = ObjectSchema.GetInstance<ExchangeTransportConfigContainerSchema>();

		private static string mostDerivedClass = "msExchExchangeTransportCfgContainer";
	}
}
