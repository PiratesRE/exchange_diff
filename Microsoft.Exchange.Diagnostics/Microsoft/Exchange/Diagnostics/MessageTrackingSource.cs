using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal enum MessageTrackingSource
	{
		SMTP,
		PICKUP,
		DSN,
		AGENT,
		ADMIN,
		STOREDRIVER,
		ROUTING,
		DNS,
		GATEWAY,
		POISONMESSAGE,
		ORAR,
		REDUNDANCY,
		SAFETYNET,
		MAILBOXRULE,
		AGGREGATION,
		APPROVAL,
		MULTITENANCY,
		QUEUE,
		APPLICATION,
		STOREDRIVERSUBMISSION,
		PUBLICFOLDER,
		BOOTLOADER,
		MEETINGMESSAGEPROCESSOR
	}
}
