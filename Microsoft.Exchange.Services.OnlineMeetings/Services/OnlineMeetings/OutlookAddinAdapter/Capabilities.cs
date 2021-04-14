using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("Capabilities")]
	[DataContract(Name = "Capabilities")]
	[KnownType(typeof(Capabilities))]
	public class Capabilities
	{
		[XmlElement("CAAEnabled")]
		[DataMember(Name = "CAAEnabled")]
		public bool CAAEnabled { get; set; }

		[DataMember(Name = "AnonymousAllowed")]
		[XmlElement("AnonymousAllowed")]
		public bool AnonymousAllowed { get; set; }

		[DataMember(Name = "PublicMeetingLimit")]
		[XmlElement("PublicMeetingLimit")]
		public int PublicMeetingLimit { get; set; }

		[XmlElement("PublicMeetingDefault")]
		[DataMember(Name = "PublicMeetingDefault")]
		public bool PublicMeetingDefault { get; set; }

		[DataMember(Name = "AutoPromoteAllowed")]
		[XmlElement("AutoPromoteAllowed")]
		public AutoPromoteEnum AutoPromoteAllowed { get; set; }

		[XmlElement("DefaultAutoPromote")]
		[DataMember(Name = "DefaultAutoPromote")]
		public AutoPromoteEnum DefaultAutoPromote { get; set; }

		[DataMember(Name = "BypassLobbyEnabled")]
		[XmlElement("BypassLobbyEnabled")]
		public bool BypassLobbyEnabled { get; set; }

		[XmlElement("ForgetPinUrl")]
		[DataMember(Name = "ForgetPinUrl")]
		public string ForgetPinUrl { get; set; }

		[DataMember(Name = "LocalPhoneUrl")]
		[XmlElement("LocalPhoneUrl")]
		public string LocalPhoneUrl { get; set; }

		[DataMember(Name = "DefaultAnnouncementEnabled")]
		[XmlElement("DefaultAnnouncementEnabled")]
		public bool DefaultAnnouncementEnabled { get; set; }

		[DataMember(Name = "ACPMCUEnabled")]
		[XmlElement("ACPMCUEnabled")]
		public bool ACPMCUEnabled { get; set; }

		[XmlArray("Regions")]
		[XmlArrayItem("Region")]
		[DataMember(Name = "Regions")]
		public Region[] Regions { get; set; }

		[XmlElement("custom-invite")]
		[DataMember(Name = "custom-invite")]
		public CustomInvite CustomInvite { get; set; }
	}
}
