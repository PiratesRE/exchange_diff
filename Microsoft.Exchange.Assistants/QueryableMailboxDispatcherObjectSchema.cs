using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableMailboxDispatcherObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition MailboxGuid = QueryableObjectSchema.MailboxGuid;

		public static readonly SimpleProviderPropertyDefinition DecayedEventCounter = QueryableObjectSchema.DecayedEventCounter;

		public static readonly SimpleProviderPropertyDefinition NumberOfActiveDispatchers = QueryableObjectSchema.NumberOfActiveDispatchers;

		public static readonly SimpleProviderPropertyDefinition IsMailboxDead = QueryableObjectSchema.IsMailboxDead;

		public static readonly SimpleProviderPropertyDefinition IsIdle = QueryableObjectSchema.IsIdle;
	}
}
