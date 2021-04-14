using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("acpInformation")]
	[KnownType(typeof(AcpInformation))]
	[DataContract(Name = "acpInformation")]
	public class AcpInformation
	{
		[XmlAttribute("default")]
		[DataMember(Name = "default", EmitDefaultValue = false)]
		public bool IsDefault { get; set; }

		[XmlElement("tollNumber")]
		[DataMember(Name = "TollNumber", EmitDefaultValue = false)]
		public string TollNumber { get; set; }

		[DataMember(Name = "TollFreeNumber", EmitDefaultValue = false)]
		[XmlElement("tollFreeNumber")]
		public string[] TollFreeNumber { get; set; }

		[XmlElement("participantPassCode")]
		[DataMember(Name = "ParticipantPassCode", EmitDefaultValue = true)]
		public string ParticipantPassCode { get; set; }

		[XmlElement("domain")]
		[DataMember(Name = "Domain", EmitDefaultValue = true)]
		public string Domain { get; set; }

		[DataMember(Name = "name", EmitDefaultValue = false)]
		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("url")]
		[DataMember(Name = "Url", EmitDefaultValue = true)]
		public string Url { get; set; }

		internal static AcpInformation[] ConvertFrom(DialInInformation dialIn)
		{
			Collection<AcpInformation> collection = new Collection<AcpInformation>();
			if (dialIn != null && dialIn.IsAudioConferenceProviderEnabled)
			{
				collection.Add(new AcpInformation
				{
					Url = dialIn.ExternalDirectoryUri,
					TollFreeNumber = dialIn.TollFreeNumbers,
					TollNumber = dialIn.TollNumber,
					ParticipantPassCode = dialIn.ParticipantPassCode
				});
			}
			return collection.ToArray<AcpInformation>();
		}
	}
}
