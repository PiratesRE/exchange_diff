using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "ModernGroupType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ModernGroupType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ModernGroupType
	{
		[XmlElement("SmtpAddress", typeof(string))]
		[DataMember(Name = "SmtpAddress", EmitDefaultValue = false)]
		public string SmtpAddress { get; set; }

		[DataMember(Name = "DisplayName", EmitDefaultValue = false)]
		[XmlElement("DisplayName", typeof(string))]
		public string DisplayName { get; set; }

		[XmlElement("IsPinned", typeof(bool))]
		[DataMember(Name = "IsPinned", EmitDefaultValue = false)]
		public bool IsPinned { get; set; }

		[XmlElement("GroupObjectType", typeof(ModernGroupObjectType))]
		[DataMember(Name = "GroupObjectType", EmitDefaultValue = false)]
		public ModernGroupObjectType GroupObjectType { get; set; }
	}
}
