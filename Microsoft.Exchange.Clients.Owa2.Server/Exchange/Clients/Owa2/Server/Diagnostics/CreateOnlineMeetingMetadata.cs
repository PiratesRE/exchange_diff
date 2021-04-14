using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum CreateOnlineMeetingMetadata
	{
		[DisplayName("OM", "MS")]
		ManagerSipUri,
		[DisplayName("OM", "ORG")]
		Organization,
		[DisplayName("OM", "URL")]
		UcwaUrl,
		[DisplayName("OM", "UGUID")]
		UserGuid,
		[DisplayName("OM", "CID")]
		ConferenceId,
		[DisplayName("OM", "IID")]
		ItemId,
		[DisplayName("OM", "ITC")]
		IsTaskCompleted,
		[DisplayName("OM", "IUS")]
		IsUcwaSupported,
		[DisplayName("OM", "OCID")]
		OAuthCorrelationId,
		[DisplayName("OM", "EX")]
		Exceptions,
		[DisplayName("OM", "WEX")]
		WorkerExceptions,
		[DisplayName("OM", "LCT")]
		LeaderCount,
		[DisplayName("OM", "ACT")]
		AttendeeCount,
		[DisplayName("OM", "EXPT")]
		ExpiryTime,
		[DisplayName("OM", "EXA")]
		DefaultEntryExitAnnouncement,
		[DisplayName("OM", "ALA")]
		AutomaticLeaderAssignment,
		[DisplayName("OM", "ALVL")]
		AccessLevel,
		[DisplayName("OM", "PWT")]
		ParticipantsWarningThreshold,
		[DisplayName("OM", "EXAP")]
		PolicyEntryExitAnnouncement,
		[DisplayName("OM", "PUA")]
		PhoneUserAdmission,
		[DisplayName("OM", "EMR")]
		ExternalUserMeetingRecording,
		[DisplayName("OM", "MR")]
		MeetingRecording,
		[DisplayName("OM", "VA")]
		VoipAudio,
		[DisplayName("OM", "MSZ")]
		MeetingSize,
		[DisplayName("OM", "CO")]
		CacheOperation,
		[DisplayName("OM", "RSH")]
		ResponseHeaders,
		[DisplayName("OM", "RSB")]
		ResponseBody
	}
}
