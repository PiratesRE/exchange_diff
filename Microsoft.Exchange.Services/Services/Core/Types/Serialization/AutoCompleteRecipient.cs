using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types.Serialization
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AutoCompleteRecipientType")]
	[XmlType("AutoCompleteRecipientType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AutoCompleteRecipient
	{
		[DataMember(Order = 1)]
		[XmlElement]
		public string PersonaId { get; set; }

		[XmlElement]
		[DataMember(Order = 2)]
		public string RecipientId { get; set; }

		[XmlElement]
		[DataMember(Order = 3)]
		public string EmailAddress { get; set; }

		[XmlElement]
		[DataMember(Order = 4)]
		public int RelevanceScore { get; set; }

		[XmlElement]
		[DataMember(Order = 5)]
		public string DisplayName { get; set; }

		[XmlElement]
		[DataMember(Order = 6)]
		public string FolderName { get; set; }

		[DataMember(Order = 7)]
		[XmlElement]
		public string Surname { get; set; }

		[XmlElement]
		[DataMember(Order = 8)]
		public string GivenName { get; set; }
	}
}
