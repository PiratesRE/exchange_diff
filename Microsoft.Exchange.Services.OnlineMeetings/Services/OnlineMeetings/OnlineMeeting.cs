using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OnlineMeeting
	{
		public OnlineMeeting(Collection<string> leaders, Collection<string> attendees)
		{
			this.leaders = leaders;
			this.attendees = attendees;
		}

		public string Id { get; set; }

		public string MeetingUri { get; set; }

		public string WebUrl { get; set; }

		public string PstnMeetingId { get; set; }

		public LobbyBypass PstnUserLobbyBypass { get; set; }

		public string OrganizerUri { get; set; }

		public AttendanceAnnouncementsStatus AttendanceAnnouncementStatus { get; set; }

		public AutomaticLeaderAssignment AutomaticLeaderAssignment { get; set; }

		public string Subject { get; set; }

		public string Description { get; set; }

		public DateTime? ExpiryTime { get; set; }

		public bool? IsActive { get; set; }

		public AccessLevel Accesslevel { get; set; }

		public IEnumerable<string> Leaders
		{
			get
			{
				return this.leaders;
			}
		}

		public IEnumerable<string> Attendees
		{
			get
			{
				return this.attendees;
			}
		}

		public bool IsAssignedMeeting { get; set; }

		private readonly Collection<string> leaders;

		private readonly Collection<string> attendees;
	}
}
