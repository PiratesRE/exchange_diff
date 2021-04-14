using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "Token", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Token")]
	[Serializable]
	public class ClientAccessTokenResponseType
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public ClientAccessTokenType TokenType { get; set; }

		[DataMember(Name = "TokenType", IsRequired = true, Order = 2)]
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

		[DataMember(Order = 3)]
		public string TokenValue { get; set; }

		[DataMember(Order = 4)]
		public int TTL { get; set; }
	}
}
