using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal static class UcwaNewExtensions
	{
		internal static AttendanceAnnouncementsStatus ToOnlineMeetingValue(this EntryExitAnnouncement thisObject)
		{
			switch (thisObject)
			{
			case EntryExitAnnouncement.Unsupported:
				return AttendanceAnnouncementsStatus.Unsupported;
			case EntryExitAnnouncement.Disabled:
				return AttendanceAnnouncementsStatus.Disabled;
			case EntryExitAnnouncement.Enabled:
				return AttendanceAnnouncementsStatus.Enabled;
			default:
				throw new ArgumentOutOfRangeException("thisObject");
			}
		}

		internal static AutomaticLeaderAssignment ToOnlineMeetingValue(this AutomaticLeaderAssignment thisObject)
		{
			if (thisObject == AutomaticLeaderAssignment.Disabled)
			{
				return AutomaticLeaderAssignment.Disabled;
			}
			if (thisObject == AutomaticLeaderAssignment.SameEnterprise)
			{
				return AutomaticLeaderAssignment.SameEnterprise;
			}
			if (thisObject != (AutomaticLeaderAssignment)((ulong)-2147483648))
			{
				throw new ArgumentOutOfRangeException("thisObject");
			}
			return AutomaticLeaderAssignment.Everyone;
		}

		internal static AccessLevel ToOnlineMeetingValue(this AccessLevel thisObject)
		{
			switch (thisObject)
			{
			case AccessLevel.None:
				return AccessLevel.None;
			case AccessLevel.SameEnterprise:
				return AccessLevel.SameEnterprise;
			case AccessLevel.Locked:
				return AccessLevel.Locked;
			case AccessLevel.Invited:
				return AccessLevel.Invited;
			case AccessLevel.Everyone:
				return AccessLevel.Everyone;
			default:
				throw new ArgumentOutOfRangeException("thisObject");
			}
		}

		internal static LobbyBypass ToOnlineMeetingValue(this LobbyBypassForPhoneUsers thisObject)
		{
			switch (thisObject)
			{
			case LobbyBypassForPhoneUsers.Disabled:
				return LobbyBypass.Disabled;
			case LobbyBypassForPhoneUsers.Enabled:
				return LobbyBypass.Enabled;
			default:
				throw new ArgumentOutOfRangeException("thisObject");
			}
		}

		internal static MeetingPolicies ToOnlineMeetingValue(this OnlineMeetingPoliciesResource thisObject)
		{
			return new MeetingPolicies
			{
				AttendanceAnnouncementsStatus = ((thisObject.EntryExitAnnouncement == GenericPolicy.Enabled) ? AttendanceAnnouncementsStatus.Enabled : AttendanceAnnouncementsStatus.Disabled),
				ExternalUserRecording = ((thisObject.ExternalUserMeetingRecording == GenericPolicy.Enabled) ? Policy.Enabled : Policy.Disabled),
				MeetingRecording = ((thisObject.MeetingRecording == GenericPolicy.Enabled) ? Policy.Enabled : Policy.Disabled),
				MeetingSize = thisObject.MeetingSize,
				PstnUserAdmission = ((thisObject.PhoneUserAdmission == GenericPolicy.Enabled) ? Policy.Enabled : Policy.Disabled),
				VoipAudio = ((thisObject.VoipAudio == GenericPolicy.Enabled) ? Policy.Enabled : Policy.Disabled)
			};
		}

		internal static DefaultValues ToOnlineMeetingDefaultValues(this OnlineMeetingDefaultValuesResource thisObject)
		{
			return new DefaultValues
			{
				AccessLevel = thisObject.AccessLevel.ToOnlineMeetingValue(),
				AttendanceAnnouncementsStatus = thisObject.EntryExitAnnouncement.ToOnlineMeetingValue(),
				AutomaticLeaderAssignment = thisObject.AutomaticLeaderAssignment.ToOnlineMeetingValue(),
				PstnLobbyByPass = thisObject.LobbyBypassForPhoneUsers.ToOnlineMeetingValue()
			};
		}

		internal static DialInInformation ToOnlineMeetingDialInValues(this PhoneDialInInformationResource thisObject)
		{
			return new DialInInformation
			{
				DialInRegions = thisObject.ToOnlineMeetingDialInRegions(),
				ExternalDirectoryUri = thisObject.ExternalDirectoryUri,
				InternalDirectoryUri = thisObject.InternalDirectoryUri,
				IsAudioConferenceProviderEnabled = thisObject.IsAudioConferenceProviderEnabled,
				ParticipantPassCode = thisObject.ParticipantPassCode,
				TollFreeNumbers = thisObject.TollFreeNumbers,
				TollNumber = thisObject.TollNumber
			};
		}

		internal static DialInRegions ToOnlineMeetingDialInRegions(this PhoneDialInInformationResource thisObject)
		{
			if (thisObject.DialInRegions != null)
			{
				IEnumerable<DialInRegion> source = from region in thisObject.DialInRegions
				where region != null && (string.IsNullOrEmpty(thisObject.DefaultRegion) || string.Compare(thisObject.DefaultRegion, region.Name, StringComparison.OrdinalIgnoreCase) == 0)
				select new DialInRegion
				{
					Languages = ((region.Languages != null) ? new Collection<string>(region.Languages) : new Collection<string>()),
					Name = region.Name,
					Number = region.Number
				};
				return new DialInRegions(source.ToList<DialInRegion>());
			}
			return new DialInRegions();
		}

		internal static CustomizationValues ToOnlineMeetingCustomizationValues(this OnlineMeetingInvitationCustomizationResource thisObject)
		{
			return new CustomizationValues
			{
				EnterpriseHelpUrl = thisObject.EnterpriseHelpUrl,
				InvitationFooterText = thisObject.InvitationFooterText,
				InvitationHelpUrl = thisObject.InvitationHelpUrl,
				InvitationLegalUrl = thisObject.InvitationLegalUrl,
				InvitationLogoUrl = thisObject.InvitationLogoUrl
			};
		}

		internal static OnlineMeeting ToOnlineMeetingValue(this OnlineMeetingResource thisObject)
		{
			string[] leaders = thisObject.Leaders;
			IEnumerable<string> source = (leaders != null) ? ((IEnumerable<string>)leaders) : Enumerable.Empty<string>();
			string[] attendees = thisObject.Attendees;
			IEnumerable<string> source2 = (attendees != null) ? ((IEnumerable<string>)attendees) : Enumerable.Empty<string>();
			OnlineMeeting onlineMeeting = new OnlineMeeting(new Collection<string>(source.ToList<string>()), new Collection<string>(source2.ToList<string>()))
			{
				Accesslevel = ((thisObject.AccessLevel != null) ? thisObject.AccessLevel.Value.ToOnlineMeetingValue() : AccessLevel.None),
				AttendanceAnnouncementStatus = ((thisObject.EntryExitAnnouncement != null) ? thisObject.EntryExitAnnouncement.Value.ToOnlineMeetingValue() : AttendanceAnnouncementsStatus.Unsupported),
				AutomaticLeaderAssignment = ((thisObject.AutomaticLeaderAssignment != null) ? thisObject.AutomaticLeaderAssignment.Value.ToOnlineMeetingValue() : AutomaticLeaderAssignment.Disabled),
				Description = thisObject.Description,
				ExpiryTime = thisObject.ExpirationTime,
				Id = thisObject.OnlineMeetingId,
				IsActive = thisObject.IsActive,
				MeetingUri = thisObject.OnlineMeetingUri,
				OrganizerUri = thisObject.OrganizerUri,
				PstnMeetingId = thisObject.ConferenceId,
				PstnUserLobbyBypass = ((thisObject.LobbyBypassForPhoneUsers != null) ? thisObject.LobbyBypassForPhoneUsers.Value.ToOnlineMeetingValue() : LobbyBypass.Disabled),
				Subject = thisObject.Subject,
				WebUrl = thisObject.JoinUrl
			};
			if (thisObject.OnlineMeetingRel != null && thisObject.OnlineMeetingRel.Value == OnlineMeetingRel.MyAssignedOnlineMeeting)
			{
				onlineMeeting.IsAssignedMeeting = true;
			}
			else
			{
				onlineMeeting.IsAssignedMeeting = false;
			}
			return onlineMeeting;
		}

		internal static string GetReasonHeader(this WebResponse thisObject)
		{
			if (thisObject != null && thisObject.Headers != null)
			{
				return thisObject.Headers["X-Ms-diagnostics"] ?? string.Empty;
			}
			return string.Empty;
		}

		internal static string ToLogString(this HttpOperationException thisObject)
		{
			if (thisObject == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder((thisObject.InnerException != null) ? thisObject.InnerException.Message : thisObject.Message);
			if (thisObject.ErrorInformation != null)
			{
				stringBuilder.AppendFormat("ErrorInformation object: {0}.", thisObject.ErrorInformation.ToString());
			}
			return stringBuilder.ToString();
		}

		internal static string ToLogString(this AggregateException thisObject)
		{
			if (thisObject == null || thisObject.InnerExceptions == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Exception ex in thisObject.InnerExceptions)
			{
				if (ex is HttpOperationException)
				{
					stringBuilder.AppendFormat("HttpOperationException:{0};", ((HttpOperationException)ex).ToLogString());
				}
				else
				{
					stringBuilder.AppendFormat("{0}:{1};", ex.GetType(), (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
