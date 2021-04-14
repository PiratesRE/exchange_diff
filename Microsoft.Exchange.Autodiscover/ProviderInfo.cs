using System;
using Microsoft.Exchange.Autodiscover.Providers;

namespace Microsoft.Exchange.Autodiscover
{
	internal class ProviderInfo
	{
		internal ProviderInfo(Type systemType, ProviderAttribute attributes, CreateProviderDelegate createProvider)
		{
			this.systemType = systemType;
			this.attributes = attributes;
			this.createProvider = createProvider;
		}

		internal bool Match(RequestData requestData)
		{
			return requestData.ResponseSchemas.Count > 0 && requestData.ResponseSchemas[0] == this.Attributes.ResponseSchema && requestData.RequestSchemas.Count > 0 && requestData.RequestSchemas[0] == this.Attributes.RequestSchema;
		}

		internal Type SystemType
		{
			get
			{
				return this.systemType;
			}
		}

		internal ProviderAttribute Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		internal CreateProviderDelegate CreateProvider
		{
			get
			{
				return this.createProvider;
			}
		}

		private Type systemType;

		private ProviderAttribute attributes;

		private CreateProviderDelegate createProvider;
	}
}
