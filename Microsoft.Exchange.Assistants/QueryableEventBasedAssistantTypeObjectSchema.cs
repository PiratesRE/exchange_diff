using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventBasedAssistantTypeObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition AssistantGuid = QueryableObjectSchema.AssistantGuid;

		public static readonly SimpleProviderPropertyDefinition AssistantName = QueryableObjectSchema.AssistantName;

		public static readonly SimpleProviderPropertyDefinition MailboxType = QueryableObjectSchema.MailboxType;

		public static readonly SimpleProviderPropertyDefinition MapiEventType = QueryableObjectSchema.MapiEventType;

		public static readonly SimpleProviderPropertyDefinition NeedMailboxSession = QueryableObjectSchema.NeedMailboxSession;
	}
}
