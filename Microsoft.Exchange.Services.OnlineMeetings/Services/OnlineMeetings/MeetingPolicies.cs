using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class MeetingPolicies
	{
		public AttendanceAnnouncementsStatus AttendanceAnnouncementsStatus { get; set; }

		public Policy PstnUserAdmission { get; set; }

		public Policy MeetingRecording { get; set; }

		public Policy ExternalUserRecording { get; set; }

		public int MeetingSize { get; set; }

		public Policy VoipAudio { get; set; }
	}
}
