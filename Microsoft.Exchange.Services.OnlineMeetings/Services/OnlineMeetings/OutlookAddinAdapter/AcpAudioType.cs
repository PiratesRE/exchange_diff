using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("ACP")]
	[DataContract(Name = "ACP")]
	[KnownType(typeof(AcpAudioType))]
	public class AcpAudioType
	{
		[DataMember(Name = "Domain", EmitDefaultValue = true)]
		[XmlElement("Domain")]
		public string Domain { get; set; }

		[XmlElement("ACPMCUEnabled")]
		[DataMember(Name = "ACPMCUEnabled", EmitDefaultValue = true)]
		public bool AcpMcuEnabled { get; set; }
	}
}
