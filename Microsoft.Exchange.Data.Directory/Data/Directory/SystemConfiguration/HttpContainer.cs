using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class HttpContainer : Container
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return HttpContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HttpContainer.mostDerivedClass;
			}
		}

		private static HttpContainerSchema schema = ObjectSchema.GetInstance<HttpContainerSchema>();

		private static string mostDerivedClass = "msExchProtocolCfgHTTPContainer";
	}
}
