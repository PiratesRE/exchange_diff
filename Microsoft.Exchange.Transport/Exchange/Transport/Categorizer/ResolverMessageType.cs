using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal enum ResolverMessageType
	{
		Note,
		LegacyOOF,
		InternalOOF,
		ExternalOOF,
		RN,
		NRN,
		DR,
		NDR,
		DelayedDSN,
		RelayedDSN,
		ExpandedDSN,
		Recall,
		RecallReport,
		AutoReply,
		MeetingForwardNotification,
		UM
	}
}
