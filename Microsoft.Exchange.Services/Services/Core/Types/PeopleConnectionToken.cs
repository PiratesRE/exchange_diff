using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PeopleConnectionTokenType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "PeopleConnectionToken", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public sealed class PeopleConnectionToken
	{
		[DataMember]
		[XmlElement]
		public string AccessToken { get; set; }

		[XmlElement]
		[DataMember]
		public string ApplicationId { get; set; }

		[DataMember]
		[XmlElement]
		public string ApplicationSecret { get; set; }
	}
}
