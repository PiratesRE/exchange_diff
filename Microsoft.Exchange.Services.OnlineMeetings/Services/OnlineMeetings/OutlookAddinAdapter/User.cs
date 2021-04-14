using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[DataContract(Name = "User")]
	[XmlType("User")]
	[KnownType(typeof(User))]
	public class User
	{
		[DataMember(Name = "Name", EmitDefaultValue = true)]
		[XmlAttribute("name")]
		public string Name { get; set; }

		[DataMember(Name = "SmtpAddress", EmitDefaultValue = true)]
		[XmlAttribute("Smtp")]
		public string SmtpAddress { get; set; }

		[DataMember(Name = "SipAddress", EmitDefaultValue = true)]
		[XmlAttribute("Sip")]
		public string SipAddress { get; set; }
	}
}
