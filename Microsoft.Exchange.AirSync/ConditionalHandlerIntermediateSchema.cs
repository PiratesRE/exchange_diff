using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class ConditionalHandlerIntermediateSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition HandlerStartTime = ConditionalHandlerSchema.BuildValueTypePropDef<ExDateTime>("_HandlerStartTime");

		public static readonly SimpleProviderPropertyDefinition ProxyStartTime = ConditionalHandlerSchema.BuildValueTypePropDef<ExDateTime>("_ProxyStartTime");

		public static readonly SimpleProviderPropertyDefinition PostWlmStartTime = ConditionalHandlerSchema.BuildValueTypePropDef<ExDateTime>("_PostWlmStartTime");
	}
}
