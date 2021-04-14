using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableGovernorObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Status = QueryableObjectSchema.Status;

		public static readonly SimpleProviderPropertyDefinition LastRunTime = QueryableObjectSchema.LastRunTime;

		public static readonly SimpleProviderPropertyDefinition NumberConsecutiveFailures = QueryableObjectSchema.NumberConsecutiveFailures;
	}
}
