using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlRoot(ElementName = "ExchangeImpersonation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ExchangeImpersonationType
	{
		[XmlElement]
		[DataMember]
		public ConnectingSIDType ConnectingSID
		{
			get
			{
				return this.connectingSIDField;
			}
			set
			{
				this.connectingSIDField = value;
			}
		}

		private ConnectingSIDType connectingSIDField;
	}
}
