using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("AccessNumber")]
	[DataContract(Name = "AccessNumber")]
	[KnownType(typeof(AccessNumber))]
	public class AccessNumber
	{
		[DataMember(Name = "Number", EmitDefaultValue = true)]
		[XmlAttribute("Number")]
		public string Number { get; set; }

		[DataMember(Name = "LanguageID", EmitDefaultValue = true)]
		[XmlAttribute("LanguageID")]
		public int LanguageID { get; set; }
	}
}
