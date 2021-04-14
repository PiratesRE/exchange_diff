using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Get(typeof(OnlineMeetingDefaultValuesResource))]
	[DataContract(Name = "OnlineMeetingDefaultValuesResource")]
	[Parent("user")]
	internal class OnlineMeetingDefaultValuesResource : OnlineMeetingCapabilityResource
	{
		public OnlineMeetingDefaultValuesResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "entryExitAnnouncement", EmitDefaultValue = false)]
		public EntryExitAnnouncement EntryExitAnnouncement
		{
			get
			{
				return base.GetValue<EntryExitAnnouncement>("entryExitAnnouncement");
			}
			set
			{
				base.SetValue<EntryExitAnnouncement>("entryExitAnnouncement", value);
			}
		}

		[DataMember(Name = "automaticLeaderAssignment", EmitDefaultValue = false)]
		public AutomaticLeaderAssignment AutomaticLeaderAssignment
		{
			get
			{
				return base.GetValue<AutomaticLeaderAssignment>("automaticLeaderAssignment");
			}
			set
			{
				base.SetValue<AutomaticLeaderAssignment>("automaticLeaderAssignment", value);
			}
		}

		[DataMember(Name = "accessLevel", EmitDefaultValue = false)]
		public AccessLevel AccessLevel
		{
			get
			{
				return base.GetValue<AccessLevel>("accessLevel");
			}
			set
			{
				base.SetValue<AccessLevel>("accessLevel", value);
			}
		}

		[DataMember(Name = "participantsWarningThreshold", EmitDefaultValue = false)]
		public int ParticipantsWarningThreshold
		{
			get
			{
				return base.GetValue<int>("participantsWarningThreshold");
			}
			set
			{
				base.SetValue<int>("participantsWarningThreshold", value);
			}
		}

		[DataMember(Name = "lobbyBypassForPhoneUsers", EmitDefaultValue = false)]
		public LobbyBypassForPhoneUsers LobbyBypassForPhoneUsers
		{
			get
			{
				return base.GetValue<LobbyBypassForPhoneUsers>("lobbyBypassForPhoneUsers");
			}
			set
			{
				base.SetValue<LobbyBypassForPhoneUsers>("lobbyBypassForPhoneUsers", value);
			}
		}

		[DataMember(Name = "defaultOnlineMeetingRel", EmitDefaultValue = false)]
		public OnlineMeetingRel DefaultOnlineMeetingRel
		{
			get
			{
				return base.GetValue<OnlineMeetingRel>("defaultOnlineMeetingRel");
			}
			set
			{
				base.SetValue<OnlineMeetingRel>("defaultOnlineMeetingRel", value);
			}
		}

		public const string Token = "onlineMeetingDefaultValues";

		private const string EntryExitAnnouncementPropertyName = "entryExitAnnouncement";

		private const string AutomaticLeaderAssignmentPropertyName = "automaticLeaderAssignment";

		private const string AccessLevelPropertyName = "accessLevel";

		private const string ParticipantsWarningThresholdPropertyName = "participantsWarningThreshold";

		private const string LobbyBypassForPhoneUsersPropertyName = "lobbyBypassForPhoneUsers";

		private const string DefaultOnlineMeetingRelPropertyName = "defaultOnlineMeetingRel";
	}
}
