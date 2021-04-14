using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[KnownType(typeof(Inband))]
	[XmlRoot("Inband")]
	[DataContract(Name = "Inband")]
	public class Inband
	{
		[XmlArray("ACPs")]
		[XmlArrayItem("acpInformation")]
		[DataMember(Name = "ACPs", EmitDefaultValue = true)]
		public AcpInformation[] ACPs { get; set; }

		[DataMember(Name = "MaxMeetingSize", EmitDefaultValue = true)]
		[XmlElement("MaxMeetingSize")]
		public int MaxMeetingSize { get; set; }

		[DataMember(Name = "AudioEnabled", EmitDefaultValue = true)]
		[XmlElement("AudioEnabled")]
		public bool AudioEnabled { get; set; }

		[DataMember(Name = "EnableEnterpriseCustomizedHelp", EmitDefaultValue = true)]
		[XmlElement("EnableEnterpriseCustomizedHelp")]
		public bool EnableEnterpriseCustomizedHelp { get; set; }

		[DataMember(Name = "CustomizedHelpUrl", EmitDefaultValue = true)]
		[XmlElement("CustomizedHelpUrl")]
		public string CustomizedHelpUrl { get; set; }
	}
}
