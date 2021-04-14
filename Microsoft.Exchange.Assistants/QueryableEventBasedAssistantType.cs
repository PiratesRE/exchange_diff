using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventBasedAssistantType : QueryableObjectImplBase<QueryableEventBasedAssistantTypeObjectSchema>
	{
		public Guid AssistantGuid
		{
			get
			{
				return (Guid)this[QueryableEventBasedAssistantTypeObjectSchema.AssistantGuid];
			}
			set
			{
				this[QueryableEventBasedAssistantTypeObjectSchema.AssistantGuid] = value;
			}
		}

		public string AssistantName
		{
			get
			{
				return (string)this[QueryableEventBasedAssistantTypeObjectSchema.AssistantName];
			}
			set
			{
				this[QueryableEventBasedAssistantTypeObjectSchema.AssistantName] = value;
			}
		}

		public string MailboxType
		{
			get
			{
				return (string)this[QueryableEventBasedAssistantTypeObjectSchema.MailboxType];
			}
			set
			{
				this[QueryableEventBasedAssistantTypeObjectSchema.MailboxType] = value;
			}
		}

		public string MapiEventType
		{
			get
			{
				return (string)this[QueryableEventBasedAssistantTypeObjectSchema.MapiEventType];
			}
			set
			{
				this[QueryableEventBasedAssistantTypeObjectSchema.MapiEventType] = value;
			}
		}

		public bool NeedMailboxSession
		{
			get
			{
				return (bool)this[QueryableEventBasedAssistantTypeObjectSchema.NeedMailboxSession];
			}
			set
			{
				this[QueryableEventBasedAssistantTypeObjectSchema.NeedMailboxSession] = value;
			}
		}
	}
}
