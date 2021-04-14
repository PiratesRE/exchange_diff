using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageRecipientSchema
	{
		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition RecipientIdProperty = CommonMessageTraceSchema.RecipientIdProperty;

		internal static readonly HygienePropertyDefinition ToEmailPrefixProperty = new HygienePropertyDefinition("ToEmailPrefix", typeof(string));

		internal static readonly HygienePropertyDefinition ToEmailDomainProperty = new HygienePropertyDefinition("ToEmailDomain", typeof(string));

		internal static readonly HygienePropertyDefinition MailDeliveryStatusProperty = new HygienePropertyDefinition("MailDeliveryStatus", typeof(MailDeliveryStatus));
	}
}
