using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventControllerObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ShutdownState = QueryableObjectSchema.ShutdownState;

		public static readonly SimpleProviderPropertyDefinition TimeToSaveWatermarks = QueryableObjectSchema.TimeToSaveWatermarks;

		public static readonly SimpleProviderPropertyDefinition HighestEventPolled = QueryableObjectSchema.HighestEventPolled;

		public static readonly SimpleProviderPropertyDefinition NumberEventsInQueueCurrent = QueryableObjectSchema.NumberEventsInQueueCurrent;

		public static readonly SimpleProviderPropertyDefinition EventFilter = QueryableObjectSchema.EventFilter;

		public static readonly SimpleProviderPropertyDefinition RestartRequired = QueryableObjectSchema.RestartRequired;

		public static readonly SimpleProviderPropertyDefinition TimeToUpdateIdleWatermarks = QueryableObjectSchema.TimeToUpdateIdleWatermarks;

		public static readonly SimpleProviderPropertyDefinition ActiveMailboxes = QueryableObjectSchema.ActiveMailboxes;

		public static readonly SimpleProviderPropertyDefinition UpToDateMailboxes = QueryableObjectSchema.UpToDateMailboxes;

		public static readonly SimpleProviderPropertyDefinition DeadMailboxes = QueryableObjectSchema.DeadMailboxes;

		public static readonly SimpleProviderPropertyDefinition RecoveryEventDispatcheres = QueryableObjectSchema.RecoveryEventDispatcheres;

		public static readonly SimpleProviderPropertyDefinition Governor = QueryableObjectSchema.Governor;
	}
}
