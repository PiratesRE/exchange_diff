using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class PopImapConditionalHandlerSchema : ConditionalHandlerSchema
	{
		public static readonly SimpleProviderPropertyDefinition RequestId = ConditionalHandlerSchema.BuildStringPropDef("RequestId");

		public static readonly SimpleProviderPropertyDefinition Parameters = ConditionalHandlerSchema.BuildStringPropDef("Parameters");

		public static readonly SimpleProviderPropertyDefinition Response = ConditionalHandlerSchema.BuildValueTypePropDef<int>("Response");

		public static readonly SimpleProviderPropertyDefinition ResponseType = ConditionalHandlerSchema.BuildValueTypePropDef<int>("ResponseType");

		public static readonly SimpleProviderPropertyDefinition LightLogContext = ConditionalHandlerSchema.BuildStringPropDef("LightLogContext");

		public static readonly SimpleProviderPropertyDefinition Message = ConditionalHandlerSchema.BuildStringPropDef("Message");

		public static readonly SimpleProviderPropertyDefinition Traces = ConditionalHandlerSchema.BuildStringPropDef("Traces");
	}
}
