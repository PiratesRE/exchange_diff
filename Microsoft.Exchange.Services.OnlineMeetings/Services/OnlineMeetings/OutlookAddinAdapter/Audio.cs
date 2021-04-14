using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[KnownType(typeof(Audio))]
	[XmlType("Audio")]
	[DataContract(Name = "Audio")]
	public class Audio
	{
		[XmlAttribute("Type")]
		[DataMember(Name = "Type", EmitDefaultValue = true)]
		public AudioType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.AudioModalityEnabled = (this.type != AudioType.None);
			}
		}

		[XmlAttribute("AudioModalityEnabled")]
		[DataMember(Name = "AudioModalityEnabled", EmitDefaultValue = true)]
		public bool AudioModalityEnabled { get; set; }

		[DataMember(Name = "CAA", EmitDefaultValue = false)]
		[XmlElement("CAA")]
		public CaaAudioType CaaAudio { get; set; }

		[DataMember(Name = "ACP", EmitDefaultValue = false)]
		[XmlElement("ACP")]
		public AcpAudioType AcpAudio { get; set; }

		internal static Audio ConvertFrom(OnlineMeetingResult onlineMeetingResult)
		{
			Audio audio = new Audio();
			audio.AudioModalityEnabled = (onlineMeetingResult.MeetingPolicies.VoipAudio == Policy.Enabled);
			audio.Type = AudioType.CAA;
			audio.CaaAudio = new CaaAudioType();
			audio.CaaAudio.PstnId = onlineMeetingResult.OnlineMeeting.PstnMeetingId;
			if (onlineMeetingResult.DialIn.DialInRegions.Count > 0)
			{
				audio.CaaAudio.Region = new CaaRegion();
				audio.CaaAudio.Region.Name = onlineMeetingResult.DialIn.DialInRegions[0].Name;
			}
			audio.CaaAudio.BypassLobby = (onlineMeetingResult.OnlineMeeting.PstnUserLobbyBypass == LobbyBypass.Enabled);
			audio.CaaAudio.AnnouncementEnabled = (onlineMeetingResult.OnlineMeeting.AttendanceAnnouncementStatus == AttendanceAnnouncementsStatus.Enabled);
			return audio;
		}

		private AudioType type;
	}
}
