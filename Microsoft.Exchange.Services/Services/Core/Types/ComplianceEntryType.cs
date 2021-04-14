using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ComplianceEntryType
	{
		public ComplianceEntryType(string id, string name, string description)
		{
			this.Id = id;
			this.Name = name;
			this.Description = description;
		}

		[DataMember]
		[XmlElement("Id", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string Id { get; set; }

		[DataMember]
		[XmlElement("Name", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string Name { get; set; }

		[XmlElement("Description", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember]
		public string Description { get; set; }
	}
}
