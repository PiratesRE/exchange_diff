using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum InboxRuleMessageType
	{
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeAutomaticReply)]
		AutomaticReply,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeAutomaticForward)]
		AutomaticForward,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeEncrypted)]
		Encrypted,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeCalendaring)]
		Calendaring,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeCalendaringResponse)]
		CalendaringResponse,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypePermissionControlled)]
		PermissionControlled,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeVoicemail)]
		Voicemail,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeSigned)]
		Signed,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeApprovalRequest)]
		ApprovalRequest,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeReadReceipt)]
		ReadReceipt,
		[LocDescription(ServerStrings.IDs.InboxRuleMessageTypeNonDeliveryReport)]
		NonDeliveryReport
	}
}
