using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public enum NullableInboxRuleMessageType
	{
		NullInboxRuleMessageType = -1,
		AutomaticReply,
		AutomaticForward,
		Encrypted,
		Calendaring,
		CalendaringResponse,
		PermissionControlled,
		Voicemail,
		Signed,
		ApprovalRequest,
		ReadReceipt,
		NonDeliveryReport
	}
}
