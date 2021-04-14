using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableDatabaseManagerObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition StartState = QueryableObjectSchema.StartState;

		public static readonly SimpleProviderPropertyDefinition IsStopping = QueryableObjectSchema.IsStopping;

		public static readonly SimpleProviderPropertyDefinition Throttle = QueryableObjectSchema.Throttle;

		public static readonly SimpleProviderPropertyDefinition Governor = QueryableObjectSchema.Governor;
	}
}
