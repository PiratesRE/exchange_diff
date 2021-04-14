using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "TokenRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientAccessTokenRequestType
	{
		[DataMember]
		public string Id { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public ClientAccessTokenType TokenType { get; set; }

		[DataMember(Name = "TokenType", IsRequired = true)]
		[XmlIgnore]
		public string TokenTypeString
		{
			get
			{
				return EnumUtilities.ToString<ClientAccessTokenType>(this.TokenType);
			}
			set
			{
				this.TokenType = EnumUtilities.Parse<ClientAccessTokenType>(value);
			}
		}

		[DataMember]
		[XmlElement]
		public string Scope { get; set; }
	}
}
