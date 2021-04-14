using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventDispatcherObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition AssistantName = QueryableObjectSchema.AssistantName;

		public static readonly SimpleProviderPropertyDefinition AssistantGuid = QueryableObjectSchema.AssistantGuid;

		public static readonly SimpleProviderPropertyDefinition CommittedWatermark = QueryableObjectSchema.CommittedWatermark;

		public static readonly SimpleProviderPropertyDefinition HighestEventQueued = QueryableObjectSchema.HighestEventQueued;

		public static readonly SimpleProviderPropertyDefinition RecoveryEventCounter = QueryableObjectSchema.RecoveryEventCounter;

		public static readonly SimpleProviderPropertyDefinition IsInRetry = QueryableObjectSchema.IsInRetry;

		public static readonly SimpleProviderPropertyDefinition PendingQueueLength = QueryableObjectSchema.PendingQueueLength;

		public static readonly SimpleProviderPropertyDefinition ActiveQueueLength = QueryableObjectSchema.ActiveQueueLength;

		public static readonly SimpleProviderPropertyDefinition PendingWorkers = QueryableObjectSchema.PendingWorkers;

		public static readonly SimpleProviderPropertyDefinition ActiveWorkers = QueryableObjectSchema.ActiveWorkers;
	}
}
