using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("user")]
	[DataContract(Name = "OnlineMeetingPoliciesResource")]
	[Get(typeof(OnlineMeetingPoliciesResource))]
	internal class OnlineMeetingPoliciesResource : OnlineMeetingCapabilityResource
	{
		public OnlineMeetingPoliciesResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "entryExitAnnouncement", EmitDefaultValue = false)]
		public GenericPolicy EntryExitAnnouncement
		{
			get
			{
				return base.GetValue<GenericPolicy>("entryExitAnnouncement");
			}
			set
			{
				base.SetValue<GenericPolicy>("entryExitAnnouncement", value);
			}
		}

		[DataMember(Name = "phoneUserAdmission", EmitDefaultValue = false)]
		public GenericPolicy PhoneUserAdmission
		{
			get
			{
				return base.GetValue<GenericPolicy>("phoneUserAdmission");
			}
			set
			{
				base.SetValue<GenericPolicy>("phoneUserAdmission", value);
			}
		}

		[DataMember(Name = "externalUserMeetingRecording", EmitDefaultValue = false)]
		public GenericPolicy ExternalUserMeetingRecording
		{
			get
			{
				return base.GetValue<GenericPolicy>("externalUserMeetingRecording");
			}
			set
			{
				base.SetValue<GenericPolicy>("externalUserMeetingRecording", value);
			}
		}

		[DataMember(Name = "meetingRecording", EmitDefaultValue = false)]
		public GenericPolicy MeetingRecording
		{
			get
			{
				return base.GetValue<GenericPolicy>("meetingRecording");
			}
			set
			{
				base.SetValue<GenericPolicy>("meetingRecording", value);
			}
		}

		[DataMember(Name = "voipAudio", EmitDefaultValue = false)]
		public GenericPolicy VoipAudio
		{
			get
			{
				return base.GetValue<GenericPolicy>("voipAudio");
			}
			set
			{
				base.SetValue<GenericPolicy>("voipAudio", value);
			}
		}

		[DataMember(Name = "meetingSize", EmitDefaultValue = false)]
		public int MeetingSize
		{
			get
			{
				return base.GetValue<int>("meetingSize");
			}
			set
			{
				base.SetValue<int>("meetingSize", value);
			}
		}

		public const string Token = "onlineMeetingPolicies";

		private const string EntryExitAnnouncementPropertyName = "entryExitAnnouncement";

		private const string PhoneUserAdmissionPropertyName = "phoneUserAdmission";

		private const string ExternalUserMeetingRecordingPropertyName = "externalUserMeetingRecording";

		private const string MeetingRecordingPropertyName = "meetingRecording";

		private const string VoipAudioPropertyName = "voipAudio";

		private const string MeetingSizePropertyName = "meetingSize";
	}
}
