using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableThrottleObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ThrottleName = QueryableObjectSchema.ThrottleName;

		public static readonly SimpleProviderPropertyDefinition CurrentThrottle = QueryableObjectSchema.CurrentThrottle;

		public static readonly SimpleProviderPropertyDefinition ActiveWorkItems = QueryableObjectSchema.ActiveWorkItems;

		public static readonly SimpleProviderPropertyDefinition OverThrottle = QueryableObjectSchema.OverThrottle;

		public static readonly SimpleProviderPropertyDefinition PendingWorkItemsOnBase = QueryableObjectSchema.PendingWorkItemsOnBase;

		public static readonly SimpleProviderPropertyDefinition QueueLength = QueryableObjectSchema.QueueLength;
	}
}
