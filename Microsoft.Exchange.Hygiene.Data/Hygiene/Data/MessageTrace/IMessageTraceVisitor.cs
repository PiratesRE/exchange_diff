using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal interface IMessageTraceVisitor
	{
		void Visit(MessageTraceEntityBase entity);

		void Visit(MessageTrace messageTrace);

		void Visit(MessageProperty messageProperty);

		void Visit(MessageEvent messageEvent);

		void Visit(MessageEventProperty messageEventProperty);

		void Visit(MessageEventRule messageEventRule);

		void Visit(MessageEventRuleProperty messageEventRuleProperty);

		void Visit(MessageEventRuleClassification messageEventRuleClassification);

		void Visit(MessageEventRuleClassificationProperty messageEventRuleClassificationProperty);

		void Visit(MessageEventSourceItem messageEventSourceItem);

		void Visit(MessageEventSourceItemProperty messageEventSourceItemProperty);

		void Visit(MessageClassification messageClassification);

		void Visit(MessageClassificationProperty messageClassificationProperty);

		void Visit(MessageClientInformation messageClientInformation);

		void Visit(MessageClientInformationProperty messageClientInformationProperty);

		void Visit(MessageRecipient messageRecipient);

		void Visit(MessageRecipientProperty messageRecipientProperty);

		void Visit(MessageRecipientStatus recipientStatus);

		void Visit(MessageRecipientStatusProperty recipientStatusProperty);

		void Visit(MessageAction messageAction);

		void Visit(MessageActionProperty messageActionProperty);
	}
}
