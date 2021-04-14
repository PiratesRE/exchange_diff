using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("CAA")]
	[DataContract(Name = "CAA")]
	[KnownType(typeof(CaaAudioType))]
	public class CaaAudioType
	{
		[DataMember(Name = "pstnId", EmitDefaultValue = true)]
		[XmlElement("pstnId")]
		public string PstnId { get; set; }

		[XmlElement("region")]
		[DataMember(Name = "Region", EmitDefaultValue = true)]
		public CaaRegion Region { get; set; }

		[XmlElement("BypassLobby")]
		[DataMember(Name = "BypassLobby", EmitDefaultValue = true)]
		public bool BypassLobby { get; set; }

		[DataMember(Name = "AnnouncementEnabled", EmitDefaultValue = true)]
		[XmlElement("AnnouncementEnabled")]
		public bool AnnouncementEnabled { get; set; }
	}
}
