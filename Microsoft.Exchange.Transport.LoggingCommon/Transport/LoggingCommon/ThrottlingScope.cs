using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum ThrottlingScope
	{
		All,
		Tenant,
		Sender,
		Recipient,
		SenderRecipient,
		SenderRecipientSubject,
		SenderRecipientSubjectPart,
		MBServer,
		MDB,
		AccountForest
	}
}
