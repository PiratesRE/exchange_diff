using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[DataContract(Name = "MeetingOwner")]
	[KnownType(typeof(MeetingOwner))]
	[XmlType("MeetingOwner")]
	public class MeetingOwner
	{
		[XmlAttribute("Smtp")]
		[DataMember(Name = "Smtp", EmitDefaultValue = true)]
		public string Smtp { get; set; }

		[DataMember(Name = "Sip", EmitDefaultValue = true)]
		[XmlAttribute("Sip")]
		public string Sip { get; set; }

		public static MeetingOwner ConvertFrom(string organizerUri)
		{
			return new MeetingOwner
			{
				Smtp = organizerUri,
				Sip = organizerUri
			};
		}
	}
}
