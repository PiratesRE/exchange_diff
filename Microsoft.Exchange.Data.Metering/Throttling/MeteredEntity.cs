using System;

namespace Microsoft.Exchange.Data.Metering.Throttling
{
	internal enum MeteredEntity
	{
		Total,
		AnonymousSender,
		Sender,
		SenderMessageSize,
		SenderSubject,
		SenderDomain,
		SenderDomainSubject,
		SenderDomainRecipient,
		Recipient,
		RecipientSubject,
		RecipientDomain,
		RecipientDomainSubject,
		SenderRecipient,
		SenderRecipientMessageSize,
		Tenant,
		IPAddress,
		Queue,
		AccountForest,
		Priority
	}
}
