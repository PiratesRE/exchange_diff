using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UnifiedGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "UnifiedGroup", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class UnifiedGroup
	{
		[DataMember(Name = "SmtpAddress", EmitDefaultValue = false)]
		[XmlElement("SmtpAddress", typeof(string))]
		public string SmtpAddress { get; set; }

		[XmlElement("LegacyDN", typeof(string))]
		[DataMember(Name = "LegacyDN", EmitDefaultValue = false)]
		public string LegacyDN { get; set; }

		[DataMember(Name = "DisplayName", EmitDefaultValue = false)]
		[XmlElement("DisplayName", typeof(string))]
		public string DisplayName { get; set; }

		[DataMember(Name = "IsFavorite", EmitDefaultValue = false)]
		[XmlElement("IsFavorite", typeof(bool))]
		public bool IsFavorite { get; set; }

		[DataMember(Name = "AccessType", EmitDefaultValue = false)]
		[XmlElement("AccessType", typeof(UnifiedGroupAccessType))]
		public UnifiedGroupAccessType AccessType { get; set; }
	}
}
