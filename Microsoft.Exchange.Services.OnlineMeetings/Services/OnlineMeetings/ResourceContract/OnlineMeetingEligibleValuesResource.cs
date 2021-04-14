using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("user")]
	[DataContract(Name = "onlineMeetingEligibleValuesResource")]
	[Get(typeof(OnlineMeetingEligibleValuesResource))]
	internal class OnlineMeetingEligibleValuesResource : OnlineMeetingCapabilityResource
	{
		public OnlineMeetingEligibleValuesResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "entryExitAnnouncements", EmitDefaultValue = false)]
		public EntryExitAnnouncement[] EntryExitAnnouncements
		{
			get
			{
				return base.GetValue<EntryExitAnnouncement[]>("entryExitAnnouncements");
			}
			set
			{
				base.SetValue<EntryExitAnnouncement[]>("entryExitAnnouncements", value);
			}
		}

		[DataMember(Name = "automaticLeaderAssignments", EmitDefaultValue = false)]
		public AutomaticLeaderAssignment[] AutomaticLeaderAssignments
		{
			get
			{
				return base.GetValue<AutomaticLeaderAssignment[]>("automaticLeaderAssignments");
			}
			set
			{
				base.SetValue<AutomaticLeaderAssignment[]>("automaticLeaderAssignments", value);
			}
		}

		[DataMember(Name = "accessLevels", EmitDefaultValue = false)]
		public AccessLevel[] AccessLevels
		{
			get
			{
				return base.GetValue<AccessLevel[]>("accessLevels");
			}
			set
			{
				base.SetValue<AccessLevel[]>("accessLevels", value);
			}
		}

		[DataMember(Name = "lobbyBypassForPhoneUsersSettings", EmitDefaultValue = false)]
		public LobbyBypassForPhoneUsers[] LobbyBypassForPhoneUsersSettings
		{
			get
			{
				return base.GetValue<LobbyBypassForPhoneUsers[]>("lobbyBypassForPhoneUsersSettings");
			}
			set
			{
				base.SetValue<LobbyBypassForPhoneUsers[]>("lobbyBypassForPhoneUsersSettings", value);
			}
		}

		[DataMember(Name = "eligibleOnlineMeetingRels", EmitDefaultValue = false)]
		public OnlineMeetingRel[] EligibleOnlineMeetingRels
		{
			get
			{
				return base.GetValue<OnlineMeetingRel[]>("eligibleOnlineMeetingRels");
			}
			set
			{
				base.SetValue<OnlineMeetingRel[]>("eligibleOnlineMeetingRels", value);
			}
		}

		public const string Token = "onlineMeetingEligibleValues";
	}
}
