using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "subcode")]
	internal enum ErrorSubcode
	{
		None,
		[EnumMember]
		ServiceFailure,
		[EnumMember]
		BadRequest,
		[EnumMember]
		Forbidden,
		[EnumMember]
		ResourceNotFound,
		[EnumMember]
		MethodNotAllowed,
		[EnumMember]
		Conflict,
		[EnumMember]
		InvalidOperation,
		[EnumMember]
		TooManyRequests,
		[EnumMember]
		RequestTooLarge,
		[EnumMember]
		ResourceTerminating,
		[EnumMember]
		ResourceExists,
		[EnumMember]
		InvalidResourceKey,
		[EnumMember]
		InvalidResourceState,
		[EnumMember]
		InvalidRequestBody,
		[EnumMember]
		ApplicationNotFound = 1001,
		[EnumMember]
		OnlineMeetingNotFound = 4001,
		[EnumMember]
		OnlineMeetingExists,
		[EnumMember]
		ConversationNotFound = 5001,
		[EnumMember]
		InvitationNotFound,
		[EnumMember]
		CallNotFound,
		[EnumMember]
		SessionNotFound,
		[EnumMember]
		ConversationOperationFailed,
		[EnumMember]
		InvalidInvitationType,
		[EnumMember]
		SessionContextNotChangable,
		[EnumMember]
		PendingSessionRenegotiation,
		[EnumMember]
		CallNotAnswered,
		[EnumMember]
		CallCancelled,
		[EnumMember]
		CallDeclined,
		[EnumMember]
		CallFailed,
		[EnumMember]
		CallTransfered,
		[EnumMember]
		CallReplaced,
		[EnumMember]
		InvalidSDP,
		[EnumMember]
		MediaTypeNotSupported,
		[EnumMember]
		OfferAnswerFailure,
		[EnumMember]
		AudioUnavailable,
		[EnumMember]
		UserNotEnabledForOutsideVoice,
		[EnumMember]
		CallTransferFailed
	}
}
