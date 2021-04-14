using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal abstract class MessageTraceVisitorBase : IMessageTraceVisitor
	{
		public virtual void Visit(MessageTraceEntityBase entity)
		{
		}

		public virtual void Visit(MessageTrace messageTrace)
		{
		}

		public virtual void Visit(MessageClientInformation messageTrace)
		{
		}

		public virtual void Visit(MessageClientInformationProperty messageTrace)
		{
		}

		public virtual void Visit(MessageProperty messageProperty)
		{
		}

		public virtual void Visit(MessageEvent messageEvent)
		{
		}

		public virtual void Visit(MessageEventProperty messageEventProperty)
		{
		}

		public virtual void Visit(MessageEventRule messageEventRule)
		{
		}

		public virtual void Visit(MessageEventRuleProperty messageEventRuleProperty)
		{
		}

		public virtual void Visit(MessageEventRuleClassification messageEventRuleClassification)
		{
		}

		public virtual void Visit(MessageEventRuleClassificationProperty messageEventRuleClassificationProperty)
		{
		}

		public virtual void Visit(MessageEventSourceItem messageEventSourceItem)
		{
		}

		public virtual void Visit(MessageEventSourceItemProperty messageEventSourceItemProperty)
		{
		}

		public virtual void Visit(MessageClassification messageClassification)
		{
		}

		public virtual void Visit(MessageClassificationProperty messageClassificationProperty)
		{
		}

		public virtual void Visit(MessageRecipient messageRecipient)
		{
		}

		public virtual void Visit(MessageRecipientProperty messageRecipientProperty)
		{
		}

		public virtual void Visit(MessageRecipientStatus recipientStatus)
		{
		}

		public virtual void Visit(MessageRecipientStatusProperty recipientStatusProperty)
		{
		}

		public virtual void Visit(MessageAction messageAction)
		{
		}

		public virtual void Visit(MessageActionProperty messageActionProperty)
		{
		}
	}
}
