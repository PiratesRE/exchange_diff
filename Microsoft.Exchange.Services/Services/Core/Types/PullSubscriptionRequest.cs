using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PullSubscriptionRequest")]
	[XmlType("PullSubscriptionRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class PullSubscriptionRequest : SubscriptionRequestBase
	{
		[XmlElement("Timeout", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "Timeout", IsRequired = true)]
		public int Timeout { get; set; }
	}
}
