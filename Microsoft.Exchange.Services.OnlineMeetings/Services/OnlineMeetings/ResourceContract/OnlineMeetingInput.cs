using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "OnlineMeetingInput")]
	internal class OnlineMeetingInput : Resource
	{
		public OnlineMeetingInput() : base(string.Empty)
		{
		}

		public OnlineMeetingInput(string selfUri) : base(selfUri)
		{
		}

		public OnlineMeetingInput(OnlineMeetingInput input) : this()
		{
			this.AccessLevel = input.AccessLevel;
			this.EntryExitAnnouncement = input.EntryExitAnnouncement;
			this.Attendees = input.Attendees;
			this.AutomaticLeaderAssignment = input.AutomaticLeaderAssignment;
			this.Description = input.Description;
			this.ExpirationTime = input.ExpirationTime;
			this.Leaders = input.Leaders;
			this.PhoneUserAdmission = input.PhoneUserAdmission;
			this.LobbyBypassForPhoneUsers = input.LobbyBypassForPhoneUsers;
			this.Subject = input.Subject;
		}

		[DataMember(Name = "entryExitAnnouncement", EmitDefaultValue = false)]
		public EntryExitAnnouncement? EntryExitAnnouncement
		{
			get
			{
				return base.GetValue<EntryExitAnnouncement?>("entryExitAnnouncement");
			}
			set
			{
				base.SetValue<EntryExitAnnouncement?>("entryExitAnnouncement", value);
			}
		}

		[DataMember(Name = "automaticLeaderAssignment", EmitDefaultValue = false)]
		public AutomaticLeaderAssignment? AutomaticLeaderAssignment
		{
			get
			{
				return base.GetValue<AutomaticLeaderAssignment?>("automaticLeaderAssignment");
			}
			set
			{
				base.SetValue<AutomaticLeaderAssignment?>("automaticLeaderAssignment", value);
			}
		}

		[DataMember(Name = "accessLevel", EmitDefaultValue = false)]
		public AccessLevel? AccessLevel
		{
			get
			{
				return base.GetValue<AccessLevel?>("accessLevel");
			}
			set
			{
				base.SetValue<AccessLevel?>("accessLevel", value);
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

		internal static class PropertyNames
		{
			public const string AccessLevel = "accessLevel";

			public const string Attendees = "attendees";

			public const string AutomaticLeaderAssignment = "automaticLeaderAssignment";

			public const string Description = "description";

			public const string EntryExitAnnouncement = "entryExitAnnouncement";

			public const string ExpirationTime = "expirationTime";

			public const string Leaders = "leaders";

			public const string LobbyBypassForPhoneUsers = "lobbyBypassForPhoneUsers";

			public const string PhoneUserAdmission = "phoneUserAdmission";

			public const string Subject = "subject";
		}
	}
}
