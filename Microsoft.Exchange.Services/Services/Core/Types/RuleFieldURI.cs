﻿using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleFieldURIType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum RuleFieldURI
	{
		RuleId,
		DisplayName,
		Priority,
		IsNotSupported,
		Actions,
		[XmlEnum("Condition:Categories")]
		ConditionCategories,
		[XmlEnum("Condition:ContainsBodyStrings")]
		ConditionContainsBodyStrings,
		[XmlEnum("Condition:ContainsHeaderStrings")]
		ConditionContainsHeaderStrings,
		[XmlEnum("Condition:ContainsRecipientStrings")]
		ConditionContainsRecipientStrings,
		[XmlEnum("Condition:ContainsSenderStrings")]
		ConditionContainsSenderStrings,
		[XmlEnum("Condition:ContainsSubjectOrBodyStrings")]
		ConditionContainsSubjectOrBodyStrings,
		[XmlEnum("Condition:ContainsSubjectStrings")]
		ConditionContainsSubjectStrings,
		[XmlEnum("Condition:FlaggedForAction")]
		ConditionFlaggedForAction,
		[XmlEnum("Condition:FromAddresses")]
		ConditionFromAddresses,
		[XmlEnum("Condition:FromConnectedAccounts")]
		ConditionFromConnectedAccounts,
		[XmlEnum("Condition:HasAttachments")]
		ConditionHasAttachments,
		[XmlEnum("Condition:Importance")]
		ConditionImportance,
		[XmlEnum("Condition:IsApprovalRequest")]
		ConditionIsApprovalRequest,
		[XmlEnum("Condition:IsAutomaticForward")]
		ConditionIsAutomaticForward,
		[XmlEnum("Condition:IsAutomaticReply")]
		ConditionIsAutomaticReply,
		[XmlEnum("Condition:IsEncrypted")]
		ConditionIsEncrypted,
		[XmlEnum("Condition:IsMeetingRequest")]
		ConditionIsMeetingRequest,
		[XmlEnum("Condition:IsMeetingResponse")]
		ConditionIsMeetingResponse,
		[XmlEnum("Condition:IsNDR")]
		ConditionIsNDR,
		[XmlEnum("Condition:IsPermissionControlled")]
		ConditionIsPermissionControlled,
		[XmlEnum("Condition:IsReadReceipt")]
		ConditionIsReadReceipt,
		[XmlEnum("Condition:IsSigned")]
		ConditionIsSigned,
		[XmlEnum("Condition:IsVoicemail")]
		ConditionIsVoicemail,
		[XmlEnum("Condition:ItemClasses")]
		ConditionItemClasses,
		[XmlEnum("Condition:MessageClassifications")]
		ConditionMessageClassifications,
		[XmlEnum("Condition:NotSentToMe")]
		ConditionNotSentToMe,
		[XmlEnum("Condition:SentCcMe")]
		ConditionSentCcMe,
		[XmlEnum("Condition:SentOnlyToMe")]
		ConditionSentOnlyToMe,
		[XmlEnum("Condition:SentToAddresses")]
		ConditionSentToAddresses,
		[XmlEnum("Condition:SentToMe")]
		ConditionSentToMe,
		[XmlEnum("Condition:SentToOrCcMe")]
		ConditionSentToOrCcMe,
		[XmlEnum("Condition:Sensitivity")]
		ConditionSensitivity,
		[XmlEnum("Condition:WithinDateRange")]
		ConditionWithinDateRange,
		[XmlEnum("Condition:WithinSizeRange")]
		ConditionWithinSizeRange,
		[XmlEnum("Exception:Categories")]
		ExceptionCategories,
		[XmlEnum("Exception:ContainsBodyStrings")]
		ExceptionContainsBodyStrings,
		[XmlEnum("Exception:ContainsHeaderStrings")]
		ExceptionContainsHeaderStrings,
		[XmlEnum("Exception:ContainsRecipientStrings")]
		ExceptionContainsRecipientStrings,
		[XmlEnum("Exception:ContainsSenderStrings")]
		ExceptionContainsSenderStrings,
		[XmlEnum("Exception:ContainsSubjectOrBodyStrings")]
		ExceptionContainsSubjectOrBodyStrings,
		[XmlEnum("Exception:ContainsSubjectStrings")]
		ExceptionContainsSubjectStrings,
		[XmlEnum("Exception:FlaggedForAction")]
		ExceptionFlaggedForAction,
		[XmlEnum("Exception:FromAddresses")]
		ExceptionFromAddresses,
		[XmlEnum("Exception:FromConnectedAccounts")]
		ExceptionFromConnectedAccounts,
		[XmlEnum("Exception:HasAttachments")]
		ExceptionHasAttachments,
		[XmlEnum("Exception:Importance")]
		ExceptionImportance,
		[XmlEnum("Exception:IsApprovalRequest")]
		ExceptionIsApprovalRequest,
		[XmlEnum("Exception:IsAutomaticForward")]
		ExceptionIsAutomaticForward,
		[XmlEnum("Exception:IsAutomaticReply")]
		ExceptionIsAutomaticReply,
		[XmlEnum("Exception:IsEncrypted")]
		ExceptionIsEncrypted,
		[XmlEnum("Exception:IsMeetingRequest")]
		ExceptionIsMeetingRequest,
		[XmlEnum("Exception:IsMeetingResponse")]
		ExceptionIsMeetingResponse,
		[XmlEnum("Exception:IsNDR")]
		ExceptionIsNDR,
		[XmlEnum("Exception:IsPermissionControlled")]
		ExceptionIsPermissionControlled,
		[XmlEnum("Exception:IsReadReceipt")]
		ExceptionIsReadReceipt,
		[XmlEnum("Exception:IsSigned")]
		ExceptionIsSigned,
		[XmlEnum("Exception:IsVoicemail")]
		ExceptionIsVoicemail,
		[XmlEnum("Exception:ItemClasses")]
		ExceptionItemClasses,
		[XmlEnum("Exception:MessageClassifications")]
		ExceptionMessageClassifications,
		[XmlEnum("Exception:NotSentToMe")]
		ExceptionNotSentToMe,
		[XmlEnum("Exception:SentCcMe")]
		ExceptionSentCcMe,
		[XmlEnum("Exception:SentOnlyToMe")]
		ExceptionSentOnlyToMe,
		[XmlEnum("Exception:SentToAddresses")]
		ExceptionSentToAddresses,
		[XmlEnum("Exception:SentToMe")]
		ExceptionSentToMe,
		[XmlEnum("Exception:SentToOrCcMe")]
		ExceptionSentToOrCcMe,
		[XmlEnum("Exception:Sensitivity")]
		ExceptionSensitivity,
		[XmlEnum("Exception:WithinDateRange")]
		ExceptionWithinDateRange,
		[XmlEnum("Exception:WithinSizeRange")]
		ExceptionWithinSizeRange,
		[XmlEnum("Action:AssignCategories")]
		ActionAssignCategories,
		[XmlEnum("Action:CopyToFolder")]
		ActionCopyToFolder,
		[XmlEnum("Action:Delete")]
		ActionDelete,
		[XmlEnum("Action:ForwardAsAttachmentToRecipients")]
		ActionForwardAsAttachmentToRecipients,
		[XmlEnum("Action:ForwardToRecipients")]
		ActionForwardToRecipients,
		[XmlEnum("Action:MarkImportance")]
		ActionMarkImportance,
		[XmlEnum("Action:MarkAsRead")]
		ActionMarkAsRead,
		[XmlEnum("Action:MoveToFolder")]
		ActionMoveToFolder,
		[XmlEnum("Action:PermanentDelete")]
		ActionPermanentDelete,
		[XmlEnum("Action:RedirectToRecipients")]
		ActionRedirectToRecipients,
		[XmlEnum("Action:SendSMSAlertToRecipients")]
		ActionSendSMSAlertToRecipients,
		[XmlEnum("Action:ServerReplyWithMessage")]
		ActionServerReplyWithMessage,
		[XmlEnum("Action:StopProcessingRules")]
		ActionStopProcessingRules,
		IsEnabled,
		IsInError,
		Conditions,
		Exceptions
	}
}