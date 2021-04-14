using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "meetingResource")]
	internal abstract class MeetingResource : Resource
	{
		protected MeetingResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "entryExitAnnouncement", EmitDefaultValue = false)]
		public EntryExitAnnouncement? EntryExitAnnouncement
		{
			get
			{
				return new EntryExitAnnouncement?(base.GetValue<EntryExitAnnouncement>("entryExitAnnouncement"));
			}
			set
			{
				base.SetValue<EntryExitAnnouncement>("entryExitAnnouncement", value);
			}
		}

		[DataMember(Name = "automaticLeaderAssignment", EmitDefaultValue = false)]
		public AutomaticLeaderAssignment? AutomaticLeaderAssignment
		{
			get
			{
				return new AutomaticLeaderAssignment?(base.GetValue<AutomaticLeaderAssignment>("automaticLeaderAssignment"));
			}
			set
			{
				base.SetValue<AutomaticLeaderAssignment>("automaticLeaderAssignment", value);
			}
		}

		[DataMember(Name = "accessLevel", EmitDefaultValue = false)]
		public AccessLevel? AccessLevel
		{
			get
			{
				return new AccessLevel?(base.GetValue<AccessLevel>("accessLevel"));
			}
			set
			{
				base.SetValue<AccessLevel>("accessLevel", value);
			}
		}

		[DataMember(Name = "onlineMeetingId", EmitDefaultValue = false)]
		public string OnlineMeetingId
		{
			get
			{
				return base.GetValue<string>("onlineMeetingId");
			}
			set
			{
				base.SetValue<string>("onlineMeetingId", value);
			}
		}

		[DataMember(Name = "onlineMeetingUri", EmitDefaultValue = false)]
		public string OnlineMeetingUri
		{
			get
			{
				return base.GetValue<string>("onlineMeetingUri");
			}
			set
			{
				base.SetValue<string>("onlineMeetingUri", value);
			}
		}

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description
		{
			get
			{
				return base.GetValue<string>("description");
			}
			set
			{
				base.SetValue<string>("description", value);
			}
		}

		[DataMember(Name = "expirationTime", EmitDefaultValue = false)]
		public DateTime? ExpirationTime
		{
			get
			{
				return base.GetValue<DateTime?>("expirationTime");
			}
			set
			{
				base.SetValue<DateTime?>("expirationTime", value);
			}
		}

		[DataMember(Name = "extensions", EmitDefaultValue = false)]
		public ResourceCollection<OnlineMeetingExtensionResource> Extensions
		{
			get
			{
				return base.GetValue<ResourceCollection<OnlineMeetingExtensionResource>>("extensions");
			}
			set
			{
				base.SetValue<ResourceCollection<OnlineMeetingExtensionResource>>("extensions", value);
			}
		}

		[DataMember(Name = "isActive", EmitDefaultValue = false)]
		public bool? IsActive
		{
			get
			{
				return base.GetValue<bool?>("isActive");
			}
			set
			{
				base.SetValue<bool?>("isActive", value);
			}
		}

		[DataMember(Name = "phoneUserAdmission", EmitDefaultValue = false)]
		public PhoneUserAdmission? PhoneUserAdmission
		{
			get
			{
				return base.GetValue<PhoneUserAdmission?>("phoneUserAdmission");
			}
			set
			{
				base.SetValue<PhoneUserAdmission?>("phoneUserAdmission", value);
			}
		}

		[DataMember(Name = "lobbyBypassForPhoneUsers", EmitDefaultValue = false)]
		public LobbyBypassForPhoneUsers? LobbyBypassForPhoneUsers
		{
			get
			{
				return base.GetValue<LobbyBypassForPhoneUsers?>("lobbyBypassForPhoneUsers");
			}
			set
			{
				base.SetValue<LobbyBypassForPhoneUsers?>("lobbyBypassForPhoneUsers", value);
			}
		}

		[DataMember(Name = "organizerUri", EmitDefaultValue = false)]
		public string OrganizerUri
		{
			get
			{
				return base.GetValue<string>("organizerUri");
			}
			set
			{
				base.SetValue<string>("organizerUri", value);
			}
		}

		[DataMember(Name = "leaders", EmitDefaultValue = false)]
		public string[] Leaders
		{
			get
			{
				return base.GetValue<string[]>("leaders");
			}
			set
			{
				base.SetValue<string[]>("leaders", value);
			}
		}

		[DataMember(Name = "attendees", EmitDefaultValue = false)]
		public string[] Attendees
		{
			get
			{
				return base.GetValue<string[]>("attendees");
			}
			set
			{
				base.SetValue<string[]>("attendees", value);
			}
		}

		[DataMember(Name = "conferenceId", EmitDefaultValue = false)]
		public string ConferenceId
		{
			get
			{
				return base.GetValue<string>("conferenceId");
			}
			set
			{
				base.SetValue<string>("conferenceId", value);
			}
		}

		[DataMember(Name = "onlineMeetingRel", EmitDefaultValue = false)]
		public OnlineMeetingRel? OnlineMeetingRel
		{
			get
			{
				return new OnlineMeetingRel?(base.GetValue<OnlineMeetingRel>("onlineMeetingRel"));
			}
			set
			{
				base.SetValue<OnlineMeetingRel>("onlineMeetingRel", value);
			}
		}

		[DataMember(Name = "subject", EmitDefaultValue = false)]
		public string Subject
		{
			get
			{
				return base.GetValue<string>("subject");
			}
			set
			{
				base.SetValue<string>("subject", value);
			}
		}

		[DataMember(Name = "joinUrl", EmitDefaultValue = false)]
		public string JoinUrl
		{
			get
			{
				return base.GetValue<string>("joinUrl");
			}
			set
			{
				base.SetValue<string>("joinUrl", value);
			}
		}

		[Ignore]
		public long? Version { get; set; }

		private const string EntryExitAnnouncementPropertyName = "entryExitAnnouncement";

		private const string AutomaticLeaderAssignmentPropertyName = "automaticLeaderAssignment";

		private const string AccessLevelPropertyName = "accessLevel";

		private const string OnlineMeetingIdPropertyName = "onlineMeetingId";

		private const string OnlineMeetingUriPropertyName = "onlineMeetingUri";

		private const string DescriptionPropertyName = "description";

		private const string ExpirationTimePropertyName = "expirationTime";

		private const string IsActivePropertyName = "isActive";

		private const string ExtensionsPropertyName = "extensions";

		private const string LobbyBypassForPhoneUsersPropertyName = "lobbyBypassForPhoneUsers";

		private const string OrganizerUriPropertyName = "organizerUri";

		private const string LeadersPropertyName = "leaders";

		private const string AttendeesPropertyName = "attendees";

		private const string ConferenceIdPropertyName = "conferenceId";

		private const string OnlineMeetingRelPropertyName = "onlineMeetingRel";

		private const string SubjectPropertyName = "subject";

		private const string JoinUrlPropertyName = "joinUrl";
	}
}
