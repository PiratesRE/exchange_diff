using System;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public enum MessageType
	{
		[LocDescription(RulesTasksStrings.IDs.MessageTypeOof)]
		OOF,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeAutoForward)]
		AutoForward,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeEncrypted)]
		Encrypted,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeCalendaring)]
		Calendaring,
		[LocDescription(RulesTasksStrings.IDs.MessageTypePermissionControlled)]
		PermissionControlled,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeVoicemail)]
		Voicemail,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeSigned)]
		Signed,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeApprovalRequest)]
		ApprovalRequest,
		[LocDescription(RulesTasksStrings.IDs.MessageTypeReadReceipt)]
		ReadReceipt
	}
}
