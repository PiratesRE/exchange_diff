using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableThrottleGovernorObjectSchema : QueryableGovernorObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Throttle = QueryableObjectSchema.Throttle;
	}
}
