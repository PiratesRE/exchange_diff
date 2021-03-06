using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal enum ErrorSubcode
	{
		None,
		ServiceFailure,
		ServiceUnavailable,
		Timeout,
		RequestCancelled,
		Forbidden,
		NotFound,
		MethodNotAllowed,
		ClientTimeout,
		Conflict,
		Gone,
		PreconditionFailed,
		PreconditionRequired,
		EntityTooLarge,
		TooManyRequests,
		RequestTooLarge,
		UnsupportedMediaType,
		LocalFailure,
		RemoteFailure,
		ResourceTerminating,
		ResourceExists,
		InvalidRequest,
		UnhandledException,
		Unknown,
		AnonymousNotAllowed,
		InviteesOnly,
		SignInForCommunicationRequired,
		NotEnterpriseVoiceEnabled,
		UserLookupFailed,
		AuthenticatedJoinNotSupported,
		AlreadyExists,
		ApplicationNotFound = 1001,
		ApplicationTerminating,
		NoteSizeTooBig = 2001,
		UnknownPhoneType,
		ReadOnlyPhoneType,
		DuplicatePhoneType,
		InvalidTeamDelegateRingTime,
		DelegateRingDisabled,
		ForwardImmediateCustomDisabled,
		SimulRingCustomDisabled,
		TeamRingDisabled,
		VoicemailNotEnabled,
		NoDelegatesConfigured,
		NoTeamMembersConfigured,
		InvalidUnansweredRingTime,
		UnknownAvailability,
		PhotoDisabled,
		UnauthorizedPhotoDisplay,
		ResultExceeededLimits = 3001,
		MembershipChangesNotSupported,
		TooManyOnlineMeetings,
		ThreadIdAlreadyExists,
		DoNotDisturb,
		ConnectedElsewhere,
		SessionNotFound,
		SessionContextNotChangable,
		CallNotAnswered,
		FederationRequired,
		Canceled,
		Declined,
		CallNotAcceptable,
		Transferred,
		CallReplaced,
		EscalationFailed,
		InvalidSDP,
		OfferAnswerFailure,
		AudioUnavailable,
		UserNotEnabledForOutsideVoice,
		InsufficientBandwidth,
		RepliedWithOtherModality,
		DestinationNotFound,
		DialoutNotAllowed,
		Unreachable,
		MediaEncryptionNotSupported,
		MediaEncryptionRequired,
		Unavailable,
		TooManyParticipants,
		TooManyLobbyParticipants,
		Busy,
		AttendeeNotAllowed,
		Demoted,
		MediaFailure,
		Removed,
		TemporarilyUnavailable,
		ModalityNotSupported,
		NotAllowed,
		Ejected,
		Denied,
		Ended,
		ConversationTerminatedNoConnectedModality,
		ParameterValidationFailure,
		UserNotEnabledForPushNotifications,
		PushNotificationSubscriptionFailure,
		PushNotificationSubscriptionAlreadyExists,
		ConferenceRoleChanged,
		MissingAnonymousDisplayName,
		SessionSwitched,
		EventChannelOutOfSync = 6001
	}
}
