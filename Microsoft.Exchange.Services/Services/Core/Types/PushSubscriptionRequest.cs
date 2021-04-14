using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("PushSubscriptionRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PushSubscriptionRequest")]
	public class PushSubscriptionRequest : SubscriptionRequestBase
	{
		[DataMember(Name = "StatusFrequency", IsRequired = true, Order = 1)]
		[XmlElement("StatusFrequency", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public int StatusFrequency { get; set; }

		[XmlElement("URL", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "URL", IsRequired = true, Order = 1)]
		public string Url { get; set; }

		[DataMember(Name = "CallerData", IsRequired = false, Order = 2)]
		[XmlElement("CallerData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string CallerData { get; set; }
	}
}
